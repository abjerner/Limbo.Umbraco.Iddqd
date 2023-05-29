using System.Linq;
using System.Reflection;
using Newtonsoft.Json;
using Skybrud.Essentials.Reflection;

#pragma warning disable CS1591

namespace Limbo.Umbraco.Iddqd.Models {

    public class PropertyEditorAssembly {

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

        [JsonProperty("repositoryUrl")]
        public string? RepositoryUrl { get; }

        [JsonProperty("marketplaceUrl")]
        public string? MarketplaceUrl { get; }

        public PropertyEditorAssembly(Assembly assembly) {
            Name = assembly.FullName?.Split(',')[0];
            Copyright = assembly.GetCustomAttribute<AssemblyCopyrightAttribute>()?.Copyright;
            Title = assembly.GetCustomAttribute<AssemblyTitleAttribute>()?.Title;
            Version = ReflectionUtils.GetInformationalVersion(assembly);
            Description = assembly.GetCustomAttribute<AssemblyDescriptionAttribute>()?.Description;
            FileVersion = assembly.GetCustomAttribute<AssemblyFileVersionAttribute>()?.Version;
            Configuration = assembly.GetCustomAttribute<AssemblyConfigurationAttribute>()?.Configuration;
            Company = assembly.GetCustomAttribute<AssemblyCompanyAttribute>()?.Company;
            Product = assembly.GetCustomAttribute<AssemblyProductAttribute>()?.Product;
            RepositoryUrl = assembly.GetCustomAttributes<AssemblyMetadataAttribute>().FirstOrDefault(x => x.Key == "RepositoryUrl")?.Value;
            MarketplaceUrl = assembly.GetCustomAttributes<AssemblyMetadataAttribute>().FirstOrDefault(x => x.Key == "UmbracoMarketplaceUrl")?.Value;
        }

    }

}