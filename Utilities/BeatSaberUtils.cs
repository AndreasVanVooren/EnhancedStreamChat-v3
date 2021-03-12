﻿using BeatSaberMarkupLanguage;
using System.Linq;
using TMPro;
using UnityEngine;

namespace EnhancedStreamChat.Utilities
{
    public static class BeatSaberUtils
	{
		private static Material? _noGlow;
		public static Material? UINoGlowMaterial => _noGlow ??= Resources.FindObjectsOfTypeAll<Material>().Where(m => m.name == "UINoGlow").FirstOrDefault();

		private static Shader? _tmpNoGlowFontShader;
		public static Shader? TMPNoGlowFontShader => BeatSaberUI.MainTextFont.material.shader;

		// DaNike to the rescue 
		public static bool TryGetTMPFontByFamily(string family, out TMP_FontAsset font)
		{
			if (FontManager.TryGetTMPFontByFamily(family, out font))
			{
				font.material.shader = TMPNoGlowFontShader;
				return true;
			}

			return false;
		}
	}
}