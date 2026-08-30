using System;
using System.Collections.Generic;
using Umbraco.Cms.Core.Models;

namespace Limbo.Umbraco.Iddqd.Models.DataTypes;

/// <summary>
/// Class with information about an <see cref="IDataType"/>.
/// </summary>
public class IddqdDataType {

    /// <summary>
    /// Gets the numeric ID of the data type.
    /// </summary>
    public required int Id { get; init; }

    /// <summary>
    /// Gets the GUID key of the data type.
    /// </summary>
    public required Guid Key { get; init; }

    /// <summary>
    /// Gets the name of the data type.
    /// </summary>
    public required string Name { get; init; }

    /// <summary>
    /// Gets the path opf the data type.
    /// </summary>
    public required List<object> Path { get; init; }

    public required string EditorAlias { get; init; }

    public string? EditorUiAlias { get; init; }

    public required string DatabaseType { get; init; }

    public IddqdDataEditor? Editor { get; init; }

    public required DateTime CreateDate { get; init; }

    public required DateTime UpdateDate { get; init; }

}