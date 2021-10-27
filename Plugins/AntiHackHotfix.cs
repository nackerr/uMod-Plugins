namespace Oxide.Plugins
{
    [Info("AntiHack Hotfix", "Ryz0r", "1.0.0"), Description("Temporary fix for the admin in rock violation.")]
    public class AntiHackHotfix : RustPlugin
    {
        private object OnPlayerViolation(BasePlayer player, AntiHackType type, float amount)
        {
            if (type == AntiHackType.InsideTerrain && player.IsAdmin) return false;
            return null;
        }
    }
}