using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace Oxide.Plugins
{
    [Info("Ridin Solo", "Ryz0r", "1.0.0")]
    [Description("Only allows a single player to mount vehicles.")]
    public class RidinSolo : RustPlugin
    {
        private object CanMountEntity(BasePlayer player, BaseMountable entity)
        {
            var parentVehicle = entity.GetParentEntity() as BaseVehicle;
            if (parentVehicle == null) return null;
            
            if (parentVehicle.NumMounted() >= 1)
            {
                
                return true;
            }
            return null;
        }
    }
}