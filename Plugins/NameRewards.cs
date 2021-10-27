using Oxide.Core.Libraries.Covalence;

namespace Oxide.Plugins
{
    [Info("NameRewards", "Kappasaurus", "1.1.0", ResourceId = 0)]
    [Description("Adds players to a group based on phrases in their name")]

    class NameRewards : CovalencePlugin
    {
        ConfigData config;

        class ConfigData
        {
            public string Group { get; set; }
            public string[] Phrases { get; set; }
            public bool RemoveOnNameChange { get; set; }
        }

        protected override void LoadDefaultConfig()
        {
            Config.WriteObject(new ConfigData
            {
                Group = "vip",
                Phrases = new[] { "Oxide", "Example" },
                RemoveOnNameChange = true
            }, true);
        }

        void Init()
        {
            config = Config.ReadObject<ConfigData>();
            if (!permission.GroupExists(config.Group))
                permission.CreateGroup(config.Group, config.Group, 0);
        }

        void OnUserConnected(IPlayer player)
        {
            foreach (var phrase in config.Phrases)
            {
                if (player.Name.ToLower().Contains(phrase.ToLower()) &&
                    !permission.UserHasGroup(player.Id, config.Group))
                {
                    permission.AddUserGroup(player.Id, config.Group);
                }
                else if (!player.Name.ToLower().Contains(phrase.ToLower()) &&
                         permission.UserHasGroup(player.Id, config.Group) &&
                         config.RemoveOnNameChange)
                {
                    permission.RemoveUserGroup(player.Id, config.Group);
                }
            }
        }
    }
}