using Newtonsoft.Json;
using Umbraco.Cms.Core.Models.Membership;

namespace Limbo.Umbraco.Iddqd.Models.ContentApps.Users;

/// <summary>
/// Class with limited information about a backoffice user.
/// </summary>
public class ApiUser {

    /// <summary>
    /// Gets the numeric ID of the user.
    /// </summary>
    [JsonProperty("id")]
    public int Id { get; }

    /// <summary>
    /// Gets the name of the user.
    /// </summary>
    [JsonProperty("name")]
    public string? Name { get; }

    /// <summary>
    /// Initializes a new instance based on the specified <paramref name="id"/>. Notice that other properties will not be populated when using this constructor.
    /// </summary>
    /// <param name="id">The ID of the user.</param>
    public ApiUser(int id) {
        Id = id;
    }

    /// <summary>
    /// Initializes a new instance based on the specified <paramref name="user"/>.
    /// </summary>
    /// <param name="user">An instance of <see cref="IUser"/> representing the user.</param>
    public ApiUser(IUser user) {
        Id = user.Id;
        Name = user.Name;
    }

}