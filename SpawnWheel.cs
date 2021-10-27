using UnityEngine;

namespace Oxide.Plugins
{
    [Info("Spawn Casino Wheel", "Ryz0r", "1.0.0")]
    [Description("I fucked up. Spawning a new wheel!")]
    public class SpawnWheel : RustPlugin
    {
        private static LayerMask GROUND_MASKS = LayerMask.GetMask("Terrain", "World", "Construction");
        
        [ChatCommand("wheel")]
        private void WheelCommand(BasePlayer player, string command, string[] args)
        {
            RaycastHit hit;
            if (!Physics.Raycast(player.eyes.HeadRay(), out hit, 10f, GROUND_MASKS)) return;

            var position = hit.point;
            var wheelEntity = (BigWheelGame) GameManager.server.CreateEntity("assets/prefabs/misc/casino/bigwheel/big_wheel.prefab", position);
            wheelEntity.transform.localRotation = Quaternion.Euler(90, 0, 145);
            wheelEntity.Spawn();
        }
    }
}