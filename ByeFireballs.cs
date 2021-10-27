using System.Collections.Generic;
using UnityEngine;

namespace Oxide.Plugins
{
    [Info("Bye Fireballs", "Ryz0r", "1.0.2")]
    [Description("Removes fireballs from MiniCopter crashes.")]
    public class ByeFireballs : RustPlugin
    {
        private void OnEntitySpawned(BaseHelicopterVehicle m)
        {
            m.fireBall.guid = null;
        }
    }
}