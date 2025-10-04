using System;
using UnityEngine;

public static class ManaColorUtility
{
    public static string NormalizeColor(string color)
    {
        if (string.IsNullOrEmpty(color))
            return "Colorless";

        return color switch
        {
            "Artifact" => "Colorless",
            "None" => "Colorless",
            _ => color
        };
    }

    public static string GetHexCode(string color)
    {
        return NormalizeColor(color) switch
        {
            "White" => "#FFD966",   // Yellow
            "Blue" => "#0096FF",    // Azure
            "Black" => "#8A2BE2",   // Purple
            "Red" => "#FF8C00",     // Orange
            "Green" => "#228B22",   // Green
            _ => "#B0B0B0"            // Grey for colorless
        };
    }

    public static string GetDisplayName(string color)
    {
        return NormalizeColor(color).ToLowerInvariant();
    }

    public static string FormatColoredManaNumber(int amount, string colorName)
    {
        string normalized = NormalizeColor(colorName);
        string hex = GetHexCode(normalized);
        string markHex = hex.Length == 7 ? hex + "FF" : hex;

        string textColorHex = "#000000";
        if (ColorUtility.TryParseHtmlString(hex, out Color color))
        {
            float luminance = 0.2126f * color.r + 0.7152f * color.g + 0.0722f * color.b;
            textColorHex = luminance < 0.5f ? "#FFFFFF" : "#000000";
        }

        return $"<mark={markHex}><color={textColorHex}>{amount}</color></mark>";
    }
}
