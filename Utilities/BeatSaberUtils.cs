﻿using BeatSaberMarkupLanguage;
using System.Linq;
using TMPro;
using UnityEngine;
using Object = UnityEngine.Object;

namespace EnhancedStreamChat.Utilities
{
    public static class BeatSaberUtils
    {
        private static Material _noGlow;
        public static Material UINoGlowMaterial
        {
            get
            {
                if (_noGlow == null)
                {
                    _noGlow = Resources.FindObjectsOfTypeAll<Material>().Where(m => m.name == "UINoGlow").FirstOrDefault();
                    if (_noGlow != null)
                    {
                        _noGlow = Object.Instantiate(_noGlow);
                    }
                }
                return _noGlow;
            }
        }

        private static Shader _tmpNoGlowFontShader;
        public static Shader TMPNoGlowFontShader
        {
            get
            {
                if(_tmpNoGlowFontShader == null)
                {
                    _tmpNoGlowFontShader = Resources.FindObjectsOfTypeAll<TMP_FontAsset>().Last(f2 => f2.name == "Teko-Medium SDF No Glow")?.material?.shader;
                }
                return _tmpNoGlowFontShader;
            }
        }

        // DaNike to the rescue 
        public static bool TryGetTMPFontByFamily(string family, out TMP_FontAsset font)
        {
            if(FontManager.TryGetTMPFontByFamily(family, out font))
            {
                font.material.shader = TMPNoGlowFontShader;
                return true;
            }
            return false;
        }
    }
}
