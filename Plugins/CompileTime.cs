using System;
using System.Linq;
using Oxide.Core.Libraries.Covalence;

namespace Oxide.Plugins
{
    [Info("Compile Time", "Ryz0r", "1.0.0"), Description("Lists useful information about plugin runtime.")]
    public class CompileTime : CovalencePlugin
    {
        private string rowMessage = "{0} {1} {2}";

        [Command("compiletime.all")]
        private void CompileTimeCommandAll(IPlayer player, string command, string[] args)
        {
            if (!player.IsAdmin)
                return;

            var table = new TextTable();
            table.AddColumns("Plugin", "Active Time", "% Of Total Time");

            foreach (var p in plugins.GetAll().OrderByDescending(p => p.TotalHookTime))
            {
                table.AddRow(p.Name, p.TotalHookTime.ToString("0.##"),
                    (p.TotalHookTime / UnityEngine.Time.realtimeSinceStartup * 100).ToString("0.##"));
            }
            
            player.Reply(table.ToString());
        }
        
        [Command("compiletime.highest")]
        private void CompileTimeCommandHighest(IPlayer player, string command, string[] args)
        {
            if (!player.IsAdmin)
                return;
            
            var table = new TextTable();
            table.AddColumns("Plugin", "Active Time", "% Of Total Time");

            foreach (var p in plugins.GetAll().OrderByDescending(p => p.TotalHookTime))
            {
                table.AddRow(p.Name, p.TotalHookTime.ToString("0.##"),
                    (p.TotalHookTime / UnityEngine.Time.realtimeSinceStartup * 100).ToString("0.##"));
                
                break;
            }
            
            player.Reply(table.ToString());
        }
    }
}