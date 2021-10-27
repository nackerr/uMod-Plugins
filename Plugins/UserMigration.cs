namespace Oxide.Plugins
{
    [Info("User Migration", "Ryz0r", "1.0.0")]
    [Description("Migrate permissions from one group to another.")]
    public class UserMigration : RustPlugin
    {
        private const string Group1 = "verified";
        private const string Group2 = "linked";
        private void Init()
        {
            if (!permission.GroupExists(Group1) || !permission.GroupExists(Group2)) return;
            
            foreach (var p in permission.GetGroupPermissions(Group1))
            {
                permission.GrantGroupPermission(Group2, p, this);
            }
        }
    }
}