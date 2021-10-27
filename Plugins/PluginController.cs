using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Oxide.Core;

namespace Oxide.Plugins
{
    [Info("Plugin Controller", "Ryz0r", "1.0.0")]
    [Description("Unloads configured plugins at server wipe, then loads them after some time.")]
    public class PluginController : RustPlugin
    {
        private Configuration _config;
        protected override void SaveConfig() => Config.WriteObject(_config);
        protected override void LoadDefaultConfig() => _config = new Configuration();

        private class Configuration
        {
            [JsonProperty(PropertyName = "PluginsAndTimes", ObjectCreationHandling = ObjectCreationHandling.Replace)]
            public Dictionary<string, float> PluginsAndTimes = new Dictionary<string, float>
            {
                { "UniversalLink", 900f },
                { "PluginName2", 300f },
                { "PluginName3", 600f}
            };
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
        
        private void OnServerInitialized(bool initial)
        {
            if (!initial) return;
            
            foreach (var p in _config.PluginsAndTimes)
            { 
                Interface.Oxide.UnloadPlugin(p.Key);
                timer.Once(p.Value, () =>
                {
                    Interface.Oxide.LoadPlugin(p.Key);
                });
            }
        }
    }
}