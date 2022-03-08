using System.Collections.Generic;
namespace Oxide.Plugins
{
    [Info("Test Gen Pickup Prevention", "Ryz0r", "1.0.0")]
    [Description("Prevents others from meddling with your test generators!")]
    public class TestGenPickupPrevention : RustPlugin
    {
        private object CanPickupEntity(BasePlayer player, ElectricGenerator generator)
        {
            if (player.currentTeam != 0UL && RelationshipManager.ServerInstance.playerToTeam[player.userID].members
                .Contains(generator.OwnerID)) return null;
            if (generator.OwnerID == player.userID) return null;
            
            return false;
        }
    }
}