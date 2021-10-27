using UnityEngine;

namespace Oxide.Plugins
{
    [Info("Disconnect Lights", "Ryz0r", "1.0.0")]
    [Description("Turns off all of a players power consuming light sources when they disconnect.")]
    public class DisconnectLights : RustPlugin
    {
        private void OnPlayerDisconnected(BasePlayer player, string reason)
        {
            foreach(var lightSource in UnityEngine.Object.FindObjectsOfType<IOEntity>())
            {
                if (lightSource.OwnerID != player.userID) return;
                
                lightSource.SetFlag(BaseEntity.Flags.Disabled, true);
                lightSource.SetFlag(BaseEntity.Flags.On, false);
                lightSource.UpdateHasPower(0, 0);
            }
        }
        
        private void OnPlayerConnected(BasePlayer player)
        {
            foreach(var lightSource in UnityEngine.Object.FindObjectsOfType<IOEntity>())
            {
                if (lightSource.OwnerID != player.userID) return;
                
                lightSource.SetFlag(BaseEntity.Flags.Disabled, false);
                lightSource.SetFlag(BaseEntity.Flags.On, true);
            }
        }
    }
}