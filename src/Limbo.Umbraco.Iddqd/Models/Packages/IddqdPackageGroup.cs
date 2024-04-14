using System.Collections.Generic;
using Newtonsoft.Json;

namespace Limbo.Umbraco.Iddqd.Models.Packages;

#pragma warning disable CS1591

public class IddqdPackageGroup {

    [JsonProperty("name")]
    public string? Name { get; }

    [JsonProperty("items")]
    public IEnumerable<IddqdPackageManifest> Items { get; }

    public IddqdPackageGroup(string? name, IEnumerable<IddqdPackageManifest> items) {
        Name = name;
        Items = items;
    }

}