using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Newtonsoft.Json;
using Oxide.Core;
using Oxide.Core.Libraries;
using Oxide.Core.Libraries.Covalence;
using Steamworks;

namespace Oxide.Plugins
{
    [Info("Steam Server Disconnect Notify", "Ryz0r", "1.0.0")]
    [Description("Notify when Steam Server Disconnect happens.")]
    public class SteamServerDisconnectNotify : RustPlugin
    {
        private PluginData _data;
        private void SaveData() => Interface.Oxide.DataFileSystem.WriteObject(Name, _data);
        
        #region Data Stuff
        private void OnNewSave()
        {
            _data = new PluginData();
            SaveData();

            Interface.Oxide.UnloadPlugin(Name);
            timer.Once(900f, () =>
            {
                Interface.Oxide.LoadPlugin(Name);
            });
        }

        private void LoadData()
        {
            try
            {
                _data = Interface.Oxide.DataFileSystem.ReadObject<PluginData>(Name);
            }
            catch (Exception e)
            {
                PrintError(e.ToString());
            }

            if (_data == null) _data = new PluginData();
        }

        private class PluginData
        {
            [JsonProperty(PropertyName = "Disconnect List")]
            public HashSet<string> DisconnectList = new HashSet<string>();
        }

        #endregion
        #region Load/Save Data
        private void OnServerSave()
        {;
            SaveData();
        }

        private void Loaded()
        {
            LoadData();
        }
        #endregion
        #region Configuration
        private Configuration _config;
        private class Configuration
        {
            [JsonProperty(PropertyName = "Discord Webhook URL")]
            public string WebhookURL = "https://support.discord.com/hc/en-us/articles/228383668-Intro-to-Webhooks";
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


        protected override void SaveConfig()
        {
            Config.WriteObject(_config);
        }

        #endregion
        #region Functions
        private void SendDiscordMessage(string content, string desc)
        {
            var embed = new Embed()
                .SetColor("#00FFFF")
                .SetDescription(desc);
            
            var headers = new Dictionary<string, string>() {{"Content-Type", "application/json"}};
            const float timeout = 500f;
            
            webrequest.Enqueue(_config.WebhookURL, new DiscordMessage(content, embed).ToJson(),  GetCallback, this,
                RequestMethod.POST, headers, timeout);
        }
        
        private void GetCallback(int code, string response)
        {
            if (response != null && code == 204) return;
            
            Puts($"Error: {code} - Couldn't get an answer from server.");
        }
        #endregion
        #region Discord Stuff
        private class DiscordMessage
        {
            public DiscordMessage(string content, params Embed[] embeds)
            {
                Content = content;
                Embeds  = embeds.ToList();
            }

            [JsonProperty("content")] public string Content { get; set; }
            [JsonProperty("embeds")] public List<Embed> Embeds { get; set; }
            

            public string ToJson() => JsonConvert.SerializeObject(this);
        }

        private class Embed
        {
            [JsonProperty("fields")] public List<Field> Fields { get; set; } = new List<Field>();
            [JsonProperty("color")] public int Color { get; set; }
            [JsonProperty("description")] public string Description { get; set; }

            public Embed AddField(string name, string value, bool inline)
            {
                Fields.Add(new Field(name, Regex.Replace(value, "<.*?>", string.Empty), inline));

                return this;
            }
            
            public Embed SetColor(string color)
            {
                var replace = color.Replace("#", "");
                var decValue = int.Parse(replace, System.Globalization.NumberStyles.HexNumber);
                Color = decValue;
                return this;
            }
            
            public Embed SetDescription(string content)
            {
                Description = content;
                return this;
            }
        }

        private class Field
        {
            public Field(string name, string value, bool inline)
            {
                Name = name;
                Value = value;
                Inline = inline;
            }

            [JsonProperty("name")] public string Name { get; set; }
            [JsonProperty("value")] public string Value { get; set; }
            [JsonProperty("inline")] public bool Inline { get; set; }
        }
        #endregion
        
        private void Init()
        {
            SteamServer.OnSteamServersDisconnected += NotifyFunction;
        }

        private void NotifyFunction(Result result)
        {
            var timeUtc = DateTime.UtcNow;
            var timeEst = timeUtc.Subtract(TimeSpan.FromHours(4));
            var timeEstNew = timeEst.ToString("ddd, dd MMM yyy hh':'mm':'ss EST");

            _data.DisconnectList.Add(timeEstNew);
            SendDiscordMessage("<@&856610847517966366>", $"Steam Server Disconnect has happened.\nCurrent Time: {timeEstNew}\nServer IP: {covalence.Server.Address}:{covalence.Server.Port}");
            SaveData();
        }
        
        [ConsoleCommand("ssdn.test")]
        private void TestWebookCommand(ConsoleSystem.Arg arg)
        {
            var timeUtc = DateTime.UtcNow;
            var timeEst = timeUtc.Subtract(TimeSpan.FromHours(4));
            var timeEstNew = timeEst.ToString("ddd, dd MMM yyy hh':'mm':'ss EST");

            _data.DisconnectList.Add(timeEstNew);
            SendDiscordMessage("<@&856610847517966366>", $"Steam Server Disconnect has happened.\nCurrent Time: {timeEstNew}\nServer IP: {covalence.Server.Address}:{covalence.Server.Port}");
            SaveData();
        }
        
        [ConsoleCommand("ssdn.clear")]
        private void TestCommand(ConsoleSystem.Arg arg)
        {
            _data.DisconnectList.Clear();
            SaveData();
        }
    }
}