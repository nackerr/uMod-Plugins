using UnityEngine;

namespace Oxide.Plugins
{
    [Info("Find That Entity", "Ryz0r", "1.0.0"), Description("Finds an entity.")]
    public class FindThatEntity : RustPlugin
    {
        private void Init()
        {
            cmd.AddChatCommand("findent", this, nameof(FindEnt));
        }
        
        private void FindEnt(BasePlayer player)
        {
            RaycastHit raycastHit;
            if (Physics.Raycast(player.eyes.HeadRay(), out raycastHit))
            {
                var target = raycastHit.GetEntity();
                if (!target) return;

                player.ChatMessage(target.ShortPrefabName);
            }
            else
            {
                player.ChatMessage("A valid entity could not be located.");
            }
        }
    }
}