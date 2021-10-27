using System.Collections.Generic;
using UnityEngine;

namespace Oxide.Plugins
{
    [Info("Death Location Respawn", "Ryz0r", "1.0.0")]
    [Description("Returns a player to their death location on respawn.")]
    public class DeathLocationRespawn : RustPlugin
    {
        private Dictionary<ulong, Vector3> _deathPoints = new Dictionary<ulong, Vector3>();
        
        private object OnPlayerRespawn(BasePlayer player)
        {
            return _deathPoints.ContainsKey(player.userID) ? new BasePlayer.SpawnPoint() { pos = _deathPoints[player.userID] } : null;
        }
    }
}