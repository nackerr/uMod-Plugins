using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Oxide.Core;

namespace Oxide.Plugins
{
    [Info("Report Gather", "Ryz0r", "1.0.0")]
    [Description("Gathers all reports from your server and syncs them to a database.")]
    public class ReportGather : CovalencePlugin
    {
        #region Configuration

        private Configuration _config;
        protected override void SaveConfig() => Config.WriteObject(_config);
        protected override void LoadDefaultConfig() => _config = new Configuration();
        private class Configuration
        {
            [JsonProperty(PropertyName = "Auth Key")]
            public string AuthKey = "RtlKiQRJsWKC05";
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
        
        private void ProcessReport(string reporter, string targetName, string targetId, string subject, string message, string type, string targetIP = "")
        {
            var bodyInfo = new JObject {{"ReporterId", reporter}, {"TargetId", targetId}, {"TargetName", targetName}, {"Type", type}, {"Subject", subject}, {"Message", message}, {"TargetIP", targetIP}};
            
            webrequest.Enqueue($"https://ryz.sh/reports/index.php",
                bodyInfo.ToString(Formatting.None), (code, response) => 
                {
                    Puts($"A report For {targetId} has been sent.");
                    Puts(response);
                }, this, Core.Libraries.RequestMethod.POST, new Dictionary<string, string>
                {
                    { "Content-Type", " application/json" }, {"Auth-Key", _config.AuthKey}
                }, 15f);   
        }
        
        private void OnPlayerReported(BasePlayer reporter, string targetName, string targetId, string subject, string message, string type)
        {
            var p = BasePlayer.FindByID(Convert.ToUInt64(targetId));
            if (p && p.IsConnected)
            {
                ProcessReport(reporter.UserIDString, targetName, targetId, subject, message, type, p.net.connection.ipaddress);
            }
            else
            {
                ProcessReport(reporter.UserIDString, targetName, targetId, subject, message, type, null);
            }
        }
    }
}