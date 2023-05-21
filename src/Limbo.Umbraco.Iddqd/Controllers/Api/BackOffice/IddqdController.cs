using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Examine;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json.Linq;
using Skybrud.Essentials.Json.Newtonsoft.Extensions;
using Umbraco.Cms.Core.Models;
using Umbraco.Cms.Core.PropertyEditors;
using Umbraco.Cms.Core.Services;
using Umbraco.Cms.Infrastructure.Examine;
using Umbraco.Cms.Web.BackOffice.Controllers;
using Umbraco.Cms.Web.Common.Attributes;

#pragma warning disable CS1591

namespace Limbo.Umbraco.Iddqd.Controllers.Api.BackOffice {

    [PluginController("Limbo")]
    public class IddqdController : UmbracoAuthorizedApiController {

        private readonly IContentService _contentService;
        private readonly IMediaService _mediaService;
        private readonly IExamineManager _examineManager;
        private readonly IContentValueSetBuilder _contentValueSetBuilder;
        private readonly IValueSetBuilder<IMedia> _mediaValueSetBuilder;
        private readonly IServiceProvider _serviceProvider;

        public IddqdController(IContentService contentService, IMediaService mediaService, IExamineManager examineManager, IContentValueSetBuilder contentValueSetBuilder, IValueSetBuilder<IMedia> mediaValueSetBuilder, IServiceProvider serviceProvider) {
            _contentService = contentService;
            _mediaService = mediaService;
            _examineManager = examineManager;
            _contentValueSetBuilder = contentValueSetBuilder;
            _mediaValueSetBuilder = mediaValueSetBuilder;
            _serviceProvider = serviceProvider;
        }

        public IEnumerable<object> GetExamineResultForContent(string id, string section, string contentTypeAlias) {

            List<string> indexNames;

            switch (section) {

                case "content":
                    indexNames = new List<string> { "ExternalIndex", "InternalIndex" };
                    break;

                case "media":
                    indexNames = new List<string> { "ExternalIndex", "InternalIndex" };
                    if (contentTypeAlias == "umbracoMediaArticle") indexNames.Add("PDFIndex");
                    break;

                default:
                    indexNames = new List<string>();
                    break;

            }

            foreach (string indexName in indexNames) {

                if (!_examineManager.TryGetIndex(indexName, out IIndex? index)) continue;

                ISearchResult? result = index.Searcher
                    .CreateQuery()
                    .NativeQuery($"id:{id}")
                    .Execute()
                    .FirstOrDefault();

                if (result is null) {
                    yield return new { indexName, result = default(object) };
                    continue;
                }

                string indexType = result.Values["__IndexType"];

                yield return new {
                    indexName,
                    indexType,
                    id = result.Id,
                    score = result.Score,
                    nodeTypeAlias = result.Values["__NodeTypeAlias"],
                    editId = result.Id,
                    editSection = section,
                    editUrl = $"/{section}/{section}/edit/{result.Id}",
                    name = result.Values.TryGetValue("nodeName", out string? nodeName) ? nodeName : null,
                    fieldCount = result.Values.Count,
                    canReIndex = true,
                    values = result.AllValues
                };

            }

        }

        public object ReIndexNode(string indexName, string id, string indexType, string section, string contentTypeAlias) {

            if (!_examineManager.TryGetIndex(indexName, out IIndex index)) throw new Exception("Index not found.");

            switch (indexName) {

                case "ExternalIndex":
                case "InternalIndex":

                    if (indexType == "content") {

                        if (!int.TryParse(id, out int intId)) throw new Exception("Invalid ID.");

                        IContent? content = _contentService.GetById(intId);
                        if (content == null) throw new Exception("Content not found.");

                        var valueSets = _contentValueSetBuilder.GetValueSets(content);

                        index.IndexItems(valueSets);

                    } else if (indexType == "media") {

                        if (!int.TryParse(id, out int intId)) throw new Exception("Invalid ID.");

                        IMedia? media = _mediaService.GetById(intId);
                        if (media == null) throw new Exception("Media not found.");

                        var valueSets = _mediaValueSetBuilder.GetValueSets(media);

                        index.IndexItems(valueSets);

                    } else {

                        throw new Exception($"Unsupported type {indexType}.");

                    }

                    break;

                case "PDFIndex":

                    if (indexType == "pdf" || indexType == "media") {

                        if (!int.TryParse(id, out int intId)) throw new Exception("Invalid ID.");

                        Type? type = Type.GetType("UmbracoExamine.PDF.IPdfIndexValueSetBuilder, UmbracoExamine.PDF");
                        if (type is null) throw new Exception("Type 'UmbracoExamine.PDF.IPdfIndexValueSetBuilder' not found.");

                        if (_serviceProvider.GetService(type) is not IValueSetBuilder<IMedia> pdfValueSetBuilder) throw new Exception("Service of type 'UmbracoExamine.PDF.IPdfIndexValueSetBuilder' not found.");

                        IMedia? media = _mediaService.GetById(intId);
                        if (media == null) throw new Exception("Media not found.");

                        var valueSets = pdfValueSetBuilder.GetValueSets(media);

                        index.IndexItems(valueSets);

                    } else {

                        throw new Exception($"Unsupported index type {indexType}.");

                    }

                    break;

                default:

                    throw new Exception($"Unsupported index {indexName}.");


            }

            // Seems like a necessary sleep as Umbraco/Examine needs to finish indexing the node
            Thread.Sleep(2500);

            return GetExamineResultForContent(id, section, contentTypeAlias);

        }

        public object GetPropertyEditors() {

            static JObject ToJson(IDataEditor editor) {
                return new JObject {
                    {"alias", editor.Alias},
                    {"icon", editor.Icon},
                    {"group", editor.Group},
                    {"name", editor.Name},
                    {"deprecated", editor.IsDeprecated},
                    {"type", editor.GetType().FullName}
                };
            }

            static JObject DataTypeToJson(IDataType dataType) {
                return new JObject {
                    {"id", dataType.Id},
                    {"key", dataType.Key},
                    {"name", dataType.Name},
                    {"editUrl", $"/umbraco/#/settings/dataTypes/edit/{dataType.Id}"}
                };
            }

            var dataTypesByEditorAlias = _serviceProvider
                .GetRequiredService<IDataTypeService>()
                .GetAll()
                .GroupBy(x => x.EditorAlias)
                .ToDictionary(x => x.Key, x => x);

            var propertyEditors = _serviceProvider
                .GetRequiredService<PropertyEditorCollection>()
                .OrderBy(x => x.Alias);

            List<JObject> temp = new();

            foreach (var group in propertyEditors.GroupBy(x => x.Alias)) {

                string alias = group.Key;

                dataTypesByEditorAlias.TryGetValue(alias, out IGrouping<string, IDataType>? dataTypes);

                foreach (var editor in group) {

                    JObject json = ToJson(editor);

                    JArray duplicates = new();
                    foreach (var duplicate in group.Where(x => x != editor)) {
                        duplicates.Add(ToJson(duplicate));
                    }
                    if (duplicates.Count > 0) json.Add("duplicates", duplicates);

                    JArray dataTypesArray = new();
                    if (dataTypes is not null) {
                        foreach (IDataType dataType in dataTypes) {
                            dataTypesArray.Add(DataTypeToJson(dataType));
                        }
                    }
                    json.Add("dataTypes", dataTypesArray);

                    temp.Add(json);

                }

            }

            return temp
                .OrderBy(x => x.GetString("group"))
                .ThenBy(x => x.GetString("alias"))
                .GroupBy(x => x.GetString("group"))
                .Select(x => new { name = x.Key, editors = x });

        }

    }

}