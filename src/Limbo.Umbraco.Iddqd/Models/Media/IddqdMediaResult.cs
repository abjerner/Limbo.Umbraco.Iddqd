using System;

namespace Limbo.Umbraco.Iddqd.Models.Media;

public class IddqdMediaResult {

    public required int Id { get; init; }

    public required Guid Key { get; init; }

    public required string Name { get; init; }

    public required DateTime CreateDate { get; init; }

    public required DateTime UpdateDate { get; init; }

}