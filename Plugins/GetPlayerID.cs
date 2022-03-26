using System.Collections.Generic;
using UnityEngine;

namespace Oxide.Plugins
{
    public class GetPlayerID : RustPlugin
    {
        [ChatCommand("getsteam")]
        private void PlayerCommand(BasePlayer player, string command, string[] args)
        {
            player.GetPlayerInFront(5f);
        }
    }
}

public static class BasePlayerExtensions
{
    public static BasePlayer GetPlayerInFront (this BasePlayer player, float distance)
    {
        RaycastHit hit;
        if (!Physics.Raycast(player.eyes.HeadRay(), out hit, 5f)) return null;

        var entity = hit.GetEntity();
        if (entity == null) return null;

        var bp = hit.GetEntity() as BasePlayer;
        return bp == null ? null : bp;
    }
}