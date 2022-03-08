using Oxide.Core.Libraries.Covalence;
using UnityEngine;

namespace Oxide.Plugins
{
    [Info("Give Entity To Player", "Ryz0r", "1.0.0")]
    [Description("Gives a player an entity spawned in front of them.")]
    public class GiveEntityToPlayer : CovalencePlugin
    {
        private static readonly int GlobalLayerMask = LayerMask.GetMask("Construction", "Default", "Deployed",
            "Resource", "Terrain", "Water", "World");
        
        [Command("give.spawn")]
        private void TestCommand(IPlayer player, string command, string[] args)
        {
            if (args.Length <= 1)
            {
                Puts("Invalid Command Usage!\nTry: give.spawn <steam id/name> <entity short prefab name (I.E. minicopter)>");
                return;
            }

            var bp = BasePlayer.Find(args[0]);
            if (bp == null) return;
            
            RaycastHit hit;
            if (!Physics.Raycast(bp.eyes.HeadRay(), out hit, Mathf.Infinity, GlobalLayerMask)) return;
            
            var position = hit.point + Vector3.up * 2f;
            server.Command($"entity.spawn {args[1]} ({position.x},{position.y},{position.z})");
        }
    }
}