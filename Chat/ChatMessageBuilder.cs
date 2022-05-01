﻿using CatCore.Models.Twitch.IRC;
using CatCore.Models.Twitch.Media;
using EnhancedStreamChat.Graphics;
using EnhancedStreamChat.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace EnhancedStreamChat.Chat
{
    public class ChatMessageBuilder
    {
        private readonly ChatImageProvider _chatImageProvider;

        public ChatMessageBuilder(ChatImageProvider chatImageProvider)
        {
            this._chatImageProvider = chatImageProvider;
        }


        /// <summary>
        /// This function *blocks* the calling thread, and caches all the images required to display the message, then registers them with the provided font.
        /// </summary>
        /// <param name="msg">The chat message to get images from</param>
        /// <param name="font">The font to register these images to</param>
        public async Task<bool> PrepareImages(IESCChatMessage msg, EnhancedFontInfo font)
        {
            var tasks = new List<Task<EnhancedImageInfo>>();
            var pendingEmoteDownloads = new HashSet<string>();

            foreach (var emote in msg.Emotes.OfType<TwitchEmote>()) {
                if (string.IsNullOrEmpty(emote.Id) || pendingEmoteDownloads.Contains(emote.Id)) {
                    continue;
                }
                if (!font.CharacterLookupTable.ContainsKey(emote.Id)) {
                    pendingEmoteDownloads.Add(emote.Id);
                    var tcs = new TaskCompletionSource<EnhancedImageInfo>();
                    SharedCoroutineStarter.instance.StartCoroutine(this._chatImageProvider.TryCacheSingleImage(emote.Id, emote.Url, emote.Animated, (info) =>
                    {
                        if (info != null) {
                            if (!font.TryRegisterImageInfo(info, out var character)) {
                                Logger.Warn($"Failed to register emote \"{emote.Id}\" in font {font.Font.name}.");
                            }
                        }
                        tcs.SetResult(info);
                    }, forcedHeight: 110));
                    tasks.Add(tcs.Task);
                }
            }

            if (msg.Sender is TwitchUser twitchUser) {
                foreach (var badge in twitchUser.Badges) {
                    if (string.IsNullOrEmpty(badge.Id) || pendingEmoteDownloads.Contains(badge.Id)) {
                        continue;
                    }
                    if (!font.CharacterLookupTable.ContainsKey(badge.Id)) {
                        pendingEmoteDownloads.Add(badge.Id);
                        var tcs = new TaskCompletionSource<EnhancedImageInfo>();
                        SharedCoroutineStarter.instance.StartCoroutine(this._chatImageProvider.TryCacheSingleImage(badge.Id, badge.Uri, false, (info) =>
                        {
                            if (info != null) {
                                if (!font.TryRegisterImageInfo(info, out var character)) {
                                    Logger.Warn($"Failed to register badge \"{badge.Id}\" in font {font.Font.name}.");
                                }
                            }
                            tcs.SetResult(info);
                        }, forcedHeight: 100));
                        tasks.Add(tcs.Task);
                    }
                }
            }
            // Wait on all the resources to be ready
            var result = await Task.WhenAll(tasks);
            return result.All(x => x != null);
        }

        public Task<string> BuildMessage(IESCChatMessage msg, EnhancedFontInfo font)
        {
            return Task.Run(async () =>
            {
                try {
                    if (!await this.PrepareImages(msg, font)) {
                        Logger.Warn($"Failed to prepare some/all images for msg \"{msg.Message}\"!");
                        //return msg.Message;
                    }
                    var badges = new Stack<EnhancedImageInfo>();
                    if (msg.Sender is TwitchUser twitchUser) {
                        foreach (var badge in twitchUser.Badges) {
                            if (!this._chatImageProvider.CachedImageInfo.TryGetValue(badge.Id, out var badgeInfo)) {
                                Logger.Warn($"Failed to find cached image info for badge \"{badge.Id}\"!");
                                continue;
                            }
                            badges.Push(badgeInfo);
                        }
                    }
                    var sb = new StringBuilder(msg.Message); // Replace all instances of < with a zero-width non-breaking character
                    foreach (var emote in msg.Emotes) {
                        if (!this._chatImageProvider.CachedImageInfo.TryGetValue(emote.Id, out var replace)) {
                            Logger.Warn($"Emote {emote.Name} was missing from the emote dict! The request to {emote.Url} may have timed out?");
                            continue;
                        }
                        //Logger.Info($"replase id {replace.ImageId}");
                        //Logger.Info($"Emote: {emote.Name}, StartIndex: {emote.StartIndex}, EndIndex: {emote.EndIndex}, Len: {sb.Length}");
                        if (!font.TryGetCharacter(replace.ImageId, out var character)) {
                            Logger.Warn($"Emote {emote.Name} was missing from the character dict! Font hay have run out of usable characters.");
                            continue;
                        }
                        //Logger.Info($"target char {character}");
                        try {
                            // Replace emotes by index, in reverse order (msg.Emotes is sorted by emote.StartIndex in descending order)
                            sb.Replace(emote.Name, emote switch
                            {
                                TwitchEmote t when t.Bits > 0 => $"{char.ConvertFromUtf32((int)character)}\u00A0<color={t.Color}><size=77%><b>{t.Bits}\u00A0</b></size></color>",
                                _ => char.ConvertFromUtf32((int)character)
                            });
                        }
                        catch (Exception ex) {
                            Logger.Error($"An unknown error occurred while trying to swap emote {emote.Name} into string of length {sb.Length} at location ({emote.StartIndex}, {emote.EndIndex})\r\n{ex}");
                        }
                    }
                    // Escape all html tags in the message
                    sb.Replace("<", "<\u2060");

                    if (msg.IsSystemMessage) {
                        // System messages get a grayish color to differenciate them from normal messages in chat, and do not receive a username/badge prefix
                        sb.Insert(0, $"<color=#bbbbbbbb>");
                        sb.Append("</color>");
                    }
                    else {
                        var nameColorCode = msg.Sender.Color;
                        if (ColorUtility.TryParseHtmlString(msg.Sender.Color.Substring(0, 7), out var nameColor)) {
                            Color.RGBToHSV(nameColor, out var h, out var s, out var v);
                            if (v < 0.85f) {
                                v = 0.85f;
                                nameColor = Color.HSVToRGB(h, s, v);
                            }
                            nameColorCode = ColorUtility.ToHtmlStringRGBA(nameColor);
                            nameColorCode = nameColorCode.Insert(0, "#");
                        }
                        if (msg.IsActionMessage) {
                            // Message becomes the color of their name if it's an action message
                            sb.Insert(0, $"<color={nameColorCode}><b>{msg.Sender.DisplayName}</b> ");
                            sb.Append("</color>");
                        }
                        else {
                            // Insert username w/ color
                            sb.Insert(0, $"<color={nameColorCode}><b>{msg.Sender.DisplayName}</b></color>: ");
                        }
                        var parsedBadge = new HashSet<string>();
                        if (msg.Sender is TwitchUser twitchUser1) {
                            for (var i = 0; i < twitchUser1.Badges.Count; i++) {
                                // Insert user badges at the beginning of the string in reverse order
                                var badge = badges.Pop();
                                if (parsedBadge.Contains(badge.ImageId)) {
                                    continue;
                                }
                                parsedBadge.Add(badge.ImageId);
                                if (badge != null && font.TryGetCharacter(badge.ImageId, out var character)) {
                                    sb.Insert(0, $"{char.ConvertFromUtf32((int)character)} ");
                                }
                                else {
                                    Logger.Warn("Undefind badge");
                                }
                            }
                        }
                    }
                    return sb.ToString();
                }
                catch (Exception ex) {
                    Logger.Error($"An exception occurred in ChatMessageBuilder while parsing msg with {msg.Emotes.Count} emotes. Msg: \"{msg.Message}\". {ex}");
                }
                return msg.Message;
            });
        }
    }
}
