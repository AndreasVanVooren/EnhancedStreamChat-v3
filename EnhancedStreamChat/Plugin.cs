using System;
using IPA;
using IPALogger = IPA.Logging.Logger;
using EnhancedStreamChat.Chat;
using IPA.Loader;

namespace EnhancedStreamChat
{
    [Plugin(RuntimeOptions.DynamicInit)]
    public class Plugin
    {
        internal static string Name => "EnhancedStreamChat";
        internal static string Version => _meta.HVersion.ToString();

        private static PluginMetadata _meta = null!;

        [Init]
        public void Init(IPALogger logger, PluginMetadata meta)
        {
            _meta = meta;

            Logger.log = logger;
            Logger.log.Debug("Logger initialized.");

            var _ = ChatConfig.instance;
        }

        [OnEnable]
        public void OnEnable()
        {
            try
            {
                ChatManager.instance.enabled = true;
            }
            catch (Exception ex)
            {
                Logger.log.Error(ex);
            }
        }

        [OnDisable]
        public void OnDisable()
        {
            ChatManager.instance.enabled = false;
        }
    }
}
