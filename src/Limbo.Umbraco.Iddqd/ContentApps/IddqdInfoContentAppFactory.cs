using System.Collections.Generic;
using Umbraco.Cms.Core.Models.ContentEditing;
using Umbraco.Cms.Core.Models.Membership;
using Umbraco.Cms.Core.Models;
using System.Linq;
using Umbraco.Cms.Core;

#pragma warning disable CS1591

namespace Limbo.Umbraco.Iddqd.ContentApps {

    public class IddqdInfoContentAppFactory : IContentAppFactory {

        public ContentApp? GetContentAppFor(object source, IEnumerable<IReadOnlyUserGroup> userGroups) {
            if (!userGroups.Any(x => x.AllowedSections.Contains(Constants.Applications.Settings))) return null;
            return source switch {
                IContentType => new ContentApp {
                    Alias = "info",
                    Name = "Info",
                    Icon = "icon-info",
                    View = $"/App_Plugins/{IddqdPackage.Alias}/Views/ContentType.html"
                },
                IMediaType => new ContentApp {
                    Alias = "info",
                    Name = "Info",
                    Icon = "icon-info",
                    View = $"/App_Plugins/{IddqdPackage.Alias}/Views/ContentType.html"
                },
                _ => null
            };
        }

    }

}