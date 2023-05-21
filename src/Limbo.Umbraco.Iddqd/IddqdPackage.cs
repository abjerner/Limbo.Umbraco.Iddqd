using System;
using System.Diagnostics;
using Umbraco.Cms.Core.Semver;

namespace Limbo.Umbraco.Iddqd  {

    /// <summary>
    /// Static class with various information and constants about the package.
    /// </summary>
    public static class IddqdPackage {

        /// <summary>
        /// Gets the alias of the package.
        /// </summary>
        public const string Alias = "Limbo.Umbraco.Iddqd";

        /// <summary>
        /// Gets the friendly name of the package.
        /// </summary>
        public const string Name = "Limbo Iddqd";

        /// <summary>
        /// Gets the version of the package.
        /// </summary>
        public static readonly Version Version = typeof(IddqdPackage).Assembly.GetName().Version!;

        /// <summary>
        /// Gets the informational version of the package.
        /// </summary>
        public static readonly string InformationalVersion = FileVersionInfo.GetVersionInfo(typeof(IddqdPackage).Assembly.Location).ProductVersion!;

        /// <summary>
        /// Gets the semantic version of the package.
        /// </summary>
        public static readonly SemVersion SemVersion = InformationalVersion;

    }

}