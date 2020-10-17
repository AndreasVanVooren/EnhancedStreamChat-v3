﻿using System;
using IPA;
using IPALogger = IPA.Logging.Logger;
using EnhancedStreamChat.Chat;
using IPA.Loader;
using System.Reflection;

namespace EnhancedStreamChat
{
    [Plugin(RuntimeOptions.DynamicInit)]
    public class Plugin
    {
        internal static Plugin instance { get; private set; }
        internal static string Name => "EnhancedStreamChat";
        internal static string Version => _meta.Version.ToString() ?? Assembly.GetExecutingAssembly().GetName().Version.ToString();

        private static PluginMetadata _meta;

        [Init]
        public void Init(IPALogger logger, PluginMetadata meta)
        {
            instance = this;
            _meta = meta;
            Logger.log = logger;
            Logger.log.Debug("Logger initialized.");
            var config = ChatConfig.instance;
        }

        [OnEnable]
        public void OnEnable()
        {
            try
            {
                ChatManager.instance.enabled = true;
            }
            catch(Exception ex)
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
