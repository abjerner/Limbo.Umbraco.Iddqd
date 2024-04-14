using System.Collections.Generic;
using Limbo.Umbraco.Iddqd.Models.Packages;
using Umbraco.Cms.Core.Manifest;

#pragma warning disable CS1591

namespace Limbo.Umbraco.Iddqd.Services;

public class IddqdService {

    private readonly IManifestParser _manifestParser;
    private readonly ManifestFilterCollection _manifestFilterCollection;

    public IddqdService(IManifestParser manifestParser, ManifestFilterCollection manifestFilterCollection) {
        _manifestParser = manifestParser;
        _manifestFilterCollection = manifestFilterCollection;
    }

    public virtual IReadOnlyList<IddqdPackageManifest> GetPackages() {

        List<IddqdPackageManifest> allManifests = new();

        foreach (IManifestFilter filter in _manifestFilterCollection) {

            List<PackageManifest> manifests = new();
            filter.Filter(manifests);

            foreach (PackageManifest manifest in manifests) {
                allManifests.Add(new IddqdPackageManifest(manifest, filter));
            }

        }

        foreach (var manifest in _manifestParser.GetManifests()) {
            if (string.IsNullOrWhiteSpace(manifest.Source)) continue;
            allManifests.Add(new IddqdPackageManifest(manifest));
        }

        return allManifests;

    }

}