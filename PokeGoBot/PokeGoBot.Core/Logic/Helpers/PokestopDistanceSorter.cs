using System.Collections.Generic;
using System.Linq;
using PokeGoBot.Core.Data.Poco;
using POGOProtos.Map.Fort;

namespace PokeGoBot.Core.Logic.Helpers
{
    public class PokestopDistanceSorter
    {
        public static List<PokestopPoco> SortByDistance(IEnumerable<FortData> pokestopsData, double clientLatitude, double clientLongitude, 
            double playerMaxTravelInMeters)
        {
            List<PokestopPoco> poco = new List<PokestopPoco>();

            foreach (var pokeStop in pokestopsData)
            {
                var distance = Navigation.CalculateDistanceInMeters(clientLatitude, clientLongitude,
                    pokeStop.Latitude, pokeStop.Longitude);

                poco.Add(new PokestopPoco()
                {
                    Distance =  distance,
                    Latitude = pokeStop.Latitude,
                    Longitude = pokeStop.Longitude,
                    Id = pokeStop.Id
                });
            }

            var result = poco.Where(d => d.Distance <= playerMaxTravelInMeters).ToList();
            return result.OrderBy(f => f.Distance).ToList(); ;
        }
    }
}
