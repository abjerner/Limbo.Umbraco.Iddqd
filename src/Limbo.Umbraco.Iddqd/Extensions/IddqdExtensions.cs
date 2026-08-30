using System.Linq;
using System.Reflection;

namespace Limbo.Umbraco.Iddqd.Extensions;

public static class IddqdExtensions {

    internal static string? GetMetadata(this Assembly assembly, string key) {
        // This is here to avoid conflicting namespaces (System.Reflection vs Skybrud.Essentials.Reflection.Extensions)
        return assembly.GetCustomAttributes<AssemblyMetadataAttribute>().FirstOrDefault(x => x.Key == key)?.Value;
    }

}