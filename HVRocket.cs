namespace Oxide.Plugins
{
    public class HVRocket : RustPlugin
    {
        private object OnPlayerDeath(BasePlayer player, HitInfo info)
        {
            if (info.WeaponPrefab.ShortPrefabName == "rocket_hv")
            {
                Puts("HV Rocket fired.");
            }
            return null;
        }
    }
}