using IPA.Config.Stores;
using IPA.Config.Stores.Attributes;
using IPA.Config.Stores.Converters;
using System;
using System.IO;
using System.Reflection;
using System.Runtime.CompilerServices;
using UnityEngine;

[assembly: InternalsVisibleTo(GeneratedStore.AssemblyVisibilityTarget)]
namespace EnhancedStreamChat.Chat
{
    public class ChatConfig
    {
        public static ChatConfig instance = null!;

        public virtual bool PreCacheAnimatedEmotes { get; set; } = true;

        public virtual string SystemFontName { get; set; } = "Segoe UI";
        public virtual Color BackgroundColor { get; set; } = Color.black.ColorWithAlpha(0.5f);
        public virtual Color TextColor { get; set; } = Color.white;
        public virtual Color AccentColor { get; set; } = new Color(0.57f, 0.28f, 1f, 1f);
        public virtual Color HighlightColor { get; set; } = new Color(0.57f, 0.28f, 1f, 0.06f);
        public virtual Color PingColor { get; set; } = new Color(1f, 0f, 0f, 0.13f);

        public virtual int ChatWidth { get; set; } = 120;
        public virtual int ChatHeight { get; set; } = 140;
        public virtual float FontSize { get; set; } = 3.4f;
        public virtual bool AllowMovement { get; set; } = false;
        public virtual bool SyncOrientation { get; set; } = false;
        public virtual bool ReverseChatOrder { get; set; } = false;

        public virtual Vector3 Menu_ChatPosition { get; set; } = new Vector3(0, 3.75f, 2.5f);
        public virtual Vector3 Menu_ChatRotation { get; set; } = new Vector3(325, 0, 0);

        public virtual Vector3 Song_ChatPosition { get; set; } = new Vector3(0, 3.75f, 2.5f);
        public virtual Vector3 Song_ChatRotation { get; set; } = new Vector3(325, 0, 0);
    }
}
