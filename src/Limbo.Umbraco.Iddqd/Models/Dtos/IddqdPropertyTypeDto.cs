using System;

namespace Limbo.Umbraco.Iddqd.Models.Dtos;

public class IddqdPropertyTypeDto {

    public required int ContentTypeId { get; init; }

    public required Guid ContentTypeKey { get; init; }

    public required string ContentTypeAlias { get; init; }

    public required string ContentTypeIcon { get; init; }

    public required string ContentTypeName { get; init; }

    public required string PropertyAlias { get; init; }

    public required string PropertyName { get; init; }

    public required Guid NodeObjectType { get; init; }

    public string NodeObjectAlias => NodeObjectType.ToString("D").ToUpper() switch {
        global::Umbraco.Cms.Core.Constants.ObjectTypes.Strings.DocumentType => "ContentType",
        global::Umbraco.Cms.Core.Constants.ObjectTypes.Strings.MediaType => "MediaType",
        global::Umbraco.Cms.Core.Constants.ObjectTypes.Strings.MemberType => "MemberType",
        _ => "Unknown"
    };

}