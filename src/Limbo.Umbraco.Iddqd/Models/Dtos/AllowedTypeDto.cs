using NPoco;

namespace Limbo.Umbraco.Iddqd.Models.Dtos;

/// <summary>
/// Class representing a DTO for the relation between a content type and the content type allowed to be created under it.
/// </summary>
[TableName("cmsContentTypeAllowedContentType")]
public class AllowedTypeDto {

    /// <summary>
    /// Gets the ID of the relation (the parent).
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Gets the ID of the allowed content type (the child)
    /// </summary>
    public int AllowedId { get; set; }

    /// <summary>
    /// Gets the sort order of the relation.
    /// </summary>
    public int SortOrder { get; set; }

}