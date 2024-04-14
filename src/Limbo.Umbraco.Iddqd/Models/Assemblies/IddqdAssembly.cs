using System.Linq;
using System.Reflection;
using Newtonsoft.Json;
using Skybrud.Essentials.Reflection;
using Umbraco.Extensions;

#pragma warning disable CS1591

namespace Limbo.Umbraco.Iddqd.Models.Assemblies;

public class IddqdAssembly {

    [JsonProperty("name")]
    public string? Name { get; }

    [JsonProperty("copyright")]
    public string? Copyright { get; }

    [JsonProperty("title")]
    public string? Title { get; }

    [JsonProperty("description")]
    public string? Description { get; }

    [JsonProperty("version")]
    public string? Version { get; }

    [JsonProperty("fileVersion")]
    public string? FileVersion { get; }

    [JsonProperty("configuration")]
    public string? Configuration { get; }

    [JsonProperty("company")]
    public string? Company { get; }

    [JsonProperty("product")]
    public string? Product { get; }

    [JsonProperty("packageProjectUrl")]
    public string? PackageProjectUrl { get; }

    [JsonProperty("documentationUrl")]
    public string? DocumentationUrl { get; }

    [JsonProperty("repositoryUrl")]
    public string? RepositoryUrl { get; }

    [JsonProperty("marketplaceUrl")]
    public string? MarketplaceUrl { get; }

    [JsonProperty("nuGetUrl")]
    public string? NuGetUrl { get; }

    public IddqdAssembly(Assembly assembly) {
        Name = assembly.FullName?.Split(',')[0];
        Copyright = assembly.GetCustomAttribute<AssemblyCopyrightAttribute>()?.Copyright;
        Title = assembly.GetCustomAttribute<AssemblyTitleAttribute>()?.Title;
        Version = ReflectionUtils.GetInformationalVersion(assembly);
        Description = assembly.GetCustomAttribute<AssemblyDescriptionAttribute>()?.Description;
        FileVersion = assembly.GetCustomAttribute<AssemblyFileVersionAttribute>()?.Version;
        Configuration = assembly.GetCustomAttribute<AssemblyConfigurationAttribute>()?.Configuration;
        Company = assembly.GetCustomAttribute<AssemblyCompanyAttribute>()?.Company;
        Product = assembly.GetCustomAttribute<AssemblyProductAttribute>()?.Product;
        PackageProjectUrl = GetMetadataValue(assembly, "PackageProjectUrl");
        DocumentationUrl = GetMetadataValue(assembly, "DocumentationUrl");
        RepositoryUrl = GetMetadataValue(assembly, "RepositoryUrl");
        MarketplaceUrl = GetMetadataValue(assembly, "UmbracoMarketplaceUrl");
        NuGetUrl = GetMetadataValue(assembly, "NuGetUrl");
    }

    private static string? GetMetadataValue(Assembly assembly, string key) {
        return assembly.GetCustomAttributes<AssemblyMetadataAttribute>().FirstOrDefault(x => x.Key.InvariantEquals(key))?.Value;
    }

}