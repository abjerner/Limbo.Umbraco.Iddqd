using System.Collections.Generic;
using Umbraco.Cms.Core.Manifest;

namespace Limbo.Umbraco.Iddqd;

/// <inheritdoc />
public class IddqdManifestFilter : IManifestFilter {

    /// <inheritdoc />
    public void Filter(List<PackageManifest> manifests) {
        manifests.Add(new PackageManifest {
            AllowPackageTelemetry = true,
            PackageName = IddqdPackage.Name,
            Version = IddqdPackage.InformationalVersion,
            Scripts = new[] {
                $"/App_Plugins/{IddqdPackage.Alias}/Scripts/App.js",
                $"/App_Plugins/{IddqdPackage.Alias}/Scripts/Directives/Stairs.js",
                $"/App_Plugins/{IddqdPackage.Alias}/Scripts/Controllers/Overlays/Package.js",
                $"/App_Plugins/{IddqdPackage.Alias}/Scripts/Controllers/Overlays/PropertyEditor.js",
                $"/App_Plugins/{IddqdPackage.Alias}/Scripts/Controllers/Overlays/PropertyEditorDataTypes.js",
                $"/App_Plugins/{IddqdPackage.Alias}/Scripts/Controllers/ContentApp.js",
                $"/App_Plugins/{IddqdPackage.Alias}/Scripts/Controllers/Examine.js",
                $"/App_Plugins/{IddqdPackage.Alias}/Scripts/Controllers/Packages.js",
                $"/App_Plugins/{IddqdPackage.Alias}/Scripts/Controllers/PropertyEditors.js"
            },
            Stylesheets = new[] {
                $"/App_Plugins/{IddqdPackage.Alias}/Styles/Styles.css"
            },
        });
    }

}