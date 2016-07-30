using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using PokeGoBot.WPF.Handlers;
using PokeGoBot.WPF.Logging;
using PokemonGo.RocketAPI;
using PokemonGo.RocketAPI.Extensions;
using POGOProtos.Map.Fort;

namespace PokeGoBot.WPF.Bot.Handlers
{
    public interface IPokestopsHandler
    {
        Task FarmPokestops(Client client);
    }

    public class PokestopsHandler : IPokestopsHandler
    {
        private readonly ISettingsHandler _settings;
        private readonly ILogger _logger;

        public PokestopsHandler(ISettingsHandler settings, ILogger logger)
        {
            _settings = settings;
            _logger = logger;
        }

        public async Task FarmPokestops(Client client)
        {
            var mapObjects = await client.Map.GetMapObjects();

            var pokeStops =
                mapObjects.MapCells.SelectMany(i => i.Forts)
                    .Where( i =>
                            i.Type.Equals(FortType.Checkpoint) &&
                            i.CooldownCompleteTimestampMs < DateTime.UtcNow.ToUnixTime());

            foreach (var pokeStop in pokeStops)
            {
                var distance = Navigation.DistanceBetween2Coordinates(client.CurrentLatitude, client.CurrentLongitude,
                    pokeStop.Latitude, pokeStop.Longitude);
                if (distance > 100)
                    await Task.Delay(15000);
                else
                    await Task.Delay(500);

                await client.Player.UpdatePlayerLocation(pokeStop.Latitude, pokeStop.Longitude,
                            _settings.Settings.DefaultAltitude);

                var fortInfo = await client.Fort.GetFort(pokeStop.Id, pokeStop.Latitude, pokeStop.Longitude);
                var fortSearch = await client.Fort.SearchFort(pokeStop.Id, pokeStop.Latitude, pokeStop.Longitude);
                
                await Task.Delay(1000);
            }
        }
    }
}
