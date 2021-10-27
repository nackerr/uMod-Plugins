using ConVar;

namespace Oxide.Plugins
{
    [Info("Weather Be Gone", "Ryz0r", "1.0.0"), Description("Disables weather on server startup. Enables on plugin unload.")]
    public class WeatherBeGone : RustPlugin
    {
        private void OnServerInitialized()
        {
            NextTick(() =>
            {
                Server.Command("weather.load clear");
                Server.Command("weather.clear_chance 1");
                Server.Command("weather.rain_chance 0");
                Server.Command("weather.fog_chance 0");
                Server.Command("weather.storm_chance 0");
                Server.Command("weather.dust_chance 0");
                Server.Command("Weather.overcast_chance 0");
            });
        }

        private void Unload()
        {
            Server.Command("weather.clear_chance 0.7");
            Server.Command("weather.rain_chance 0.3");
            Server.Command("weather.fog_chance 0.1");
            Server.Command("weather.storm_chance 0.1");
            Server.Command("weather.dust_chance 0.2");
            Server.Command("weather.overcast_chance 0.1");
        }
    }
}