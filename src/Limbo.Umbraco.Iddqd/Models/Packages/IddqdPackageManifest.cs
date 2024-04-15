using System.Linq;
using System.Reflection;
using Limbo.Umbraco.Iddqd.Models.Assemblies;
using Newtonsoft.Json;
using Skybrud.Essentials.Reflection;
using Skybrud.Essentials.Strings;
using Umbraco.Cms.Core.Manifest;
using Umbraco.Extensions;

#pragma warning disable CS1591

namespace Limbo.Umbraco.Iddqd.Models.Packages;

public class IddqdPackageManifest {

    private readonly PackageManifest _manifest;

    [JsonProperty("packageId")]
    public string? PackageId { get; }

    [JsonProperty("packageName")]
    public string PackageName => _manifest.PackageName ?? string.Empty;

    [JsonProperty("version")]
    public string? Version { get; }

    [JsonProperty("assembly")]
    public IddqdAssembly? Assembly { get; }

    [JsonProperty("path")]
    public string? Path { get; }

    [JsonProperty("type")]
    public IddqdPackageType Type { get; }

    [JsonProperty("allowPackageTelemetry")]
    public bool AllowPackageTelemetry => _manifest.AllowPackageTelemetry;

    public IddqdPackageManifest(PackageManifest manifest, IManifestFilter filter) {

        Assembly assembly = filter.GetType().Assembly;

        _manifest = manifest;
        Assembly = new IddqdAssembly(assembly);
        PackageId = Assembly.Name;
        Type = IddqdPackageType.ManifestFilter;

        try {
            string? packageId = manifest.GetType().GetProperty("PackageId")?.GetValue(manifest) as string;
            if (!string.IsNullOrWhiteSpace(packageId)) PackageId = packageId;
        } catch {
            // We don't really care about the exception
        }

        Version = _manifest.Version;

        if (string.IsNullOrWhiteSpace(Version)) {
            Version = ReflectionUtils.GetInformationalVersion(assembly);
        }

    }

    public IddqdPackageManifest(PackageManifest manifest) {

        _manifest = manifest;

        Path = _manifest.Source;
        Type = IddqdPackageType.PackageManifest;
        Version = manifest.Version;

        // Get the package ID and/or assembly version from the package.manifest file
        string? packageId = manifest.PackageId;
        string? versionAssemblyName = manifest.VersionAssemblyName;

        // If the path is within the content root, we should only show the file's virtual path instead
        string[] path = Path.Split('/', '\\');
        int pos = path.IndexOf("App_Plugins");
        if (pos >= 0 && pos < path.Length - 1) {
            if (string.IsNullOrWhiteSpace(PackageId)) PackageId = path[pos + 1];
            Path = $"~/{string.Join("/", path.Skip(pos))}";
        }

        // Try to load the assembly if we have determined an assembly name
        try {
            string assemblyName = StringUtils.FirstWithValue(versionAssemblyName, packageId);
            if (string.IsNullOrWhiteSpace(assemblyName)) return;
            Assembly assembly = System.Reflection.Assembly.Load(assemblyName);
            if (string.IsNullOrWhiteSpace(Version)) Version = ReflectionUtils.GetInformationalVersion(assembly);
            Assembly = new IddqdAssembly(assembly);
        } catch {
            // ignore
        }


    }

    public bool IsMatch(string text) {
        if (PackageName.InvariantContains(text)) return true;
        if (PackageId is not null && PackageId.InvariantContains(text)) return true;
        if (Assembly?.Name is not null && Assembly.Name.InvariantContains(text)) return true;
        return false;
    }

}