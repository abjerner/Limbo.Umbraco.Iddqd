using System;
using System.Collections.Generic;
using Newtonsoft.Json;
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
    [JsonProperty("id")]
    public int Id => _dataType.Id;

    /// <summary>
    /// Gets the GUID key of the data type.
    /// </summary>
    [JsonProperty("key")]
    public Guid Key => _dataType.Key;

    /// <summary>
    /// Gets the name of the data type.
    /// </summary>
    [JsonProperty("name")]
    public string Name => _dataType.Name!;

    /// <summary>
    /// Gets the path opf the data type.
    /// </summary>
    [JsonProperty("path")]
    public List<object> Path { get; }

    /// <summary>
    /// Initializes a new instance based on the specified <paramref name="dataType"/> and <paramref name="path"/>.
    /// </summary>
    /// <param name="dataType">The data type.</param>
    /// <param name="path">The path of the data type, excluding the data type it self.</param>
    public IddqdDataType(IDataType dataType, List<object> path) {
        _dataType = dataType;
        Path = path;
    }

}