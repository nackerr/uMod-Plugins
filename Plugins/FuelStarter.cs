namespace Oxide.Plugins
{
    [Info("Fuel Starter", "Ryz0r", "1.0.0"), Description("Allows MiniCopters to spawn with Low Grade.")]
    public class FuelStarter : RustPlugin
    {
        private void OnEntitySpawned(MiniCopter mini)
        {
            if (mini == null) return;
            
            NextTick(() =>
            {
                var fc = mini.GetFuelSystem().GetFuelContainer();
                if (fc == null) return;
                
                fc.inventory.AddItem(fc.allowedItem, 150);
            });
        }
    }
}