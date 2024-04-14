using System.Collections.Generic;
using Newtonsoft.Json;
using Skybrud.Essentials.Collections;
using Skybrud.Essentials.Json.Newtonsoft.Converters.Enums;

#pragma warning disable CS1591

namespace Limbo.Umbraco.Iddqd.Models.Packages;

public class IddqdPackageListResult {

    [JsonProperty("type")]
    [JsonConverter(typeof(EnumCamelCaseConverter))]
    public IddqdPackageType Type { get; }

    [JsonProperty("sortField")]
    [JsonConverter(typeof(EnumCamelCaseConverter))]
    public IddqdPackageField SortField { get; }

    [JsonProperty("sortOrder")]
    [JsonConverter(typeof(EnumCamelCaseConverter))]
    public SortOrder SortOrder { get; }

    [JsonProperty("groupBy")]
    [JsonConverter(typeof(EnumCamelCaseConverter))]
    public IddqdPackageField GroupBy { get; }

    [JsonProperty("groups")]
    public IEnumerable<IddqdPackageGroup> Groups { get; }

    public IddqdPackageListResult(IddqdPackageType type, IddqdPackageField sortField, SortOrder sortOrder, IddqdPackageField groupBy, IEnumerable<IddqdPackageGroup> groups) {
        Type = type;
        SortField = sortField;
        SortOrder = sortOrder;
        GroupBy = groupBy;
        Groups = groups;
    }

}