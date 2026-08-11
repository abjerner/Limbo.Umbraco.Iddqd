using Umbraco.Cms.Api.Management.OpenApi;

namespace Limbo.Umbraco.Iddqd.Api;

internal class IddqdSecurityFilter : BackOfficeSecurityRequirementsOperationFilterBase {

    protected override string ApiName => IddqdApiConstants.Name;

}