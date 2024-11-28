using UnityEngine;
using UnityEngine.UI;

public static class HexColorConverter
{
    public static Color SetUIHexColor(string hexColor)
    {
        Color color;
        if (ColorUtility.TryParseHtmlString(hexColor, out color))
        {
            return color;
        }
        else
        {
            Debug.LogError("Invalid HEX Color Code");
            return color;
        }
    }
}