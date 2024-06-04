namespace Limbo.Umbraco.Iddqd.Models.Dtos;

/// <summary>
/// Class representing a data type DTO.
/// </summary>
public class DataTypeDto {

    /// <summary>
    /// Gets or sets the ID of the data type.
    /// </summary>
    public int NodeId { get; set; }

    /// <summary>
    /// Gets or sets the config of the data type.
    /// </summary>
    public string Config { get; set; } = null!;

}