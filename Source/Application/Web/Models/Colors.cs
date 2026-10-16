using System.Globalization;

namespace Web.Models;

/// <summary>
/// Couleurs du modèle (RGBColor) : 8 chiffres hexadécimaux RRGGBBAA, comme le CSS #RRGGBBAA (FFFFFFFF = blanc opaque).
/// Le champ de formulaire &lt;input type="color"&gt; ne connaît que #rrggbb : la transparence est alors opaque (FF).
/// </summary>
public static class Colors
{
    /// <summary>Couleur CSS d'une valeur du modèle, ou <paramref name="fallback"/> si elle est absente ou invalide.</summary>
    public static string Css(string? rgbColor, string fallback)
        => IsHex(rgbColor, 8) || IsHex(rgbColor, 6) ? "#" + rgbColor : fallback;

    /// <summary>Valeur pour un champ &lt;input type="color"&gt; (#rrggbb).</summary>
    public static string ToInput(string? rgbColor)
        => IsHex(rgbColor, 8) || IsHex(rgbColor, 6) ? "#" + rgbColor![..6].ToLowerInvariant() : "#ffffff";

    /// <summary>Valeur du modèle (RRGGBBFF) d'une saisie #rrggbb ; blanc si la saisie est invalide.</summary>
    public static string FromInput(string? input)
    {
        var hex = input?.TrimStart('#');
        return IsHex(hex, 6) ? hex!.ToUpperInvariant() + "FF" : "FFFFFFFF";
    }

    /// <summary>Couleur de texte lisible (noir ou blanc) sur un fond donné.</summary>
    public static string TextOn(string? rgbColor)
    {
        if (!IsHex(rgbColor, 8) && !IsHex(rgbColor, 6))
        {
            return "#1d1e20";
        }

        var r = int.Parse(rgbColor![..2], NumberStyles.HexNumber);
        var g = int.Parse(rgbColor[2..4], NumberStyles.HexNumber);
        var b = int.Parse(rgbColor[4..6], NumberStyles.HexNumber);
        return (0.299 * r + 0.587 * g + 0.114 * b) > 150 ? "#1d1e20" : "#ffffff";
    }

    private static bool IsHex(string? value, int length)
        => value is not null && value.Length == length && value.All(Uri.IsHexDigit);
}
