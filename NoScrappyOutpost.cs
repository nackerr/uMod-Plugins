using Rust;
namespace Oxide.Plugins
{
    [Info("No Scrappy Outpost", "Ryz0r", "1.0.0")]
    [Description("Prevents people from dying to scrap helicopter in outpost.")]
    public class NoScrappyOutpost : RustPlugin
    {
        private object OnEntityTakeDamage(BasePlayer player, HitInfo info)
        {
            var scrap = info.Initiator as ScrapTransportHelicopter;
            if (scrap == null) return null;

            if (!player.InSafeZone()) return null;

            if (info.damageTypes.GetMajorityDamageType() == DamageType.Collision)
            {
                return true;
            }

            return null;
        }
    }
}