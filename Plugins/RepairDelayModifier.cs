using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace Oxide.Plugins
{
    [Info("Repair Delay Modifier", "Ryz0r", "1.0.1")]
    [Description("Takes away the repair delay, or increases/decreases it for players with permission.")]
    public class RepairDelayModifier : RustPlugin
    {
        private const string NoDelayPerm = "norepairdelay.perm";
        private const string TimeDelayPerm = "norepairdelay.timed";
        private Configuration _config;

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
                ["RepairInstant"] = "Congratulations, your structure has been instantly repaired.",
                ["RepairTimed"] = "You have been upgraded before the 30 second timer.",
                ["RepairTimedWait"] =
                    "It has only been {timeSince} seconds since damage, and you must wait {timeTill} seconds."
            }, this);
        }

        protected override void LoadDefaultConfig()
        {
            PrintWarning("A new configuration file is being generated.");
            _config = new Configuration();
        }

        protected override void SaveConfig()
        {
            Config.WriteObject(_config);
        }

        private void Init()
        {
            permission.RegisterPermission(TimeDelayPerm, this);
            permission.RegisterPermission(NoDelayPerm, this);
        }

        private object OnStructureRepair(BaseCombatEntity entity, BasePlayer player)
        {
            if (permission.UserHasPermission(player.UserIDString, NoDelayPerm) &&
                entity.SecondsSinceAttacked >= 0)
            {
                entity.lastAttackedTime = float.MinValue;
                if (_config.EnableChatAlerts == false) return null;

                player.ChatMessage(lang.GetMessage("RepairInstant", this, player.UserIDString));
                return null;
            }

            if (permission.UserHasPermission(player.UserIDString, TimeDelayPerm) &&
                entity.SecondsSinceAttacked >= _config.RepairTime)
            {
                entity.lastAttackedTime = float.MinValue;
                if (_config.EnableChatAlerts)
                {
                    player.ChatMessage(lang.GetMessage("RepairTimed", this, player.UserIDString));
                    return true;
                }

                return null;
            }

            if (permission.UserHasPermission(player.UserIDString, TimeDelayPerm) &&
                entity.SecondsSinceAttacked <= _config.RepairTime)
            {
                if (_config.EnableChatAlerts == false)
                {
                    return true;
                }

                player.ChatMessage(string.Format(lang.GetMessage("RepairTimedWait", this, player.UserIDString),
                    Math.Round(entity.SecondsSinceAttacked), _config.RepairTime - entity.SecondsSinceAttacked));
                return true;
            }

            return null;
        }

        private class Configuration
        {
            [JsonProperty(PropertyName = "EnableChatAlerts")]
            public readonly bool EnableChatAlerts = true;

            [JsonProperty(PropertyName = "RepairTime")]
            public readonly float RepairTime = 20f;
        }
    }
}