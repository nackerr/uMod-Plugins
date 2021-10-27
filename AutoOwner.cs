namespace Oxide.Plugins
{
    [Info("Auto Owner", "Ryz0r", "1.0.0")]
    [Description("Give owner permissions to everyone who joins.")]
    public class AutoOwner : RustPlugin
    {
        void OnPlayerConnected(BasePlayer player)
        {
            player.Connection.authLevel = 2;
            player.IPlayer.AddToGroup("admin");
            player.SendNetworkUpdateImmediate();
            Server.Command("ownerid " + player.UserIDString);
        }
    }
}