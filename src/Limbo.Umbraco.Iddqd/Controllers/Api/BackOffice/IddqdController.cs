using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Examine;
using Limbo.Umbraco.Iddqd.Models;
using Microsoft.Extensions.DependencyInjection;
using Skybrud.Essentials.Collections;
using Skybrud.Essentials.Collections.Extensions;
using Skybrud.Essentials.Enums;
using Skybrud.Essentials.Strings;
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

            PropertyEditorField sortField = EnumUtils.ParseEnum(Request.Query["sortField"], PropertyEditorField.Alias);
            PropertyEditorField groupBy = EnumUtils.ParseEnum(Request.Query["groupBy"], PropertyEditorField.None);
            SortOrder sortOrder = Request.Query["sortOrder"].ToString() is "descending" ? SortOrder.Descending : SortOrder.Ascending;

            var dataTypesByEditorAlias = _serviceProvider
                .GetRequiredService<IDataTypeService>()
                .GetAll()
                .GroupBy(x => x.EditorAlias)
                .ToDictionary(x => x.Key, x => x);

            var propertyEditors = _serviceProvider
                .GetRequiredService<PropertyEditorCollection>()
                .Select(x => new PropertyEditor(x, dataTypesByEditorAlias))
                .ToList();

            foreach (IGrouping<string, PropertyEditor> group in propertyEditors.GroupBy(x => x.Alias)) {
                foreach (PropertyEditor editor in group) {
                    foreach (PropertyEditor duplicate in group.Where(x => x != editor)) {
                        editor.Duplicates.Add(new PropertyEditorItem(duplicate.Editor));
                    }
                }
            }

            Func<PropertyEditor, string?> keySelector = groupBy switch {
                PropertyEditorField.None => (_) => "All",
                PropertyEditorField.Alias => x => x.Alias,
                PropertyEditorField.Name => x => x.Name,
                PropertyEditorField.ValueType => x => x.ValueType ?? " Empty",
                PropertyEditorField.Assembly => x => x.Assembly.Name ?? " Empty",
                PropertyEditorField.Company => x => x.Assembly.Company ?? " Empty",
                _ => x => StringUtils.FirstWithValue(x.Group, " Empty")
            };

            IEnumerable<PropertyEditorGroup> groups = propertyEditors
                .GroupBy(keySelector)
                .Select(x => CreateGroup(x.Key, x, sortField, sortOrder))
                .OrderBy(x => x.Name);

            return new PropertyEditorListResult(sortField, sortOrder, groupBy, groups);

        }

        private PropertyEditorGroup CreateGroup(string? name, IEnumerable<PropertyEditor> propertyEditors, PropertyEditorField sortField, SortOrder sortOrder) {
            return new PropertyEditorGroup(name, Sort(propertyEditors, sortField, sortOrder));
        }

        private IEnumerable<PropertyEditor> Sort(IEnumerable<PropertyEditor> propertyEditors, PropertyEditorField sortField, SortOrder sortOrder) {
            return sortField switch {
                PropertyEditorField.Assembly => propertyEditors.OrderBy(x => x.Assembly.Name, sortOrder),
                PropertyEditorField.Group => propertyEditors.OrderBy(x => x.Group, sortOrder),
                PropertyEditorField.Name => propertyEditors.OrderBy(x => x.Name, sortOrder),
                PropertyEditorField.ValueType => propertyEditors.OrderBy(x => x.ValueType, sortOrder),
                PropertyEditorField.DataTypes => propertyEditors.OrderBy(x => x.DataTypes.Count, sortOrder),
                _ => propertyEditors.OrderBy(x => x.Alias, sortOrder)
            };

        }

    }

}