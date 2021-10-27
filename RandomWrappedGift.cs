using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using Oxide.Core.Libraries.Covalence;
using UnityEngine;
using Random = System.Random;

namespace Oxide.Plugins
{
    [Info("Random Wrapped Gift", "Ryz0r", "2.0.3")]
    [Description("Enables players with permission to receive a randomly wrapped gift in a configured interval.")]
    public class RandomWrappedGift : RustPlugin
    {
        private const string GifteePerm = "randomwrappedgift.giftee";
        private const string GifterPerm = "randomwrappedgift.gifter";
        public string EffectToUse = "assets/prefabs/misc/easter/painted eggs/effects/gold_open.prefab";
        private Random random = new Random();
        Dictionary<ulong, long> PlayerLastReceiveTimes = new Dictionary<ulong, long>();
        
        #region Config/Lang
        private Configuration _config;
        protected override void SaveConfig() => Config.WriteObject(_config);
        protected override void LoadDefaultConfig() => _config = new Configuration();

        private class Configuration
        {
            [JsonProperty(PropertyName = "Gift Items (Item Shortname)", ObjectCreationHandling = ObjectCreationHandling.Replace)]
            public Dictionary<string, int> GiftItems = new Dictionary<string, int>
            {
                {"rifle.ak", 1},
                {"stones", 1500}
            };
            
            [JsonProperty(PropertyName = "Wrapped Gift Interval Permissions (Seconds)", ObjectCreationHandling = ObjectCreationHandling.Replace)]
            public Dictionary<string, long> IntervalPermissions = new Dictionary<string, long>
            {
                {"randomwrappedgift.time.default", 300},
                {"randomwrappedgift.time.vip", 90},
                {"randomwrappedgift.time.custom", 30}
            };

            [JsonProperty(PropertyName = "Play Effect When Opened?")]
            public bool EffectWhenOpened = true;
            
            [JsonProperty(PropertyName = "Give gift to sleepers with permissions?")]
            public bool GiftToSleepers = false;
        }

        protected override void LoadConfig()
        {
            base.LoadConfig();
            try
            {
                _config = Config.ReadObject<Configuration>();
                if (_config == null) throw new Exception();
                SaveConfig();
            }
            catch
            {
                PrintError("Your configuration file contains an error. Using default configuration values.");
                LoadDefaultConfig();
            }
        }
        
        protected override void LoadDefaultMessages()
        {
            lang.RegisterMessages(new Dictionary<string, string>
            {
                ["NoPerm"] = "You do not have permissions to use this command.",
                ["Given"] = "You have given players gifts. Yay!",
                ["Gifted"] = "You have received a randomly wrapped gift. This happens every {0} seconds for you. Enjoy!"
            }, this); 
        }
        #endregion
        #region Init Stuff
        private void Init()
        {
            AddCovalenceCommand("give", nameof(GiveGiftCommand));
            permission.RegisterPermission(GifteePerm, this);
            permission.RegisterPermission(GifterPerm, this);

            foreach (var kvp in _config.IntervalPermissions)
            {
                permission.RegisterPermission(kvp.Key, this);
            }
        }
        
        private void OnServerInitialized()
        {
            var pluginLoadTime = DateTimeOffset.UtcNow.ToUnixTimeSeconds();

            timer.Every(1, () =>
            {
                var currentTime = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
                var playerList = _config.GiftToSleepers ? BasePlayer.allPlayerList : BasePlayer.activePlayerList;
                
                foreach (var player in playerList)
                {
                    long lastReceivedTime;
                    if (!PlayerLastReceiveTimes.TryGetValue(player.userID, out lastReceivedTime))
                        lastReceivedTime = pluginLoadTime;

                    var playerCooldownSeconds = GetLower(player);
                    
                    if (lastReceivedTime + playerCooldownSeconds > currentTime) continue;
                    
                    CreateGift(player, playerCooldownSeconds);
                    PlayerLastReceiveTimes[player.userID] = currentTime;
                }
            });
        }
        #endregion
        #region Helper Functions

        private void CreateGift(BasePlayer bp, long refTime = 0)
        {
            var theItem = _config.GiftItems.ElementAt(random.Next(0, _config.GiftItems.Count));
            var createdItem = ItemManager.CreateByName(theItem.Key);

            var soonWrapped = ItemManager.CreateByItemID(204970153, 1);

            soonWrapped.contents.AddItem(createdItem.info, theItem.Value);
            if (bp.inventory.GiveItem(soonWrapped))
            {
                bp.ChatMessage(string.Format(lang.GetMessage("Gifted", this, bp.UserIDString), refTime));

                if (_config.EffectWhenOpened)
                {
                    EffectNetwork.Send(new Effect(EffectToUse, bp.GetNetworkPosition(), Vector3.zero),
                        bp.net.connection);
                }
            }
            else
            {
                soonWrapped.RemoveFromWorld();
                createdItem.RemoveFromWorld();
            }
        }

        private long GetLower(BasePlayer player)
        {
            var permList = permission.GetUserPermissions(player.UserIDString)
                .Where(perm => perm.StartsWith("randomwrappedgift.time"));
            var userTimes = permList.Select(p => _config.IntervalPermissions[p]).ToList();
            var returnValue = userTimes.Count < 1 ? -1 : userTimes.Min();
            return returnValue;
        }

        private void GiveGiftCommand(IPlayer player, string command, string[] args)
        {
            if (!permission.UserHasPermission(player.Id, GifterPerm))
            {
                player.Reply(lang.GetMessage("NoPerm", this, player.Id));
                return;
            }
            
            player.Reply(lang.GetMessage("Given", this, player.Id));
            CreateGift(BasePlayer.Find(player.Id));
        }
        #endregion
    }
}