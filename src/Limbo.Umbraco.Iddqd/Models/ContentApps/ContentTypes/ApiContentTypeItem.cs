using System;
using Newtonsoft.Json;
using Umbraco.Cms.Core.Models;

#pragma warning disable CS1591

namespace Limbo.Umbraco.Iddqd.Models.ContentApps.ContentTypes;

public class ApiContentTypeItem {

    private readonly IContentTypeBase _contentType;

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

    [JsonProperty("type", NullValueHandling = NullValueHandling.Ignore)]
    public string? Type { get; }

    public ApiContentTypeItem(IContentTypeBase contentType) {
        _contentType = contentType;
    }

    public ApiContentTypeItem(IContentTypeBase contentType, string? type) {
        _contentType = contentType;
        Type = type;
    }

}