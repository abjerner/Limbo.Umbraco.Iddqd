using System.Collections.Generic;
using Newtonsoft.Json;

namespace Limbo.Umbraco.Iddqd.Models.ContentApps;

/// <summary>
/// Class representing a tab of a content app.
/// </summary>
public class ContentAppTab {

    /// <summary>
    /// Gets the alias of the tap.
    /// </summary>
    [JsonProperty("alias", Order = -30)]
    public string Alias { get; set; } = string.Empty;

    /// <summary>
    /// Gets the label of the tab.
    /// </summary>
    [JsonProperty("label", Order = -20)]
    public string Label { get; set; } = string.Empty;

    /// <summary>
    /// Gets a list of properties making up the tab.
    /// </summary>
    [JsonProperty("properties", Order = -10)]
    public List<ContentAppProperty> Properties { get; } = new();

}