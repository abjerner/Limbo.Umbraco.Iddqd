using System.Collections.Generic;
using System.Reflection.Emit;
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
    /// Gets the name of the tab.
    /// </summary>
    [JsonProperty("name", Order = -20)]
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Gets a list of properties making up the tab.
    /// </summary>
    [JsonProperty("properties", Order = -10)]
    public List<ContentAppProperty> Properties { get; } = [];

}