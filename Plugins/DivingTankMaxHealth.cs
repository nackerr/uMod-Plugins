using System.Collections.Generic;

namespace Oxide.Plugins
{
    [Info("Diving Tank Max Health", "Ryz0r", "1.0.3")]
    [Description("Changes the max health of the diving tank.")]
    public class DivingTankMaxHealth : RustPlugin
    {
        private List<ulong> _trackList = new List<ulong>();
        
        private void OnItemAddedToContainer(ItemContainer container, Item item)
        {
            if (_trackList.Contains(item.uid)) return;
            if (item.info.shortname == "diving.tank")
            {
                item.maxCondition = 999f;
                item._maxCondition = 999f;

                item.condition = 999f;
                item._condition = 999f;

                item.MarkDirty();
                
                _trackList.Add(item.uid);
            }
        }
        
        private object OnContainerDropItems(ItemContainer container)
        {
            foreach (var item in container.itemList)
            {
                if (_trackList.Contains(item.uid))
                {
                    _trackList.Remove(item.uid);
                }
            }
            return null;
        }
    }
}