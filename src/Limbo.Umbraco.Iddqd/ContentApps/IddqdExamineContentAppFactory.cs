using System.Collections.Generic;
using System.Linq;
using Umbraco.Cms.Core;
using Umbraco.Cms.Core.Models.ContentEditing;
using Umbraco.Cms.Core.Models.Membership;
using Umbraco.Cms.Core.Models;

#pragma warning disable CS1591

namespace Limbo.Umbraco.Iddqd.ContentApps {

    public class IddqdExamineContentAppFactory : IContentAppFactory {

        public ContentApp? GetContentAppFor(object source, IEnumerable<IReadOnlyUserGroup> userGroups) {

            if (!userGroups.Any(x => x.AllowedSections.Contains(Constants.Applications.Settings))) return null;

            if (source is IMedia or IContent) {
                return new ContentApp {
                    Alias = "examine",
                    Name = "Examine",
                    Icon = "icon-search",
                    View = "/App_Plugins/Limbo.Umbraco.Iddqd/Views/Examine.html"
                };
            }

            return null;

        }

    }

}