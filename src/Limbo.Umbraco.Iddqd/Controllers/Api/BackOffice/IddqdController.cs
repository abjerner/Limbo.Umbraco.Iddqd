using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading;
using Examine;
using Limbo.Umbraco.Iddqd.Models;
using Limbo.Umbraco.Iddqd.Models.ContentApps.ContentTypes;
using Limbo.Umbraco.Iddqd.Models.DataTypes;
using Limbo.Umbraco.Iddqd.Models.Dtos;
using Limbo.Umbraco.Iddqd.Models.Packages;
using Limbo.Umbraco.Iddqd.Services;
using Microsoft.Extensions.DependencyInjection;
using Skybrud.Essentials.AspNetCore;
using Skybrud.Essentials.Collections;
using Skybrud.Essentials.Collections.Extensions;
using Skybrud.Essentials.Enums;
using Skybrud.Essentials.Strings;
using Skybrud.Essentials.Strings.Extensions;
using Umbraco.Cms.Core.Models;
using Umbraco.Cms.Core.PropertyEditors;
using Umbraco.Cms.Core.Services;
using Umbraco.Cms.Infrastructure.Examine;
using Umbraco.Cms.Infrastructure.Scoping;
using Umbraco.Cms.Web.BackOffice.Controllers;
using Umbraco.Cms.Web.Common.Attributes;
using Umbraco.Extensions;

#pragma warning disable CS1591

namespace Limbo.Umbraco.Iddqd.Controllers.Api.BackOffice;

[PluginController("Limbo")]
public class IddqdController : UmbracoAuthorizedApiController {

    private readonly IScopeProvider _scopeProvider;
    private readonly IContentService _contentService;
    private readonly IContentTypeService _contentTypeService;
    private readonly IDataTypeService _dataTypeService;
    private readonly IMediaService _mediaService;
    private readonly IExamineManager _examineManager;
    private readonly IContentValueSetBuilder _contentValueSetBuilder;
    private readonly IValueSetBuilder<IMedia> _mediaValueSetBuilder;
    private readonly IServiceProvider _serviceProvider;
    private readonly IddqdService _iddqdService;

