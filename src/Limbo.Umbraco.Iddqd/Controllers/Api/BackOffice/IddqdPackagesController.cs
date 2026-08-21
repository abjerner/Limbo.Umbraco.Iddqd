using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Asp.Versioning;
using Limbo.Umbraco.Iddqd.Api;
using Limbo.Umbraco.Iddqd.Models.Assemblies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Umbraco.Cms.Api.Common.Attributes;
using Umbraco.Cms.Api.Management.Routing;
using Umbraco.Cms.Core.Composing;
using Umbraco.Cms.Core.Manifest;
using Umbraco.Cms.Infrastructure.Manifest;
using Umbraco.Cms.Web.Common.Authorization;

namespace Limbo.Umbraco.Iddqd.Controllers.Api.BackOffice;

[ApiController]
[VersionedApiBackOfficeRoute($"{IddqdApiConstants.Route}/packages")]
[Authorize(Policy = AuthorizationPolicies.SectionAccessContent)]
[MapToApi(IddqdApiConstants.Alias)]
[ApiVersion("1.0")]
[ApiExplorerSettings(GroupName = "Packages")]
public class IddqdPackagesController : Controller {

    private readonly TypeLoader _typeLoader;
    private readonly IReadOnlyList<IPackageManifestReader> _manifestReaders;

    #region Constructors

    public IddqdPackagesController(TypeLoader typeLoader, IEnumerable<IPackageManifestReader> manifestReaders) {
        _typeLoader = typeLoader;
        _manifestReaders = manifestReaders as IPackageManifestReader[] ?? manifestReaders.ToArray();
    }

    #endregion

    #region Public API methods

    [HttpGet("manifests")]
    public async Task<ActionResult<object>> GetManifests() {

        // Assemblies may specify additional information, so we create a dictionary so we later can map them to
        // individual manifests. Keep in mind that Umbraco's type loader does not expose all assemblies in the
        // solution, but only those that interact directly with Umbraco (or something along those lines).
        Dictionary<string, Assembly> assemblies = _typeLoader.TypeFinder.AssembliesToScan
            .Where(x => x.FullName is not null)
            .ToDictionary(x => x.FullName!.Split(',')[0]);

        List<IddqdPackageManifest> manifests = [];

        foreach (var reader in _manifestReaders) {

            var result = await reader.ReadPackageManifestsAsync();

            foreach (PackageManifest manifest in result) {

                IddqdAssembly? assembly;
                if (assemblies.TryGetValue(manifest.Id ?? string.Empty, out Assembly? ass)) {
                    assembly = new IddqdAssembly(ass);
                } else if (assemblies.TryGetValue(manifest.Name, out ass)) {
                    assembly = new IddqdAssembly(ass);
                } else if (assemblies.TryGetValue(manifest.Name.Replace(" ", ""), out ass)) {
                    assembly = new IddqdAssembly(ass);
                } else if (reader.GetType().FullName is "Umbraco.Cms.Infrastructure.Manifest.AppPluginsPackageManifestReader") {
                    assembly = null;
                } else {
                    assembly = new IddqdAssembly(reader.GetType().Assembly);
                }

                manifests.Add(new IddqdPackageManifest(manifest.Id, manifest.Name, manifest.Version, false, assembly, manifest.AllowTelemetry));
            }

        }

        return Ok(manifests);

    }

    #endregion

}

public class IddqdPackageManifest {

    public string? Id { get; }

    public string Name { get; }

    public string? Version { get; }

    public bool AppPlugins { get; }

    public IddqdAssembly? Assembly { get; }

    public bool AllowTelemetry { get; }

    public IddqdPackageManifest(string? id, string name, string? version, bool appPlugins, IddqdAssembly? assembly, bool allowTelemetry) {
        Id = id;
        Name = name;
        Version = version;
        AppPlugins = appPlugins;
        Assembly = assembly;
        AllowTelemetry = allowTelemetry;
    }

}