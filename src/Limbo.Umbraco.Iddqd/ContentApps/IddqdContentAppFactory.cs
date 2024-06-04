using System.Collections.Generic;
using Limbo.Umbraco.Iddqd.Helpers;
using Umbraco.Cms.Core.Models;
using Umbraco.Cms.Core.Models.ContentEditing;
using Umbraco.Cms.Core.Models.Membership;

namespace Limbo.Umbraco.Iddqd.ContentApps;

/// <summary>
/// Default content app factory for the Iddqd package.
/// </summary>
public class IddqdContentAppFactory : IContentAppFactory {

    private readonly IddqdBackOfficeHelper _backOfficeHelper;

    /// <summary>
    /// Initializes a new instance based on the specified dependencies.
    /// </summary>
    /// <param name="backOfficeHelper">An instance of <see cref="IddqdBackOfficeHelper"/>.</param>
    public IddqdContentAppFactory(IddqdBackOfficeHelper backOfficeHelper) {
        _backOfficeHelper = backOfficeHelper;
    }

    /// <summary>
    /// Returns the content app for the specified <paramref name="source"/>.
    /// </summary>
    /// <param name="source">The source - e.g. an <see cref="IContent"/> or <see cref="IMedia"/>.</param>
    /// <param name="userGroups">The user groups of the user.</param>
    /// <returns>An instance of <see cref="ContentApp"/>, or <see langword="null"/> if a content app shouldn't be shown for <paramref name="source"/>.</returns>
    public ContentApp? GetContentAppFor(object source, IEnumerable<IReadOnlyUserGroup> userGroups) {
        return _backOfficeHelper.GetContentApp(source, userGroups);
    }

}