    public IddqdController(IScopeProvider scopeProvider, IContentService contentService, IContentTypeService contentTypeService, IDataTypeService dataTypeService, IMediaService mediaService, IExamineManager examineManager, IContentValueSetBuilder contentValueSetBuilder, IValueSetBuilder<IMedia> mediaValueSetBuilder, IServiceProvider serviceProvider, IddqdService iddqdService) {
        _scopeProvider = scopeProvider;
        _contentService = contentService;
        _contentTypeService = contentTypeService;
        _dataTypeService = dataTypeService;
        _mediaService = mediaService;
        _examineManager = examineManager;
        _contentValueSetBuilder = contentValueSetBuilder;
        _mediaValueSetBuilder = mediaValueSetBuilder;
        _serviceProvider = serviceProvider;
        _iddqdService = iddqdService;
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

    public object GetContentType(Guid key) {

        IContentType? contentType = _contentTypeService.Get(key);
        if (contentType is null) return NotFound("Content type not found.");

        Dictionary<Guid, IDataType?> dataTypes = [];

        static ApiDataType GetDataType(IPropertyType property, Dictionary<Guid, IDataType?> dataTypes, IDataTypeService dataTypeService) {

            if (!dataTypes.TryGetValue(property.DataTypeKey, out IDataType? dataType)) {
                dataType = dataTypeService.GetDataType(property.DataTypeKey);
                dataTypes[property.DataTypeKey] = dataType;
            }

            return new ApiDataType(property.DataTypeId, property.DataTypeKey, dataType?.Name, dataType?.Editor?.Icon);

        }

        List<ApiPropertyGroup> propertyGroups = [];
        foreach (PropertyGroup propertyGroup in contentType.PropertyGroups) {
            List<ApiPropertyType> propertyTypes = [];
            if (propertyGroup.PropertyTypes is not null) {
                foreach (var propertyType in propertyGroup.PropertyTypes) {
                    ApiDataType dt = GetDataType(propertyType, dataTypes, _dataTypeService);
                    propertyTypes.Add(new ApiPropertyType(propertyType, dt));
                }
            }
            propertyGroups.Add(new ApiPropertyGroup(propertyGroup, propertyTypes));
        }

        return new ApiContentType(contentType, propertyGroups);

    }

    public object GetContentTypeRelations(Guid key) {

        IContentType? contentType = _contentTypeService.Get(key);
        if (contentType is null) return NotFound("Content type not found.");

        List<ApiContentTypeItem> contentTypes = [];

        if (contentType.ParentId > 0) {
            IContentType? parent = _contentTypeService.Get(contentType.ParentId);
            if (parent is not null) contentTypes.Add(new ApiContentTypeItem(parent, "Parent"));
        }

        foreach (var hej in _contentTypeService.GetComposedOf(contentType.Id)) {
            contentTypes.Add(new ApiContentTypeItem(hej, "Composition (child)"));
        }

        foreach (IContentTypeComposition composition in contentType.ContentTypeComposition) {
            contentTypes.Add(new ApiContentTypeItem(composition, "Composition (parent)"));
        }

        using (IScope scope = _scopeProvider.CreateScope(autoComplete: true)) {

            var sql = scope.Database.SqlContext
                .Sql()
                .From<AllowedTypeDto>()
                .Where<AllowedTypeDto>(x => x.AllowedId == contentType.Id);

            foreach (var dto in scope.Database.Fetch<AllowedTypeDto>(sql)) {
                IContentType? ct = _contentTypeService.Get(dto.Id);
                if (ct is not null) contentTypes.Add(new ApiContentTypeItem(ct, "Allowed type (parent)"));
            }

        }

        if (contentType.AllowedContentTypes is not null) {
            foreach (ContentTypeSort allowed in contentType.AllowedContentTypes) {
                var ct = _contentTypeService.Get(allowed.Alias);
                if (ct is not null) contentTypes.Add(new ApiContentTypeItem(ct, "Allowed type (child)"));
            }
        }

        // Not sure about the best way to search database configurations, so at least for now, we find the IDs of the
        // mathcing data types first, and then look them up again using the data type service
        List<int> ids = [];
        using (IScope scope = _scopeProvider.CreateScope(autoComplete: true)) {

            // Passing on the key as a parameter apparently does work, so at least for now, we just include the key
            // directly in the query
            foreach (DataTypeDto dto in scope.Database.Fetch<DataTypeDto>($"SELECT [NodeId],[Config] FROM [dbo].[umbracoDataType] WHERE [config] LIKE '%{key}%' OR [config] LIKE '%@0%'", contentType.Alias)) {

                // Converting the array into a hash set will give us O(1) lookups 😎
                HashSet<string> tokens = dto.Config
                    .ToLowerInvariant()
                    .Split(' ', '\'', '"', ',')
                    .ToHashSet(StringComparer.InvariantCultureIgnoreCase);

                // By looking for the alias of the content type, we accidentally may get incorrect results if the alias
                // matches a property name in the saved JSON. Unless we can come up with a better way to search without
                // knowing the JSON structure in advance, this is acceptable behavior
                if (tokens.Contains(contentType.Key.ToString()) || tokens.Contains(contentType.Alias)) {
                    ids.Add(dto.NodeId);
                }

            }

        }

        // Get the related data types, if any
        IEnumerable<IDataType> relatedDataTypes = ids.Count == 0 ? [] : _dataTypeService.GetAll([..ids]);

        return new {
            contentTypes,
            dataTypes = relatedDataTypes.Select(x => new ApiDataType(x))
        };

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

    public object GetDataTypesByPropertyEditor(string editorAlias) {

        IEnumerable<IDataType> dataTypes = _dataTypeService.GetByEditorAlias(editorAlias);

        List<IddqdDataType> temp = new();

        foreach (var dataType in dataTypes) {

            int[] path = dataType.Path.ToInt32Array();

            List<object> breadcrumb = new();

            foreach (int id in path) {

                if (id == -1) continue;
                if (id == dataType.Id) continue;

                EntityContainer? container = _dataTypeService.GetContainer(id);
                if (container is null) continue;

                breadcrumb.Add(new { id = container.Id, key = container.Key, name = container.Name });

            }

            temp.Add(new IddqdDataType(dataType, breadcrumb));

        }

        return temp;

    }

    public object GetPackages(string? text = null) {

        IddqdPackageType type = Request.Query.GetEnum("type", IddqdPackageType.None);
        IddqdPackageField groupBy = Request.Query.GetEnum("groupBy", IddqdPackageField.None);
        IddqdPackageField sortField = Request.Query.GetEnum("sortField", IddqdPackageField.Alias);
        SortOrder sortOrder = Request.Query.GetEnum("sortOrder", SortOrder.Ascending);

        IEnumerable<IddqdPackageManifest> all = _iddqdService.GetPackages();

        if (!string.IsNullOrWhiteSpace(text)) {
            all = all.Where(x => x.IsMatch(text));
        }

        if (type is not IddqdPackageType.None) all = all.Where(x => x.Type == type);

        Func<IddqdPackageManifest, string?> keySelector = groupBy switch {
            IddqdPackageField.None => _ => "All",
            IddqdPackageField.Alias => x => x.PackageId ?? " No package ID",
            IddqdPackageField.Name => x => x.PackageName,
            IddqdPackageField.Assembly => x => x.Assembly?.Name ?? " No assembly",
            IddqdPackageField.Company => x => x.Assembly?.Company ?? " No company",
            IddqdPackageField.Product => x => x.Assembly?.Product ?? " No product",
            IddqdPackageField.Type => x => x.Type.ToString(),
            _ => x => x.PackageName,
        };

        IEnumerable<IddqdPackageGroup> groups = all
            .GroupBy(keySelector)
            .Select(x => CreatePackageGroup(x.Key, x, sortField, sortOrder))
            .OrderBy(x => x.Name);

        return new IddqdPackageListResult(type, sortField, sortOrder, groupBy, groups);

    }

    private IddqdPackageGroup CreatePackageGroup(string? name, IEnumerable<IddqdPackageManifest> packages, IddqdPackageField sortField, SortOrder sortOrder) {
        return new IddqdPackageGroup(name, SortPackages(packages, sortField, sortOrder));
    }

    private PropertyEditorGroup CreateGroup(string? name, IEnumerable<PropertyEditor> propertyEditors, PropertyEditorField sortField, SortOrder sortOrder) {
        return new PropertyEditorGroup(name, SortPropertyEditors(propertyEditors, sortField, sortOrder));
    }

    private IEnumerable<IddqdPackageManifest> SortPackages(IEnumerable<IddqdPackageManifest> propertyEditors, IddqdPackageField sortField, SortOrder sortOrder) {
        return sortField switch {
            IddqdPackageField.Assembly => propertyEditors.OrderBy(x => x.Assembly?.Name, sortOrder),
            IddqdPackageField.Alias => propertyEditors.OrderBy(x => x.PackageId, sortOrder),
            IddqdPackageField.Name => propertyEditors.OrderBy(x => x.PackageName, sortOrder),
            IddqdPackageField.Company => propertyEditors.OrderBy(x => x.Assembly?.Company, sortOrder),
            IddqdPackageField.Product => propertyEditors.OrderBy(x => x.Assembly?.Product, sortOrder),
            _ => propertyEditors.OrderBy(x => x.PackageName, sortOrder)
        };

    }

    private IEnumerable<PropertyEditor> SortPropertyEditors(IEnumerable<PropertyEditor> propertyEditors, PropertyEditorField sortField, SortOrder sortOrder) {
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