namespace Oxide.Plugins
{
    [Info("AntiHack Hotfix", "Ryz0r", "1.0.1"), Description("Temporary fix for the admin in rock violation.")]
    public class AntiHackHotfix : RustPlugin
    {
        private void Init()
        {
            permission.RegisterPermission("ahhf.bypass", this);
        }
        
        private object OnPlayerViolation(BasePlayer player, AntiHackType type, float amount)
        {
            if (type == AntiHackType.InsideTerrain && (player.IsAdmin || permission.UserHasPermission(player.UserIDString, "ahhf.bypass"))) return false;
            return null;
        }
    }
}
