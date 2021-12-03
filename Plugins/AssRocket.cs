using System.CodeDom;
using Oxide.Core.Libraries.Covalence;
using Rust;
using UnityEngine;

namespace Oxide.Plugins
{
    [Info("Ass Rocket", "Ryz0r", "0.1.2")]
    [Description("Sends a rocket up someones ass.")]
    public class AssRocket : CovalencePlugin
    {
        private const string RocketPrefab = "assets/prefabs/ammo/rocket/rocket_hv.prefab";
        private const string ChairPrefab = "assets/bundled/prefabs/static/chair.invisible.static.prefab";
        
        [Command("arocket"), Permission("assrocket.use")]
        private void TestCommand(IPlayer player, string command, string[] args)
        {
            if (args.Length < 1)
            {
                player.Reply("Missing Target Steam ID.");
                return;
            }

            var targetPlayer = FindTargetPlayer(args[0]);
            if (targetPlayer == null)
            {
                player.Reply("Specified Target is Offline.");
                return;
            }
            
            var rocket = (BaseProjectile)GameManager.server.CreateEntity(RocketPrefab, targetPlayer.transform.position + new Vector3(0, 0, 0));
            var invisibleChair = (BaseChair)GameManager.server.CreateEntity(ChairPrefab, new Vector3((float)0, (float)-0.3, (float)-0.3));
            
            var proj = rocket.GetComponent<ServerProjectile>();
            if (proj == null) return;
            
            rocket.Spawn();
            invisibleChair.Spawn();
            
            invisibleChair.MountPlayer(targetPlayer);
            invisibleChair.SetParent(rocket);

            UnityEngine.Object.DestroyImmediate(rocket.GetComponent<Collider>());
            Physics.IgnoreCollision(rocket.GetComponent<Collider>(), invisibleChair.GetComponent<Collider>());
            
            proj.InitializeVelocity(new Vector3(0, 10, 0));
        }

        private BasePlayer FindTargetPlayer (string playerID) => BasePlayer.Find(playerID);
    }
}