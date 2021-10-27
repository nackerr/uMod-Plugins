using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Oxide.Core;
using Oxide.Core.Libraries.Covalence;

namespace Oxide.Plugins
{
    [Info("Stack Multiplier", "Ryz0r", "1.0.3")]
    [Description(
        "Allows you to multiply all items stack size by a multiplier, except those in the blocked config list.")]
    public class StackMultiplier : CovalencePlugin
    {
        private const string UsePerm = "stackmultiplier.use";
        private static PluginData _data;
        private readonly Dictionary<string, int> _defaultSizes = new Dictionary<string, int>();

        private readonly List<string> weaponList = new List<string>
        {
            "shotgun.double", "shotgun.pump", "shotgun.spas12", "shotgun.waterpipe", "pistol.nailgun", "pistol.eoka",
            "pistol.m92", "pistol.python", "pistol.semiauto", "rifle.ak", "rifle.bolt", "rifle.l96", "rifle.lr300",
            "rifle.m39", "rifle.semiauto", "pistol.revolver", "smg.thompson", "smg.mp5", "smg.2",
            "multiplegrenadelauncher", "rocket.launcher", "lmg.m249"
        };

        private Configuration _config;
        private int _multiplier = 1;

        #region Localization

        protected override void LoadDefaultMessages()
        {
            lang.RegisterMessages(new Dictionary<string, string>
            {
                ["CurrStack"] = "The current stack size is {0}x.",
                ["ArgOver"] = "You have provided too many arguments. Try: stackmultiplier.multiply 2 for 2x stacks.",
                ["DataSaved"] = "The stack sizes have been written to the data file.",
                ["ArgOverReset"] = "You have provided too many arguments.",
                ["NotInt"] = "That is not a valid integer.",
                ["BlockedItems"] = "Stack size set was blocked for items in config: {0}.",
                ["StackReset"] = "All stacks have been reset.",
                ["NoPerms"] = "You lack the permissions to use this command."
            }, this);
        }

        #endregion

        [Command("stackmultiplier.multiply")]
        private void MultiplyCommand(IPlayer player, string command, string[] args)
        {
            if (!player.HasPermission(UsePerm))
            {
                player.Reply(lang.GetMessage("NoPerms", this, player.Id));
                return;
            }

            if (args.Length == 0)
            {
                player.Reply(string.Format(lang.GetMessage("CurrStack", this, player.Id), _multiplier.ToString()));
                return;
            }

            if (args.Length > 2)
            {
                player.Reply(lang.GetMessage("ArgOver", this, player.Id));
                return;
            }

            int localMultiplier;
            if (int.TryParse(args[0], out localMultiplier))
            {
                _multiplier = localMultiplier;
                if (_config.BlockedList.Contains("weapons"))
                {
                    foreach (var gameitem in ItemManager.itemList)
                        if (!_config.BlockedList.Contains(gameitem.shortname) ||
                            !weaponList.Contains(gameitem.shortname))
                            ChangeSize(gameitem, _multiplier);
                }
                else
                {
                    foreach (var gameitem in ItemManager.itemList)
                        if (!_config.BlockedList.Contains(gameitem.shortname))
                            ChangeSize(gameitem, _multiplier);
                }

                SaveData();
                player.Reply(string.Format(lang.GetMessage("BlockedItems", this, player.Id),
                    string.Join(", ", _config.BlockedList)));
                player.Reply(string.Format(lang.GetMessage("CurrStack", this, player.Id), _multiplier.ToString()));
            }
            else
            {
                player.Reply(lang.GetMessage("NotInt", this, player.Id));
            }
        }

        [Command("stackmultiplier.reset")]
        private void ResetCommand(IPlayer player, string command, string[] args)
        {
            if (!player.HasPermission(UsePerm))
            {
                player.Reply(lang.GetMessage("NoPerms", this, player.Id));
                return;
            }

            if (args.Length > 0)
            {
                player.Reply(lang.GetMessage("ArgOverReset", this, player.Id));
                return;
            }

            ResetStacks();
            player.Reply(lang.GetMessage("StackReset", this));
        }

        [Command("stackmultiplier.save")]
        private void SaveCommand(IPlayer player, string command, string[] args)
        {
            if (!player.HasPermission(UsePerm))
            {
                player.Reply(lang.GetMessage("NoPerms", this, player.Id));
                return;
            }

            if (args.Length > 0)
            {
                player.Reply(lang.GetMessage("ArgOverReset", this, player.Id));
                return;
            }

            SaveData();
            player.Reply(lang.GetMessage("DataSaved", this));
        }

        private void ChangeSize(ItemDefinition gameitem, int multiplier)
        {
            gameitem.stackable = _defaultSizes[gameitem.shortname] * _multiplier;
            _data.savedSizes.Add(gameitem.shortname, gameitem.stackable);
        }

        private void ResetStacks()
        {
            foreach (var gameitem in ItemManager.itemList) gameitem.stackable = _defaultSizes[gameitem.shortname];

            _multiplier = 1;
        }

        #region Data

        private void SaveData()
        {
            Interface.Oxide.DataFileSystem.WriteObject(Name, _data);
        }

        private void LoadData()
        {
            try
            {
                _data = Interface.Oxide.DataFileSystem.ReadObject<PluginData>(Name);
            }
            catch (Exception e)
            {
                PrintError(e.ToString());
            }

            if (_data == null) _data = new PluginData();
        }

        private class PluginData
        {
            [JsonProperty(PropertyName = "Saved Sizes", ObjectCreationHandling = ObjectCreationHandling.Replace)]
            public readonly Dictionary<string, int> savedSizes = new Dictionary<string, int>();
        }

        #endregion

        #region Configuration

        private class Configuration
        {
            [JsonProperty(PropertyName = "BlockedList", ObjectCreationHandling = ObjectCreationHandling.Replace)]
            public readonly List<string> BlockedList = new List<string> {"hammer", "weapons"};

            [JsonProperty(PropertyName = "ResetStacksAtWipe")]
            public readonly bool ResetStacksAtWipe = false;
        }

        protected override void LoadConfig()
        {
            base.LoadConfig();
            try
            {
                _config = Config.ReadObject<Configuration>();
                if (_config == null) throw new Exception();
                SaveConfig();
            }
            catch
            {
                PrintError("Your configuration file contains an error. Using default configuration values.");
                LoadDefaultConfig();
            }
        }

        protected override void LoadDefaultConfig()
        {
            PrintWarning("A new configuration file is being generated.");
            _config = new Configuration();
        }


        protected override void SaveConfig()
        {
            Config.WriteObject(_config);
        }

        #endregion

        #region Initialization Stuff

        private void Init()
        {
            permission.RegisterPermission(UsePerm, this);
        }

        private void OnServerSave()
        {
            SaveData();
        }

        private void OnServerInitialized()
        {
            foreach (var gameitem in ItemManager.itemList) _defaultSizes.Add(gameitem.shortname, gameitem.stackable);

            if (_config.ResetStacksAtWipe == false)
                foreach (var gameitem in ItemManager.itemList)
                {
                    LoadData();
                    gameitem.stackable = _data.savedSizes[gameitem.shortname];
                }
        }

        private void Unload()
        {
            ResetStacks();
        }

        #endregion
    }
}