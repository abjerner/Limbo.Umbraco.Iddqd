using System.Collections.Generic;
using Limbo.Umbraco.Iddqd.Models.ContentApps;
using Umbraco.Cms.Core.Models;
using Umbraco.Cms.Core.Models.ContentEditing;
using Umbraco.Cms.Core.Models.Membership;
using Umbraco.Cms.Core.Services;
using Umbraco.Extensions;

namespace Limbo.Umbraco.Iddqd.Helpers;

/// <summary>
/// Backoffice helper class used throughout the Iddqd package.
/// </summary>
public class IddqdBackOfficeHelper {

    private readonly IRuntimeState _runtimeState;

    #region Constructors

    /// <summary>
    /// Initializes a new instanced based on the specified dependencies.
    /// </summary>
    /// <param name="runtimeState">An instance of <see cref="IRuntimeState"/>.</param>
    public IddqdBackOfficeHelper(IRuntimeState runtimeState) {
        _runtimeState = runtimeState;
    }

    #endregion

    #region Member methods

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

        string v = GetCacheBuster();

        return new ContentApp {
            Alias = "iddqd",
            Name = "Iddqd",
            Icon = "icon-lab",
            Weight = 101,
            View = $"/App_Plugins/{IddqdPackage.Alias}/Views/ContentApp.html?v={v}",
            ViewModel = new ContentAppModel {
                Id = content.Id,
                Key = content.Key,
                Section = "content",
                Tabs = {
                    new ContentAppTab {
                        Alias = "examine",
                        Label = "Examine",
                        Properties = {
                            new ContentAppProperty($"/App_Plugins/{IddqdPackage.Alias}/Views/ContentApps/Examine.html?v={v}")
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

        string v = GetCacheBuster();

        return new ContentApp {
            Alias = "iddqd",
            Name = "Iddqd",
            Icon = "icon-lab",
            Weight = 101,
            View = $"/App_Plugins/{IddqdPackage.Alias}/Views/ContentApp.html?v={v}",
            ViewModel = new ContentAppModel {
                Id = media.Id,
                Key = media.Key,
                Section = "media",
                Tabs = {
                    new ContentAppTab {
                        Alias = "examine",
                        Label = "Examine",
                        Properties = {
                            new ContentAppProperty($"/App_Plugins/{IddqdPackage.Alias}/Views/ContentApps/Examine.html?v={v}")
                        }
                    }
                }
            }
        };

    }

    /// <summary>
    /// Returns a cache buster value based both on Umbraco's own cache buster as well as the current version of
    /// this package. This ensures a new cache buster value when either the ClientDependency version is bumped or
    /// the package is updated.
    /// </summary>
    /// <returns>The cache buster value.</returns>
    public virtual string GetCacheBuster() {
        string version1 = _runtimeState.SemanticVersion.ToSemanticString();
        string version2 = GetInformationVersion();
        return $"{version1}.{_runtimeState.Level}.{version2}".GenerateHash();
    }

    /// <summary>
    /// Gets the information version of the package.
    /// </summary>
    /// <returns>The information version.</returns>
    public string GetInformationVersion() {
        return IddqdPackage.InformationalVersion;
    }

    /// <summary>
    /// Returns a dictonary with server variables for this pacakge, available through <c>Umbraco.Sys.ServerVariables.limbo.iddqd</c> in the backoffice.
    /// </summary>
    /// <returns>An instance of <see cref="Dictionary{TKey,TValue}"/>.</returns>
    public virtual Dictionary<string, object> GetServerVariables() {
        return new Dictionary<string, object> {
            {"cacheBuster", GetCacheBuster()},
            {"version", GetInformationVersion()}
        };
    }

    #endregion

}