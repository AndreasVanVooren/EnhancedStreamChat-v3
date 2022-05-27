﻿using UnityEngine;
using Color = UnityEngine.Color;

namespace EnhancedStreamChat.Utilities
{
    public static class ColorUtils
    {
        public static Color ToColor(this string colorString)
        {
            if (!ColorUtility.TryParseHtmlString(colorString, out var color)) {
                Logger.Warn($"BackgroundColor {colorString} is not a valid color.");
                color = Color.white;
            }

            return color;
        }
    }
}