using System;
using Newtonsoft.Json;

namespace Oxide.Plugins
{
    [Info("Delete Dropped Items", "Ryz0r", "1.0.0")]
    [Description("Instantly deletes dropped items.")]
    public class DeleteDroppedItems : RustPlugin
    {
        #region Config
        private class Configuration
        {
            [JsonProperty(PropertyName = "Dropped Delete Time (Seconds)")]
            public float DroppedDeleteTime = 10f;
        }
        private Configuration _config;

        protected override void SaveConfig()
        {
            Config.WriteObject(_config);
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
        #endregion
        
        private void OnItemDropped(Item item, BaseEntity entity)
        {
            if (item == null || entity == null) return;
            timer.Once(_config.DroppedDeleteTime, () =>
            {
                entity.Kill();
            });
        }
    }
}