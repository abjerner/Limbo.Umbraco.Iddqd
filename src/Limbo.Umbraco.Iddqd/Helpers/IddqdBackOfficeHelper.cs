using System.Collections.Generic;
using Limbo.Umbraco.Iddqd.Models.ContentApps;
using Umbraco.Cms.Core.Models;
using Umbraco.Cms.Core.Models.ContentEditing;
using Umbraco.Cms.Core.Models.Membership;

namespace Limbo.Umbraco.Iddqd.Helpers;

/// <summary>
/// Backoffice helper class used throughout the Iddqd package.
/// </summary>
public class IddqdBackOfficeHelper {

    /// <summary>
    /// Returns the content app for the specified <paramref name="source"/>.
    /// </summary>
    /// <param name="source">The source - eg. an <see cref="IContent"/> or <see cref="IMedia"/>.</param>
    /// <param name="userGroups">The user groups of the user.</param>
    /// <returns>An instance of <see cref="ContentApp"/>, or <see langword="null"/> if a content app shouldn't be shown for <paramref name="source"/>.</returns>
    public virtual ContentApp? GetContentApp(object source, IEnumerable<IReadOnlyUserGroup> userGroups) {

        return source switch {
            IContent content => GetContentApp(content),
            IMedia media => GetContentApp(media),
            //IContentType contentType => GetContentApp(contentType),
            //IMediaType mediaType => GetContentApp(mediaType),
            _ => null
        };

    }

    /// <summary>
    /// Returns the content app for the specified <paramref name="content"/> node.
    /// </summary>
    /// <param name="content">The content node.</param>
    /// <returns>An instance of <see cref="ContentApp"/> if the content app supposed to be shown; otherwise, <see langword="null"/>.</returns>
    protected virtual ContentApp? GetContentApp(IContent content) {

        if (content.ContentType.IsElement) return null;

        return new ContentApp {
            Alias = "iddqd",
            Name = "Iddqd",
            Icon = "icon-lab",
            Weight = 101,
            View = $"/App_Plugins/{IddqdPackage.Alias}/Views/ContentApp.html",
            ViewModel = new ContentAppModel {
                Id = content.Id,
                Key = content.Key,
                Section = "content",
                Tabs = {
                    new ContentAppTab {
                        Alias = "examine",
                        Label = "Examine",
                        Properties = {
                            new ContentAppProperty($"/App_Plugins/{IddqdPackage.Alias}/Views/ContentApps/Examine.html")
                        }
                    }
                }
            }
        };

    }

    /// <summary>
    /// Returns the content app for the specified <paramref name="media"/> node.
    /// </summary>
    /// <param name="media">The content node.</param>
    /// <returns>An instance of <see cref="ContentApp"/> if the media app supposed to be shown; otherwise, <see langword="null"/>.</returns>
    protected virtual ContentApp? GetContentApp(IMedia media) {

        if (media.ContentType.IsElement) return null;

        return new ContentApp {
            Alias = "iddqd",
            Name = "Iddqd",
            Icon = "icon-lab",
            Weight = 101,
            View = $"/App_Plugins/{IddqdPackage.Alias}/Views/ContentApp.html",
            ViewModel = new ContentAppModel {
                Id = media.Id,
                Key = media.Key,
                Section = "media",
                Tabs = {
                    new ContentAppTab {
                        Alias = "examine",
                        Label = "Examine",
                        Properties = {
                            new ContentAppProperty($"/App_Plugins/{IddqdPackage.Alias}/Views/ContentApps/Examine.html")
                        }
                    }
                }
            }
        };

    }

}