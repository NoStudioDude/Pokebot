using System;
using System.Device.Location;
using System.Threading.Tasks;
using PokeGoBot.Core.Data;
using PokeGoBot.Core.Logging;
using PokeGoBot.Core.Logic.Helpers;
using PokemonGo.RocketAPI;
using POGOProtos.Networking.Responses;

namespace PokeGoBot.Core.Logic.Handlers
{
    public interface IWalkingHandler
    {
        Task<PlayerUpdateResponse> Walking(Client client, double targetlat, double targetlng,
            double walkingSpeedInKilometersPerHour, Func<Task> searchForPokemonFunction);
    }

    public class WalkingHandler : IWalkingHandler
    {
        private readonly ISettingsHandler _settings;
        private readonly ILogger _logger;

        public WalkingHandler(ISettingsHandler settings,
                              ILogger logger)
        {
            _settings = settings;
            _logger = logger;
        }

        public async Task<PlayerUpdateResponse> Walking(Client client, double targetlat, double targetlng, double walkingSpeedInKilometersPerHour, 
            Func<Task> searchForPokemonFunction)
        {
            var speedInMetersPerSecond = walkingSpeedInKilometersPerHour / 3.6;

            var sourceLocation = new GeoCoordinate(client.CurrentLatitude, client.CurrentLongitude);
            var targetLocation = new GeoCoordinate(targetlat, targetlng);

            var distanceToTarget = Navigation.CalculateDistanceInMeters(sourceLocation, targetLocation);
            _logger.Write(
                $"Distance to target location: {distanceToTarget:0.##} meters. Will take {distanceToTarget / speedInMetersPerSecond:0.##} " +
                "seconds!", 
                LogLevel.INFO);

            var nextWaypointBearing = Navigation.DegreeBearing(sourceLocation, targetLocation);
            var nextWaypointDistance = speedInMetersPerSecond;
            var waypoint = Navigation.CreateWaypoint(sourceLocation, nextWaypointDistance, nextWaypointBearing);

            //Initial walking
            var requestSendDateTime = DateTime.Now;
            var result = 
                await client.Player.UpdatePlayerLocation(waypoint.Latitude, waypoint.Longitude, 
                GoogleAltitudeRequestor.GetAltitude(waypoint.Latitude, waypoint.Longitude));

            _settings.Settings.RocketSettings.DefaultLatitude = client.CurrentLatitude;
            _settings.Settings.RocketSettings.DefaultLongitude = client.CurrentLongitude;

            if (searchForPokemonFunction != null && _settings.Settings.CatchPokemons)
                await searchForPokemonFunction();

            var locatePokemonWhileWalkingDateTime = DateTime.Now;
            do
            {
                var millisecondsUntilGetUpdatePlayerLocationResponse =
                    (DateTime.Now - requestSendDateTime).TotalMilliseconds;

                sourceLocation = new GeoCoordinate(client.CurrentLatitude, client.CurrentLongitude);
                var currentDistanceToTarget = Navigation.CalculateDistanceInMeters(sourceLocation.Latitude, sourceLocation.Longitude, 
                    targetLocation.Latitude, targetLocation.Longitude);

                if (currentDistanceToTarget < 30 && _settings.Settings.PlayerWalkingSpeed > 10)
                {
                    if (speedInMetersPerSecond > 10)
                    {
                        _logger.Write("We are within 30 meters of the target. Speeding down to ~10 km/h to not pass the target.", 
                            LogLevel.INFO);
                        speedInMetersPerSecond = 10;
                    }
                }

                nextWaypointDistance = Math.Min(currentDistanceToTarget, 
                    millisecondsUntilGetUpdatePlayerLocationResponse / 1000 * speedInMetersPerSecond);

                nextWaypointBearing = Navigation.DegreeBearing(sourceLocation, targetLocation);
                waypoint = Navigation.CreateWaypoint(sourceLocation, nextWaypointDistance, nextWaypointBearing);

                requestSendDateTime = DateTime.Now;

                _logger.Write($"Updating location to LAT: {waypoint.Latitude}, LNG: {waypoint.Longitude}", LogLevel.INFO);
                result = await client.Player.UpdatePlayerLocation(waypoint.Latitude, waypoint.Longitude,
                            GoogleAltitudeRequestor.GetAltitude(waypoint.Latitude, waypoint.Longitude));

                _settings.Settings.RocketSettings.DefaultLatitude = client.CurrentLatitude;
                _settings.Settings.RocketSettings.DefaultLongitude = client.CurrentLongitude;

                // Look for pokemon's nearby while walking to destination.
                var millisecondsSinceLocatePokemonWhileWalking = (DateTime.Now - locatePokemonWhileWalkingDateTime).TotalMilliseconds;
                if (searchForPokemonFunction != null && (millisecondsSinceLocatePokemonWhileWalking >= 500) && _settings.Settings.CatchPokemons)
                {
                    locatePokemonWhileWalkingDateTime = DateTime.Now;
                    await searchForPokemonFunction();
                }

                await Task.Delay(Math.Min((int)(distanceToTarget / speedInMetersPerSecond * 1000), 3000));
            } while (Navigation.CalculateDistanceInMeters(sourceLocation.Latitude, sourceLocation.Longitude, 
            targetLocation.Latitude, targetLocation.Longitude) >= 30);

            if (searchForPokemonFunction != null)
                await searchForPokemonFunction();

            return result;
        }
    }
}
