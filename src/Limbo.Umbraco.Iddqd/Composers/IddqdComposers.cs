using Limbo.Umbraco.Iddqd.Api;
using Limbo.Umbraco.Iddqd.Factories;
using Limbo.Umbraco.Iddqd.Helpers;
using Limbo.Umbraco.Iddqd.Services;
using Microsoft.Extensions.DependencyInjection;
using Skybrud.Essentials.Umbraco.Composing;
using Umbraco.Cms.Core.Composing;
using Umbraco.Cms.Core.DependencyInjection;

namespace Limbo.Umbraco.Iddqd.Composers;

public class IddqdComposer : IComposer {

    public void Compose(IUmbracoBuilder builder) {
        builder.AddPackageManifestReader<IddqdPackageManifestReader>();
        builder.Services.ConfigureOptions<IddqdSwaggerGenOptions>();
        builder.Services.AddSingleton<IddqdHelper>();
        builder.Services.AddSingleton<IddqdService>();
        builder.Services.AddSingleton<IddqdServiceDependencies>();
        builder.Services.AddSingleton<IddqdModelFactory>();
        builder.Services.AddTransient<IddqdRequestHelper>();
    }

}