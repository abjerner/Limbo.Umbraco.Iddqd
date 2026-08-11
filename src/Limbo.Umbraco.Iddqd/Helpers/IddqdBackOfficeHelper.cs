using System;
using System.Collections.Generic;
using System.Linq;
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
    private readonly ISet<string> _allowedGroups = new HashSet<string>(StringComparer.InvariantCultureIgnoreCase) { "admin", "iddqd" };

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
    /// Returns a cache buster value based both on Umbraco's own cache buster and the current version of
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