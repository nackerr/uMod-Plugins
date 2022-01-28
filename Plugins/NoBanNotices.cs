namespace Oxide.Plugins
{
    [Info("No Ban Notices", "Ryz0r", "1.0.0")]
    [Description("Hides ban notices from all players.")]
    public class NoBanNotices : RustPlugin
    {
        private object OnServerMessage(string message, string name)
        {
            if ((message.ToLower().Contains("kicking") || message.ToLower().Contains("kickbanning")) && name.ToLower() == ("server")) return true;
            return null;
        }
    }
}