using System.Globalization;

namespace Limbo.Umbraco.Iddqd;

public static class IddqdUtils {

    private static readonly IdnMapping _idn = new();

    public static string FromPunycode(string value) {

        // TODO: consider moving to Skybrud.Essentials

        if (string.IsNullOrWhiteSpace(value)) return value;

        string[] parts = value.Split('.');
        for (int i = 0; i < parts.Length; i++) {
            parts[i] = _idn.GetUnicode(parts[i]);
        }

        return string.Join('.', parts);

    }

    public static string ToPunycode(string value) {

        // TODO: consider moving to Skybrud.Essentials

        if (string.IsNullOrWhiteSpace(value)) return value;

        string[] parts = value.Split('.');
        for (int i = 0; i < parts.Length; i++) {
            parts[i] = _idn.GetAscii(parts[i]);
        }

        return string.Join('.', parts);
    }

}
