using System.Linq;
using JSON;
using UnityEngine;

namespace Oxide.Plugins
{
    [Info("Parentless World Junk Remover", "Ryz0r", "1.0.0")]
    [Description("Removes world junk which doesn't have a parent at location 0,0,0")]
    public class ParentlessWorldJunkRemover : RustPlugin
    {
        private void OnServerInitialized()
        {
            FindAndDestroy();
            timer.Every(300f, FindAndDestroy);
        }

        private void FindAndDestroy()
        {
            var parentlessJunk = BaseNetworkable.serverEntities.entityList
                .Where(x 
                    => x.Value is BaseEntity && x.Value.IsFullySpawned() && !x.Value.HasParent() && x.Value.transform.position == new Vector3(0, 0, 0));

            var count = 0;
            foreach (var junk in parentlessJunk)
            {
                count += 1;
                junk.Value.AdminKill();
                junk.Value.children.ForEach(x => x.AdminKill());
            }

            Puts(count.ToString());
        }
    }
}