using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Umbraco.Cms.Core.Models;

#pragma warning disable CS1591

namespace Limbo.Umbraco.Iddqd.Models.ContentApps.ContentTypes;

public class ApiPropertyGroup {

    private readonly PropertyGroup _propertyGroup;

    [JsonProperty("id")]
    public int Id => _propertyGroup.Id;

    [JsonProperty("key")]
    public Guid Key => _propertyGroup.Key;

    [JsonProperty("alias")]
    public string Alias => _propertyGroup.Alias;

    [JsonProperty("name")]
    public string Name => _propertyGroup.Name ?? string.Empty;

    [JsonProperty("type")]
    public string Type => _propertyGroup.Type.ToString();

    [JsonProperty("sortOrder")]
    public int SortOrder => _propertyGroup.SortOrder;

    [JsonProperty("propertyTypes")]
    public IReadOnlyList<ApiPropertyType> PropertyTypes { get; }

    public ApiPropertyGroup(PropertyGroup propertyGroup, IReadOnlyList<ApiPropertyType> propertyTypes) {
        PropertyTypes = propertyTypes;
        _propertyGroup = propertyGroup;
    }

}