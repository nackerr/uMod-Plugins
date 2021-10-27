using System;
using System.Collections.Generic;
using System.Linq;
using Facepunch;
using Newtonsoft.Json;
using UnityEngine;
using Application = UnityEngine.Application;

namespace Oxide.Plugins
{
    [Info("Remove Console Messages", "Ryz0r", "0.1")]
    [Description("Removes certain messages of your choice from the console")]
    public class RemoveConsoleMessages : RustPlugin
    {
        private void Loaded()
        {
            Puts("Console messages are now being filtered from RemoveConsoleMessages.json in config.");
        }

        private void Init()
        {
            Application.logMessageReceived += HandleLog;
            Application.logMessageReceived -= Output.LogHandler;
        }

        private void Unload()
        {
            Application.logMessageReceived += Output.LogHandler;
            Application.logMessageReceived -= HandleLog;
        }

        private void HandleLog(string message, string stackTrace, LogType type)
        {
            if (!string.IsNullOrEmpty(message) && !config.FilterList.Any(message.Contains))
                Output.LogHandler(message, stackTrace, type);
        }

        #region Configuration

        private Configuration config;

        private class Configuration
        {
            [JsonProperty(PropertyName = "Message Filter List",
                ObjectCreationHandling = ObjectCreationHandling.Replace)]
            public readonly List<string> FilterList = new List<string>
                {"used *kill* on ent", "An error occurred whilst fetching your store information"};
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


        protected override void SaveConfig()
        {
            Config.WriteObject(config);
        }

        #endregion
    }
}