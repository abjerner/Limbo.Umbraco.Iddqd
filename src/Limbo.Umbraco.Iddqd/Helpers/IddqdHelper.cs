using System.Collections.Generic;
using Limbo.Umbraco.Iddqd.Models.Dtos;
using Umbraco.Cms.Core.Models;
using Umbraco.Cms.Infrastructure.Scoping;

namespace Limbo.Umbraco.Iddqd.Helpers;

public class IddqdHelper {

    private readonly IScopeProvider _scopeProvider;

    public IddqdHelper(IScopeProvider scopeProvider) {
        _scopeProvider = scopeProvider;
    }

    public IReadOnlyList<IddqdPropertyTypeDto> GetPropertyTypes(IDataType dataType) {

        const string sql = """
                             SELECT
                                 ct.nodeId          AS ContentTypeId,
                                 n.uniqueId         AS ContentTypeKey,
                                 ct.alias           AS ContentTypeAlias,
                                 ct.icon            AS ContentTypeIcon,
                                 n.[text]           AS ContentTypeName,
                                 pt.id              AS PropertyTypeId,
                                 pt.Alias           AS PropertyAlias,
                                 pt.Name            AS PropertyName,
                                 n.nodeObjectType   AS NodeObjectType
                             FROM cmsPropertyType pt
                             INNER JOIN cmsContentType ct
                                 ON ct.nodeId = pt.contentTypeId
                             INNER JOIN umbracoNode n
                                 ON n.id = ct.nodeId
                             WHERE pt.dataTypeId = @0
                             ORDER BY n.[text], pt.alias;
                             """;

        using IScope scope = _scopeProvider.CreateScope(autoComplete: true);

        return scope.Database.Fetch<IddqdPropertyTypeDto>(sql, dataType.Id);

    }

}