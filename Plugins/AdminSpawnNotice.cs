using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Newtonsoft.Json;
using Oxide.Core.Libraries;

namespace Oxide.Plugins
{
    [Info("Admin Spawn Notice", "Ryz0r", "1.0.2")]
    [Description("Sends a notification to Discord when an admin spawns an item.")]
    public class AdminSpawnNotice : RustPlugin
    {
        #region Configuration
        private Configuration config;
        private class Configuration
        {
            [JsonProperty(PropertyName = "Webhook_URL")]
            public string WebhookUrl = "";
            
            [JsonProperty(PropertyName = "Webhook Color in HEX")]
            public string WebhookColor = "#FF0000";
        }
		
        protected override void LoadConfig()
        {
            base.LoadConfig();
            try
            {
                config = Config.ReadObject<Configuration>();
                if (config == null) throw new Exception();
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
            config = new Configuration();
        }
		
		
        protected override void SaveConfig() => Config.WriteObject(config);
        #endregion

        private object OnServerCommand(ConsoleSystem.Arg arg)
        {
            if (!arg.cmd.FullName.StartsWith(("inventory.give"))) return null;
            if (arg.Connection.authLevel < 1) return null;
            if (!permission.UserHasGroup(arg.Connection.ownerid.ToString(), "admin")) return null;

            var itemSpawned = "";
            var amount = "";
            var admin = "";
            var player = "";
            var command = "";
            var server = ConVar.Server.hostname;

            switch (arg.cmd.FullName)
            {
                case "inventory.giveid":
                    itemSpawned = ItemManager.FindItemDefinition(int.Parse(arg.Args[0])).displayName.english;
                    amount = arg.Args[1];
                    admin = covalence.Players.FindPlayerById(arg.Connection.ownerid.ToString()).Name;
                    player = admin;
                    command = arg.cmd.FullName;
                    
                    SendDiscordMessage(command, admin, itemSpawned, amount, player, server); 
                    break;
                
                case "inventory.giveto":
                    player = arg.Args[0];
                    itemSpawned = arg.Args[1];
                    amount = arg.Args[2];
                    admin = covalence.Players.FindPlayerById(arg.Connection.ownerid.ToString()).Name;
                    command = arg.cmd.FullName;
                    
                    SendDiscordMessage(command, admin, itemSpawned, amount, player, server); 
                    break;
                
                case "inventory.giveall":
                    player = "Everyone";
                    itemSpawned = arg.Args[0];
                    amount = arg.Args[1];
                    admin = covalence.Players.FindPlayerById(arg.Connection.ownerid.ToString()).Name;
                    command = arg.cmd.FullName;
                    SendDiscordMessage(command, admin, itemSpawned, amount, player, server);
                    break;
            }
            return null;
        }

        private void SendDiscordMessage(string command, string playerName, string itemSpawned, string amount, string itemGivenTo, string server)
        {

            var embed = new Embed()
                .AddField("Admin Name:", playerName, true)
                .AddField("Command Ran:", command, true)
                .AddField("Item Given To:", itemGivenTo, true)
                .AddField("Item Spawned:", itemSpawned, true)
                .AddField("Amount", amount, true)
                .AddField("Server", server, true)
                .SetColor(config.WebhookColor);
            
            var headers = new Dictionary<string, string>() {{"Content-Type", "application/json"}};
            const float timeout = 500f;
            
            webrequest.Enqueue(config.WebhookUrl, new DiscordMessage("", embed).ToJson(),  GetCallback, this,
                RequestMethod.POST, headers, timeout);
        }
        
        private void GetCallback(int code, string response)
        {
            if (response != null && code == 204) return;
            
            Puts($"Error: {code} - Couldn't get an answer from server.");
        }
        
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
        }

        private class Field
        {
            public Field(string name, string value, bool inline)
            {
                Name   = name;
                Value  = value;
                Inline = inline;
            }

            [JsonProperty("name")] public string Name { get; set; }
            [JsonProperty("value")] public string Value { get; set; }
            [JsonProperty("inline")] public bool Inline { get; set; }
        }
        #endregion
    }
}
