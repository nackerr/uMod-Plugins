using ConVar;
using Oxide.Core.Libraries.Covalence;

namespace Oxide.Plugins
{
    [Info("The Flood", "Ryz0r", "1.0.0")]
    [Description("Simulates an awesome flood.")]
    public class TheFlood : RustPlugin
    {
        private Timer _startTimer;
        private Timer _stopTimer;
        private string[] blockMsgList = {"ocean"};

        private void Init()
        {
            AddCovalenceCommand("flood", nameof(FloodCommand));
        }
        
        void OnPlayerBanned(string name, ulong id, string address, string reason)
        {
            if (reason.Contains("EAC"))
            {
                //Do Discord Stuff
            }
        }

        private void FloodCommand(IPlayer player, string command, string[] args)
        {
            if (args.Length == 0)
            {
                Server.Broadcast("<color=red>The Flood</color> has been started. Good luck.");
                StartIncrease();
            }
            else
            {
                if (args.Length == 1)
                {
                    if (args[0] == "stop")
                    {
                        timer.Destroy(ref _startTimer);
                        covalence.Server.Command("weather.rain 0");
                        covalence.Server.Command("oceanlevel 0");
                        Server.Broadcast("<color=red>The Flood</color> has been stopped.");
                    }
                    else
                    {
                        player.Reply("This is not a valid command. Try again.");
                    }
                }
            }
        }

        private void StartIncrease()
        {
            covalence.Server.Command("weather.rain 0.8");

            float oceanLev = 0;
            _startTimer = timer.Every(2.5f, () =>
            {
                oceanLev += 0.5f;
                covalence.Server.Command("oceanlevel", oceanLev);

                if (oceanLev == 30)
                {
                    timer.Destroy(ref _startTimer);
                    Server.Broadcast(
                        "<color=red>The Flood</color> has been come and gone. You must now <color=red>survive</color>. Good luck.");

                    covalence.Server.Command("weather.rain 0");

                    timer.Once(10f, () => { StartDecrease(); });
                }
            });
        }

        private void StartDecrease()
        {
            var oceanLev = Env.oceanlevel;
            _stopTimer = timer.Every(2.5f, () =>
            {
                oceanLev -= 1f;
                covalence.Server.Command("oceanlevel", oceanLev);

                if (oceanLev == 0)
                {
                    timer.Destroy(ref _stopTimer);
                    Server.Broadcast("You have survived <color=red>The Flood</color>. Congrats.");
                }
            });
        }
    }
}