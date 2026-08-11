using System;
using Limbo.Umbraco.Iddqd.Text.Json;
using Skybrud.Essentials.Time;

namespace Limbo.Umbraco.Iddqd.Models;

public class IddqdContent {

    public required int Id { get; init; }

    public required Guid Key { get; init; }

    public required string? Name { get; init; }

    [System.Text.Json.Serialization.JsonConverter(typeof(Iso8601TimeConverter))]
    public required EssentialsTime CreateDate { get; init; }

    [System.Text.Json.Serialization.JsonConverter(typeof(Iso8601TimeConverter))]
    public required EssentialsTime UpdateDate { get; init; }

}