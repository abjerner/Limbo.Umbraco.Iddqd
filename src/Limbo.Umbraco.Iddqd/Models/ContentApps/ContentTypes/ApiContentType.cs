using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Umbraco.Cms.Core.Models;

#pragma warning disable CS1591

namespace Limbo.Umbraco.Iddqd.Models.ContentApps.ContentTypes;

public class ApiContentType {

    private readonly IContentType _contentType;

    [JsonProperty("id")]
    public int Id => _contentType.Id;

    [JsonProperty("key")]
    public Guid Key => _contentType.Key;

    [JsonProperty("alias")]
    public string Alias => _contentType.Alias;

    [JsonProperty("name")]
    public string Name => _contentType.Name ?? string.Empty;

    [JsonProperty("icon")]
    public string Icon => _contentType.Icon ?? string.Empty;

    [JsonProperty("element")]
    public bool IsElement => _contentType.IsElement;

    [JsonProperty("propertyGroups")]
    public IReadOnlyList<ApiPropertyGroup> PropertyGroups { get; }

    public ApiContentType(IContentType contentType, IReadOnlyList<ApiPropertyGroup> propertyGroups) {
        _contentType = contentType;
        PropertyGroups = propertyGroups;
    }

}