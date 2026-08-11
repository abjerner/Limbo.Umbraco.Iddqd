using Umbraco.Cms.Api.Management.OpenApi;

namespace Limbo.Umbraco.Iddqd.Api;

#pragma warning disable CS1591
public class IddqdSecurityFilter : BackOfficeSecurityRequirementsOperationFilterBase {

    protected override string ApiName => IddqdApiConstants.Name;

}