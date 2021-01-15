﻿using BeatSaberMarkupLanguage.Attributes;
using BeatSaberMarkupLanguage.Components.Settings;
using BeatSaberMarkupLanguage.Parser;
using BeatSaberMarkupLanguage.ViewControllers;
using System.Diagnostics;
using HMUI;
using UnityEngine;
using UnityEngine.UI;

namespace EnhancedStreamChat.Chat
{
    public partial class ChatDisplay : BSMLAutomaticViewController
    {

        [UIAction("#post-parse")]
        private void PostParse()
        {
            // bg
            _backgroundColorSetting.editButton.onClick.AddListener(HideSettings);
            _backgroundColorSetting.modalColorPicker.cancelEvent += ShowSettings;
            _backgroundColorSetting.CurrentColor = _chatConfig.BackgroundColor;
            // accent
            _accentColorSetting.editButton.onClick.AddListener(HideSettings);
            _accentColorSetting.modalColorPicker.cancelEvent += ShowSettings;
            _accentColorSetting.CurrentColor = _chatConfig.AccentColor;
            // highlight
            _highlightColorSetting.editButton.onClick.AddListener(HideSettings);
            _highlightColorSetting.modalColorPicker.cancelEvent += ShowSettings;
            _highlightColorSetting.CurrentColor = _chatConfig.HighlightColor;
            // ping
            _pingColorSetting.editButton.onClick.AddListener(HideSettings);
            _pingColorSetting.modalColorPicker.cancelEvent += ShowSettings;
            _pingColorSetting.CurrentColor = _chatConfig.PingColor;
            // text
            _textColorSetting.editButton.onClick.AddListener(HideSettings);
            _textColorSetting.modalColorPicker.cancelEvent += ShowSettings;
            _textColorSetting.CurrentColor = _chatConfig.TextColor;

            // Move interactables in front of the screen
            settingsModalGameObject.transform.localPosition = new Vector3(settingsModalGameObject.transform.localPosition.x, settingsModalGameObject.transform.localPosition.y, -2f);
            settingsIconGameObject.transform.localPosition = new Vector3(settingsIconGameObject.transform.localPosition.x, settingsIconGameObject.transform.localPosition.y, -2f);
        }

        [UIParams]
        internal BSMLParserParams parserParams;

        [UIObject("settings-icon")]
        internal GameObject settingsIconGameObject;

        [UIObject("settings-modal")]
        internal GameObject settingsModalGameObject;

        [UIComponent("background-color-setting")]
        ColorSetting _backgroundColorSetting;

        [UIComponent("accent-color-setting")]
        ColorSetting _accentColorSetting;

        [UIComponent("highlight-color-setting")]
        ColorSetting _highlightColorSetting;

        [UIComponent("ping-color-setting")]
        ColorSetting _pingColorSetting;

        [UIComponent("text-color-setting")]
        ColorSetting _textColorSetting;

        private Color _accentColor;
        [UIValue("accent-color")]
        public Color AccentColor
        {
            get => _chatConfig.AccentColor;
            set
            {
                _chatConfig.AccentColor = value;
                HMMainThreadDispatcher.instance.Enqueue(UpdateMessages());
                NotifyPropertyChanged();
            }
        }

        [UIValue("highlight-color")]
        public Color HighlightColor
        {
            get => _chatConfig.HighlightColor;
            set
            {
                _chatConfig.HighlightColor = value;
                HMMainThreadDispatcher.instance.Enqueue(UpdateMessages());
                NotifyPropertyChanged();
            }
        }

        [UIValue("ping-color")]
        public Color PingColor
        {
            get => _chatConfig.PingColor;
            set
            {
                _chatConfig.PingColor = value;
                HMMainThreadDispatcher.instance.Enqueue(UpdateMessages());
                NotifyPropertyChanged();
            }
        }

        [UIValue("background-color")]
        public Color BackgroundColor
        {
            get => _chatConfig.BackgroundColor;
            set
            {
                _chatConfig.BackgroundColor = value;
                _chatScreen.GetComponentInChildren<ImageView>().material.color = value;
                NotifyPropertyChanged();
            }
        }

        [UIValue("text-color")]
        public Color TextColor
        {
            get => _chatConfig.TextColor;
            set
            {
                _chatConfig.TextColor = value;
                HMMainThreadDispatcher.instance.Enqueue(UpdateMessages());
                NotifyPropertyChanged();
            }
        }

        [UIValue("font-size")]
        public float FontSize
        {
            get => _chatConfig.FontSize;
            set
            {
                _chatConfig.FontSize = value;
                HMMainThreadDispatcher.instance.Enqueue(UpdateMessages());
                NotifyPropertyChanged();
            }
        }

