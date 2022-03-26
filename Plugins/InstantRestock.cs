using System.Linq;
namespace Oxide.Plugins
{
    [Info("Instant Restock", "Ryz0r/Yoshi", "1.0.0")]
    [Description("Instantly restocks certain items.")]
    public class InstantRestock : RustPlugin
    {
        private object OnNpcGiveSoldItem(NPCVendingMachine machine, Item soldItem, BasePlayer buyer)
        {
            if (machine.sellOrders.sellOrders.Any(x => x.currencyID == -858312878) && machine.sellOrders.sellOrders.All(x => x.itemToSellID != -1021495308))
            {
                var item = ItemManager.CreateByItemID(soldItem.info.itemid, soldItem.amount);
                item.MoveToContainer(machine.inventory);
            }

            return null;
        }
    }
}