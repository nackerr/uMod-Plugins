using System.Collections.Generic;
using System.Linq;

namespace Oxide.Plugins
{
    [Info("Custom Airwolf Spawn Fix", "Ryz0r", "1.0.0")]
    [Description("Fixes Airwolf spawner on custom maps. Previously, a player could buy immediately after completing a transaction.")]
    public class CustomAirwolfSpawnFix : RustPlugin
    {
        private const string ConversationalistPrefab = "assets/prefabs/npc/bandit/shopkeepers/bandit_conversationalist.prefab";
        private void OnServerInitialized()
        {
            var count = 0;
            var vendorEntities = BaseNetworkable.serverEntities.OfType<VehicleVendor>().ToList();
            if (!vendorEntities.Any())
            {
                Puts("There were no Vehicle Vendors found on this map. This plugin won't help you.");
                return;
            }
            
            foreach (var vendor in vendorEntities)
            {
                if (vendor == null) continue;
                if (vendor.PrefabName != ConversationalistPrefab) continue;
                if (vendor.GetVehicleSpawner() != null && vendor.vehicleSpawner != null && vendor.spawnerRef.IsSet()) continue;
                
                var spawners = new List<VehicleSpawner>();
                Vis.Entities(vendor.transform.position, 75f, spawners);

                if (spawners.Count < 1)
                {
                    Puts($"Vehicle Vendor ({vendor.net.ID}) @ {vendor.transform.position} did not have any nearby spawners. Check your map file if possible.");
                    continue;
                }
            
                vendor.spawnerRef.Set(spawners[0]);
                vendor.vehicleSpawner = spawners[0];
                Puts($"Vehicle Vendor ({vendor.net.ID}) was missing a Vehicle Spawner @ {vendor.transform.position}!");
                count += 1;
            }
            
            Puts($"Fixed {count} broken Vehicle Vendors on your map!");
        }
    }
}