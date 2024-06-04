using System;
using Newtonsoft.Json;
using Umbraco.Cms.Core.Models;

#pragma warning disable CS1591

namespace Limbo.Umbraco.Iddqd.Models.ContentApps.ContentTypes;

public class ApiPropertyType {

    private readonly IPropertyType _propertyType;

    [JsonProperty("id")]
    public int Id => _propertyType.Id;

    [JsonProperty("key")]
    public Guid Key => _propertyType.Key;

    [JsonProperty("alias")]
    public string Alias => _propertyType.Alias;

    [JsonProperty("name")]
    public string Name => _propertyType.Name;

    [JsonProperty("sortOrder")]
    public int SortOrder => _propertyType.SortOrder;

    [JsonProperty("editorAlias")]
    public string EditorAlias => _propertyType.PropertyEditorAlias;

    [JsonProperty("dataType")]
    public ApiDataType? DataType { get; }

    public ApiPropertyType(IPropertyType propertyType, ApiDataType? dataType) {
        _propertyType = propertyType;
        DataType = dataType;
    }

}