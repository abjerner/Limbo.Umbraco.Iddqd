using System;
using Newtonsoft.Json;
using Umbraco.Cms.Core.Models;

#pragma warning disable CS1591

namespace Limbo.Umbraco.Iddqd.Models.ContentApps.ContentTypes;

public class ApiDataType {

    [JsonProperty("id")]
    public int Id { get; }

    [JsonProperty("key")]
    public Guid Key { get; }

    [JsonProperty("name")]
    public string? Name { get; }

    [JsonProperty("editorAlias")]
    public string? EditorAlias { get; }

    [JsonProperty("editorName")]
    public string? EditorName { get; }

    [JsonProperty("editorIcon")]
    public string? EditorIcon { get; }

    public ApiDataType(int id, Guid key, string? name, string? icon) {
        Id = id;
        Key = key;
        Name = name;
        EditorIcon = icon;
    }

    public ApiDataType(IDataType dataType) {
        Id = dataType.Id;
        Key = dataType.Key;
        Name = dataType.Name;
        EditorAlias = dataType.EditorAlias;
        //EditorName = dataType.Editor?.Name;
        //EditorIcon = dataType.Editor?.Icon;
    }

}