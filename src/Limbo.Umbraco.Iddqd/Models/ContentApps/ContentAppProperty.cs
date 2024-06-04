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
    /// Gets the data of the property.
    /// </summary>
    [JsonProperty("data", NullValueHandling = NullValueHandling.Ignore)]
    public object? Data { get; }

    /// <summary>
    /// Initializes a new instance based on the specified <paramref name="view"/> URL.
    /// </summary>
    /// <param name="view">The URL of the view.</param>
    public ContentAppProperty(string view) {
        View = view;
    }

    /// <summary>
    /// Initializes a new instance based on the specified <paramref name="view"/> and <paramref name="data"/>.
    /// </summary>
    /// <param name="view">The URL of the view.</param>
    /// <param name="data">The data of the property.</param>
    public ContentAppProperty(string view, object? data) {
        View = view;
        Data = data;
    }

}