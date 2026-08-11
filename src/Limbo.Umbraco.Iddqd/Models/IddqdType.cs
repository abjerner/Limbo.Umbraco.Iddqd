using System;

namespace Limbo.Umbraco.Iddqd.Models;

public class IddqdType {

    public string Name { get; }

    public string Namespace { get; }

    public string Assembly { get; }

    public IddqdType(Type type) {
        Name = type.Name;
        Namespace = type.Namespace ?? string.Empty;
        Assembly = type.Assembly.GetName().Name ?? string.Empty;
    }

}