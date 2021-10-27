using System.Collections.Generic;

namespace Oxide.Plugins
{
    public class HomeMapMarker : RustPlugin
    {
        private const string MarkHomePerm = "homemapmarker.use";
        private Dictionary<ulong, int> _homeList = new Dictionary<ulong,int>();

        [ChatCommand("markhome")]
        private void MarkHomeCommand(BasePlayer player, string command, string[] args)
        {
            if (!permission.UserHasPermission(player.UserIDString, MarkHomePerm))
            {
                player.ChatMessage("No perms to set home.");
                return;
            }

            if (args.Length != 1)
            {
                player.ChatMessage("Incorrect Args.");
                return;
            }

            switch (args[0].ToLower())
            {
                case "set":
                    if (_homeList.ContainsKey(player.userID))
                    {
                        player.ChatMessage("You already have your home marked. Please remove it first.");
                    }
                    else
                    {
                        
                    }
                    break;
                
                case "remove":
                    break;
                
                default:
                    break;
            }
        }
    }
}