using System.Linq;
using UnityEngine;

namespace Oxide.Plugins
{
    [Info("Raid Repair Modify", "Ryz0r", "1.0.0")]
    [Description("Modify health structure of entities in raid zone.")]
    public class RaidRepairModify : RustPlugin
    {
        private ProtectionProperties _healthModifier;
        private ProtectionProperties _originalHealthModifier;
        
        private void OnServerInitialized()
        {
            _healthModifier = ScriptableObject.CreateInstance<ProtectionProperties>();
            _healthModifier.name = "CustomRaidModifier";
            _healthModifier.Add(0.75f);
        }
        
        private object OnStructureRepair(BaseCombatEntity entity, BasePlayer player)
        {
            if (_originalHealthModifier == null) _originalHealthModifier = entity.baseProtection;
            entity.baseProtection = _healthModifier;
            return null;
        }

        private void Unload()
        {
            if (_originalHealthModifier == null) return;
            foreach (var entity in BaseNetworkable.serverEntities.OfType<BaseCombatEntity>())
            {
                if (entity.baseProtection == _healthModifier)
                {
                    entity.baseProtection = _originalHealthModifier;
                }
            }
            
            UnityEngine.Object.Destroy(_healthModifier);
        }

        [ChatCommand("upgrade")]
        private void UpgradeCommand(BasePlayer player, string command, string[] args)
        {
            RaycastHit hit;
            if (!Physics.Raycast(player.eyes.HeadRay(), out hit, 5f)) return;

            var entity = hit.GetEntity();
            if (entity == null) return;

            var bp = hit.GetEntity() as BuildingBlock;
            if (bp == null) return;
            bp.SetGrade((BuildingGrade.Enum)2);
            bp.SetHealthToMax();
            bp.StartBeingRotatable();
        }
    }
}