using System.Collections.Generic;

namespace Oxide.Plugins
{
    [Info("Quarry Lock", "Ryz0r", "1.0.0")]
    [Description("Locks a quarry to the player who first starts it.")]
    public class QuarryLock
    {
        private Dictionary<MiningQuarry, string> _quarries = new Dictionary<MiningQuarry, string>();

        private void OnQuarryToggled(MiningQuarry miningQuarry, BasePlayer player)
        {
            if (miningQuarry == null || miningQuarry.OwnerID == 0) return;
            
            if (_quarries.ContainsKey(miningQuarry) && _quarries[miningQuarry] != player.UserIDString)
            {
                player.ChatMessage("Quarry is locked to another player.");
            }
        }
    }
}