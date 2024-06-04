using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace Limbo.Umbraco.Iddqd.Models.ContentApps;

/// <summary>
/// Class representing the model for a content app.
/// </summary>
public class ContentAppModel {

    /// <summary>
    /// Gets the ID of the undelrying source.
    /// </summary>
    [JsonProperty("id")]
    public int Id { get; set; }

    /// <summary>
    /// Gets the GUID key of the undelrying source.
    /// </summary>
    [JsonProperty("key", NullValueHandling = NullValueHandling.Ignore)]
    public Guid Key { get; set; }

    /// <summary>
    /// Gets the current section.
    /// </summary>
    [JsonProperty("section", NullValueHandling = NullValueHandling.Ignore)]
    public string? Section { get; set; }

    /// <summary>
    /// Gets a list of tabs to be shown in the content app.
    /// </summary>
    [JsonProperty("tabs", Order = -10)]
    public List<ContentAppTab> Tabs { get; } = [];

}