        private int _settingsWidth = 110;
        [UIValue("settings-width")]
        public int SettingsWidth
        {
            get => _settingsWidth;
            set
            {
                _settingsWidth = value;
                NotifyPropertyChanged();
            }
        }

        [UIValue("chat-width")]
        public int ChatWidth
        {
            get => _chatConfig.ChatWidth;
            set
            {
                _chatConfig.ChatWidth = value;
                _chatScreen.ScreenSize = new Vector2(ChatWidth, ChatHeight);
                _chatContainer.GetComponent<RectMask2D>().rectTransform.sizeDelta = new Vector2(ChatWidth, ChatHeight);
                HMMainThreadDispatcher.instance.Enqueue(UpdateMessages());
                NotifyPropertyChanged();
            }
        }

        [UIValue("chat-height")]
        public int ChatHeight
        {
            get => _chatConfig.ChatHeight;
            set
            {
                _chatConfig.ChatHeight = value;
                _chatScreen.ScreenSize = new Vector2(ChatWidth, ChatHeight);
                _chatContainer.GetComponent<RectMask2D>().rectTransform.sizeDelta = new Vector2(ChatWidth, ChatHeight);
                HMMainThreadDispatcher.instance.Enqueue(UpdateMessages());
                NotifyPropertyChanged();
            }
        }

        [UIValue("chat-position")]
        public Vector3 ChatPosition
        {
            get => _isInGame ? _chatConfig.Song_ChatPosition : _chatConfig.Menu_ChatPosition;
            set
            {
                if (_isInGame || SyncOrientation)
                {
                    _chatConfig.Song_ChatPosition = value;
                }

                if (!_isInGame || SyncOrientation)
                {
                    _chatConfig.Menu_ChatPosition = value;
                }

                _chatScreen.ScreenPosition = value;
                NotifyPropertyChanged();
            }
        }

        [UIValue("chat-rotation")]
        public Vector3 ChatRotation
        {
            get => _isInGame ? _chatConfig.Song_ChatRotation : _chatConfig.Menu_ChatRotation;
            set
            {
                if (_isInGame || SyncOrientation)
                {
                    _chatConfig.Song_ChatRotation = value;
                }

                if (!_isInGame || SyncOrientation)
                {
                    _chatConfig.Menu_ChatRotation = value;
                }

                _chatScreen.ScreenRotation = Quaternion.Euler(value);
                NotifyPropertyChanged();
            }
        }

        [UIValue("allow-movement")]
        public bool AllowMovement
        {
            get => _chatConfig.AllowMovement;
            set
            {
                _chatConfig.AllowMovement = value;
                _chatScreen.ShowHandle = value;
                NotifyPropertyChanged();
            }
        }

        [UIValue("sync-orientation")]
        public bool SyncOrientation
        {
            get => _chatConfig.SyncOrientation;
            set
            {
                _chatConfig.SyncOrientation = value;

                if (value)
                {
                    ChatPosition = ChatPosition;
                    ChatRotation = ChatRotation;
                }

                NotifyPropertyChanged();
            }
        }

        [UIValue("reverse-chat-order")]
        public bool ReverseChatOrder
        {
            get => _chatConfig.ReverseChatOrder;
            set
            {
                _chatConfig.ReverseChatOrder = value;
                HMMainThreadDispatcher.instance.Enqueue(UpdateMessages());
                NotifyPropertyChanged();
            }
        }

        [UIValue("mod-version")]
        public string ModVersion
        {
            get => Plugin.Version;
        }

        [UIAction("launch-web-app")]
        private void LaunchWebApp()
        {
            ChatManager.instance._sc.LaunchWebApp();
        }

        [UIAction("launch-kofi")]
        private void LaunchKofi()
        {
            Application.OpenURL("https://ko-fi.com/brian91292");
        }

        [UIAction("launch-github")]
        private void LaunchGitHub()
        {
            Application.OpenURL("https://github.com/Auros/EnhancedStreamChat-v3");
        }

        [UIAction("on-settings-clicked")]
        private void OnSettingsClick()
        {
            Logger.log.Info("Settings clicked!");
        }

        [UIAction("#hide-settings")]
        private void OnHideSettings()
        {
            Logger.log.Info("Saving settings!");
            _chatConfig.Save();
        }

        private void HideSettings()
        {
            parserParams.EmitEvent("hide-settings");
        }

        private void ShowSettings()
        {
            parserParams.EmitEvent("show-settings");
        }
    }
}
