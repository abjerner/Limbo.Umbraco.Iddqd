using Limbo.Umbraco.Iddqd.ContentApps;
using Limbo.Umbraco.Iddqd.Helpers;
using Limbo.Umbraco.Iddqd.Notifications.Handlers;
using Limbo.Umbraco.Iddqd.Services;
using Microsoft.Extensions.DependencyInjection;
using Umbraco.Cms.Core.Composing;
using Umbraco.Cms.Core.DependencyInjection;
using Umbraco.Cms.Core.Notifications;

namespace Limbo.Umbraco.Iddqd.Composers;

/// <inheritdoc />
public class IddqdComposer : IComposer {

    /// <inheritdoc />
    public void Compose(IUmbracoBuilder builder) {

        builder.ManifestFilters().Append<IddqdManifestFilter>();

        builder.ContentApps().Append<IddqdContentAppFactory>();

        builder.Services.AddSingleton<IddqdService>();
        builder.Services.AddSingleton<IddqdBackOfficeHelper>();

        builder.AddNotificationHandler<ServerVariablesParsingNotification, ServerVariablesParsingHandler>();

    }

}