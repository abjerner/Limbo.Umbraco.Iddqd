using Limbo.Umbraco.Iddqd.Models.Assemblies;

namespace Limbo.Umbraco.Iddqd.Models.DataTypes;

public class IddqdDataEditor {

    public required string Alias { get; init; }

    public required string Type { get; init; }

    public required bool IsReadOnly { get; init; }

    public required string ValueType { get; init; }

    public required string DatabaseType { get; init; }

    public required IddqdAssembly Assembly { get; init; }

}