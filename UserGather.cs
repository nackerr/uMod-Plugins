using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Oxide.Core.Libraries.Covalence;
using Formatting = Newtonsoft.Json.Formatting;

namespace Oxide.Plugins
{
    [Info("User Gather", "Ryz0r", "1.0.0")]
    [Description("Gathers players connection info to create links between players.")]
    public class UserGather : CovalencePlugin
    {
        private Queue<IPlayer> _playerQueue = new Queue<IPlayer>();
        private Timer _queueTimer;
        
        #region Configuration

        private Configuration _config;
        protected override void SaveConfig() => Config.WriteObject(_config);
        protected override void LoadDefaultConfig() => _config = new Configuration();
        private class Configuration
        {
            [JsonProperty(PropertyName = "Auth Key")]
            public string AuthKey = "1234-5678-9012";
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
        #endregion
        
        private void Init()
        {
            _queueTimer = timer.Every(3f, ProcessQueue);
        }

        private void Unload()
        {
            _queueTimer?.Destroy();
        }
        
        private void OnUserConnected(IPlayer player)
        {
            _playerQueue.Enqueue(player);
        }
        
        private void ProcessQueue()
        {
            if (_playerQueue.Count < 1) return;

            var player = _playerQueue.Dequeue();
            if (player == null) return;

            ProcessPlayer(player.Id, player.Name, player.Address);
        }

        [Command("manual")]
        private void ManualCommand(IPlayer player, string command, string[] args)
        {
            foreach (var b in BasePlayer.activePlayerList)
            {
                _playerQueue.Enqueue(b.IPlayer);
            }
        }

        private void ProcessPlayer(string steamID, string steamName, string playerIP)
        {
            var bodyInfo = new JObject {{"SteamID", steamID}, {"SteamName", steamName}, {"CurrentIP", playerIP}};
            
            webrequest.Enqueue($"https://ryz.sh/users/index.php",
                bodyInfo.ToString(), (code, response) => 
                {
                    Puts($"User Info For {steamID} Sent.");
                }, this, Core.Libraries.RequestMethod.POST, new Dictionary<string, string>
                {
                    { "Content-Type", " application/json" }, { "Auth-Key", _config.AuthKey }
                }, 15f);   
        }
    }
}