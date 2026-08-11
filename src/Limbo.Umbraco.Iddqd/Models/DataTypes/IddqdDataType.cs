using System;
using System.Collections.Generic;
using Umbraco.Cms.Core.Models;

namespace Limbo.Umbraco.Iddqd.Models.DataTypes;

/// <summary>
/// Class with information about an <see cref="IDataType"/>.
/// </summary>
public class IddqdDataType {

    private readonly IDataType _dataType;

    /// <summary>
    /// Gets the numeric ID of the data type.
    /// </summary>
    public int Id => _dataType.Id;

    /// <summary>
    /// Gets the GUID key of the data type.
    /// </summary>
    public Guid Key => _dataType.Key;

    /// <summary>
    /// Gets the name of the data type.
    /// </summary>
    public string Name => _dataType.Name ?? string.Empty;

    /// <summary>
    /// Gets the path opf the data type.
    /// </summary>
    public List<object> Path { get; }

    public string EditorAlias => _dataType.EditorAlias;

    public string? EditorUiAlias => _dataType.EditorUiAlias;

    public DateTime CreateDate => _dataType.CreateDate;

    public DateTime UpdateDate => _dataType.UpdateDate;

    /// <summary>
    /// Initializes a new instance based on the specified <paramref name="dataType"/> and <paramref name="path"/>.
    /// </summary>
    /// <param name="dataType">The data type.</param>
    /// <param name="path">The path of the data type, excluding the data type itself.</param>
    public IddqdDataType(IDataType dataType, List<object> path) {
        _dataType = dataType;
        Path = path;
    }

}