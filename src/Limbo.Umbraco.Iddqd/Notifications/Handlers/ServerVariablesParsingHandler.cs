using System.Collections.Generic;
using Limbo.Umbraco.Iddqd.Helpers;
using Umbraco.Cms.Core.Events;
using Umbraco.Cms.Core.Notifications;

#pragma warning disable 1591

namespace Limbo.Umbraco.Iddqd.Notifications.Handlers;

public class ServerVariablesParsingHandler : INotificationHandler<ServerVariablesParsingNotification> {

    private readonly IddqdBackOfficeHelper _backoffice;

    public ServerVariablesParsingHandler(IddqdBackOfficeHelper backoffice) {
        _backoffice = backoffice;
    }

    public void Handle(ServerVariablesParsingNotification notification) {

        // Get or create the "skybrud" dictionary
        if (!(notification.ServerVariables.TryGetValue("limbo", out object? value) && value is Dictionary<string, object> limbo)) {
            notification.ServerVariables["limbo"] = limbo = new Dictionary<string, object>();
        }

        // Append the "iddqd" dictionary to "limbo"
        limbo.Add("iddqd", _backoffice.GetServerVariables());

    }

}