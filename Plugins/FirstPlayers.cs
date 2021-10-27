using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Oxide.Core;
using Oxide.Core.Libraries.Covalence;

namespace Oxide.Plugins
{
    [Info("First Players", "Ryz0r", "0.1.1")]
    [Description("Log the first X amount of players to a data file.")]
    internal class FirstPlayers : CovalencePlugin
    {
        private static readonly List<string> _playersList = new List<string>();
        private Configuration _config;

        private void SaveData()
        {
            Interface.Oxide.DataFileSystem.WriteObject(Name, _playersList);
        }

        protected override void SaveConfig()
        {
            Config.WriteObject(_config);
        }

        protected override void LoadDefaultMessages()
        {
            lang.RegisterMessages(new Dictionary<string, string>
            {
                ["JoinMessage"] = "Congrats {0}! You are one of the first {1} users to connect!"
            }, this);
        }

        private void OnNewSave(string filename)
        {
            if (Interface.Oxide.DataFileSystem.ExistsDatafile(Name))
            {
                Interface.Oxide.DataFileSystem.GetFile(Name).Clear();
                Interface.Oxide.DataFileSystem.GetFile(Name).Save();

                Puts($"Wiped '{Name}.json'");
            }
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

        protected override void LoadDefaultConfig()
        {
            PrintWarning("A new configuration file is being generated.");
            _config = new Configuration();
        }

        private void OnUserConnected(IPlayer player)
        {
            if (_config.UserAmount == -1)
            {
                UserAdd(player.Id);
            }
            else
            {
                if (_playersList.Count == _config.UserAmount)
                {
                    Puts("Maximum amount of users reached.");
                    return;
                }

                if (_playersList.Contains(player.Id))
                    return;
                UserAdd(player.Id, player.Name);
            }
        }

        private void UserAdd(string playerId, string playerName = "")
        {
            _playersList.Add(playerId);
            SaveData();

            if (_config.UserAmount != -1)
                server.Broadcast(string.Format(lang.GetMessage("JoinMessage", this), playerName, _config.UserAmount));
        }

        private class Configuration
        {
            [JsonProperty(PropertyName = "UserAmount")]
            public readonly int UserAmount = 25;
        }
    }
}