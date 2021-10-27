using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using ProtoBuf;

namespace Oxide.Plugins
{
    [Info("Personal Kill Feed", "Ryz0r", "1.0.0")]
    [Description("A personal, and team kill feed in chat.")]
    public class PersonalKillFeed : RustPlugin
    {
        private Dictionary<string, string> _cachedWeapons = new Dictionary<string, string>();
        
        #region Config/Lang
        private Configuration _config;
        protected override void SaveConfig() => Config.WriteObject(_config);
        protected override void LoadDefaultConfig() => _config = new Configuration();
        private class Configuration
        {
            [JsonProperty(PropertyName = "Send To Teams On Death")]
            public bool SendToTeams = false;
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
                ["DeathMessage"] = "<color=green>{0}</color> was killed by <color=red>{1}</color> using an {2} at a distance of {3} Meters!",
                ["KillMessage"] = "<color=green>{0}</color> killed <color=red>{1}</color> using an {2} at a distance of {3} Meters!"
            }, this); 
        }
        #endregion
        
        private void OnServerInitialized() {
            foreach (var definition in ItemManager.itemList)
            {
                var entityMod = definition.GetComponent<ItemModEntity>();
                if (entityMod == null)
                    continue;

                var weapon = entityMod.entityPrefab.Get()?.GetComponent<AttackEntity>();
                if (weapon == null)
                    continue;

                _cachedWeapons[weapon.ShortPrefabName] = definition.displayName.english;
            }
        }
        
        private object OnPlayerDeath(BasePlayer player, HitInfo info)
        {
        	Puts(player.displayName);
            if (!info.InitiatorPlayer || !info.Weapon) return null;
            var weaponName = _cachedWeapons[info.Weapon.ShortPrefabName];
			Puts(weaponName);
            var plyMessage = string.Format(lang.GetMessage("DeathMessage", this), player.displayName,
                info.InitiatorPlayer.displayName, weaponName, Math.Round(info.ProjectileDistance));
            var killMessage = string.Format(lang.GetMessage("KillMessage", this), info.InitiatorPlayer.displayName,
                player.displayName, weaponName, Math.Round(info.ProjectileDistance));
            
            info.InitiatorPlayer.ChatMessage(killMessage);
            player.ChatMessage(plyMessage);

            if (_config.SendToTeams)
            {
                if (player.currentTeam == 0UL || info.InitiatorPlayer.currentTeam == 0UL) return null;

                RelationshipManager.PlayerTeam deathTeam = RelationshipManager.ServerInstance.teams[player.currentTeam];
                RelationshipManager.PlayerTeam killerTeam = RelationshipManager.ServerInstance.teams[info.InitiatorPlayer.currentTeam];

                foreach (var p in deathTeam.members)
                {
                    var bPlayer = BasePlayer.FindByID(p);
                    if (bPlayer != null && bPlayer.IsConnected && bPlayer.UserIDString != player.UserIDString) 
                    {
                        bPlayer.ChatMessage(killMessage);
                    }
                }
                
                foreach (var p in killerTeam.members)
                {
                    var bPlayer = BasePlayer.FindByID(p);
                    if (bPlayer != null && bPlayer.IsConnected && bPlayer.UserIDString != info.InitiatorPlayer.UserIDString) 
                    {
                        bPlayer.ChatMessage(killMessage);
                    }
                }
            }
            
            return null;
        }
    }
}