using System;
using System.Linq;
using System.Threading.Tasks;
using PokeGoBot.Core.Data;
using PokeGoBot.Core.Logging;
using PokemonGo.RocketAPI;
using PokemonGo.RocketAPI.Extensions;
using POGOProtos.Map.Fort;
using POGOProtos.Networking.Responses;

namespace PokeGoBot.Core.Logic.Handlers
{
    public interface IPokestopsHandler
    {
        Task FarmPokestops(Client client);
    }

    public class PokestopsHandler : IPokestopsHandler
    {
        private readonly ILogger _logger;
        private readonly ICatchPokemonHandler _catchPokemonHandler;
        private readonly IWalkingHandler _walkingHandler;
        private readonly ISettingsHandler _settings;

        public PokestopsHandler(ISettingsHandler settings, 
                                ILogger logger,
                                ICatchPokemonHandler catchPokemonHandler,
                                IWalkingHandler walkingHandler)
        {
            _settings = settings;
            _logger = logger;
            _catchPokemonHandler = catchPokemonHandler;
            _walkingHandler = walkingHandler;
        }

        public async Task FarmPokestops(Client client)
        {
            var mapObjects = await client.Map.GetMapObjects();

            var pokeStops =
                mapObjects.MapCells.SelectMany(i => i.Forts)
                    .Where(i =>
                        i.Type.Equals(FortType.Checkpoint) &&
                        i.CooldownCompleteTimestampMs < DateTime.UtcNow.ToUnixTime());

            _logger.Write($"Found {pokeStops.Count()} pokestops nearby", LogLevel.INFO);

            foreach (var pokeStop in pokeStops)
            {
                var distance = Navigation.CalculateDistanceInMeters(client.CurrentLatitude, client.CurrentLongitude,
                    pokeStop.Latitude, pokeStop.Longitude);

                if (distance <= _settings.Settings.PlayerMaxTravelInMeters)
                {
                    if (_settings.Settings.UpdateLocation)
                    {
                        _logger.Write($"Traveling to location [LAT: {pokeStop.Latitude} | LON: {pokeStop.Longitude}]",
                        LogLevel.INFO);

                        await _walkingHandler.Walking(client, pokeStop.Latitude, pokeStop.Longitude, 
                            _settings.Settings.PlayerWalkingSpeed, () => _catchPokemonHandler.CatchAllNearbyPokemons(client));
                    }

                    var fortInfo = await client.Fort.GetFort(pokeStop.Id, pokeStop.Latitude, pokeStop.Longitude);
                    await Task.Delay(2000);

                    _logger.Write($"Spinning pokestop: {fortInfo.Name}", LogLevel.INFO);
                    var fortSearch = await client.Fort.SearchFort(pokeStop.Id, pokeStop.Latitude, pokeStop.Longitude);
                    if (fortSearch.Result == FortSearchResponse.Types.Result.Success)
                    {
                        if (fortSearch.ExperienceAwarded == 0)
                            _logger.Write("[Softban] No exp on pokestop.", LogLevel.ERROR);
                        else
                            _logger.Write($"Reward: {fortSearch.ExperienceAwarded}xp", LogLevel.INFO);
                        
                        foreach (var r in fortSearch.ItemsAwarded.GroupBy(x => x.ItemId))
                            _logger.Write($"Reward: {r.Count()}x {r.Key} ", LogLevel.INFO);

                    }
                    else if(fortSearch.Result == FortSearchResponse.Types.Result.InCooldownPeriod)
                        _logger.Write($"Pokestop in cooldown: {fortSearch.CooldownCompleteTimestampMs}", LogLevel.WARN);
                    else if(fortSearch.Result == FortSearchResponse.Types.Result.InventoryFull)
                        _logger.Write("Inventory full", LogLevel.WARN);
                    else if (fortSearch.Result == FortSearchResponse.Types.Result.OutOfRange)
                        _logger.Write("Pokestop to far away", LogLevel.WARN);

                    await Task.Delay(1000);
                }
            }
        }
    }
}
