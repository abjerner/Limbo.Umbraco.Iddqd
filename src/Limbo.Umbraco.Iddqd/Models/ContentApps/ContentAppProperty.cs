using Newtonsoft.Json;

namespace Limbo.Umbraco.Iddqd.Models.ContentApps;

/// <summary>
/// Class representing s property of a content app.
/// </summary>
public class ContentAppProperty {

    /// <summary>
    /// Gets the view of the property.
    /// </summary>
    [JsonProperty("view", Order = -10)]
    public string View { get; }

    /// <summary>
    /// Initializes a new instance based on the specified <paramref name="view"/> URL.
    /// </summary>
    /// <param name="view">The URL of the view.</param>
    public ContentAppProperty(string view) {
        View = view;
    }

}