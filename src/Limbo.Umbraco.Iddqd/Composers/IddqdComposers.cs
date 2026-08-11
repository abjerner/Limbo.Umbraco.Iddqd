using Limbo.Umbraco.Iddqd.Api;
using Limbo.Umbraco.Iddqd.Helpers;
using Limbo.Umbraco.Iddqd.Services;
using Microsoft.Extensions.DependencyInjection;
using Umbraco.Cms.Core.Composing;
using Umbraco.Cms.Core.DependencyInjection;
using Umbraco.Cms.Infrastructure.Manifest;

namespace Limbo.Umbraco.Iddqd.Composers;

public class IddqdComposer : IComposer {

    public void Compose(IUmbracoBuilder builder) {
        builder.Services.ConfigureOptions<IddqdSwaggerGenOptions>();
        builder.Services.AddSingleton<IPackageManifestReader, IddqdPackageManifestReader>();
        builder.Services.AddSingleton<IddqdHelper>();
        builder.Services.AddTransient<IddqdRequestHelper>();
        builder.Services.AddTransient<IddqdService>();
        builder.Services.AddTransient<IddqdServiceDependencies>();
    }

}