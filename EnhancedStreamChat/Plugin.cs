using System;
using IPA;
using EnhancedStreamChat.Chat;
using IPA.Loader;
using IPAConfig = IPA.Config.Config;
using IPA.Config.Stores;
using IPALogger = IPA.Logging.Logger;

namespace EnhancedStreamChat
{
    [Plugin(RuntimeOptions.DynamicInit)]
    public class Plugin
    {
        internal static string Name => "EnhancedStreamChat";
        internal static string Version => _meta.HVersion.ToString();

        private static PluginMetadata _meta = null!;

        [Init]
        public void Init(IPAConfig conf, IPALogger logger, PluginMetadata meta)
        {
            _meta = meta;

            Logger.log = logger;
            Logger.log.Debug("Logger initialized.");

            ChatConfig.instance = conf.Generated<ChatConfig>();
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
