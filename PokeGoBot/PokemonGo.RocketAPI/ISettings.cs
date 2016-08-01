using System.Collections.Generic;
using PokemonGo.RocketAPI.Enums;
using POGOProtos.Inventory.Item;

namespace PokemonGo.RocketAPI
{
    public interface ISettings
    {
        AuthType AuthType { get; set; }
        double DefaultLatitude { get; set; }
        double DefaultLongitude { get; set; }
        double DefaultAltitude { get; set; }
        string GoogleRefreshToken { get; set; }
        string Username { get; set; }
        string Password { get; set; }
        ICollection<KeyValuePair<ItemId, int>> ItemRecycleFilter { get; set; }

        bool TransferDuplicates { get; set; }
        double KeepMinCp { get; set; }
        bool EvolvePokemon { get; set; }
        bool UseLuckyEgg { get; set; }
        bool UseIncense { get; set; }

        bool UpdateLocation { get; set; }
        double PlayerWalkingSpeed { get; set; }
        int DelayBetweenActions { get; set; }
        double PlayerMaxTravelInMeters { get; set; }
        bool FarmPokestops { get; set; }
        bool CatchPokemons { get; set; }
        bool ReciclyItems { get; set; }
        int IvPercentageDiscart { get; set; }
        bool QuickTransfer { get; set; }

        int MaxPokeballs { get; set; }
        int MaxGreatballs { get; set; }
        int MaxUltraballs { get; set; }
        int MaxMasterballs { get; set; }
        int MaxRevives { get; set; }
        int MaxTopRevives { get; set; }
        int MaxPotions { get; set; }
        int MaxSuperPotions { get; set; }
        int MaxHyperPotions { get; set; }
        int MaxTopPotions { get; set; }
        int MaxBerrys { get; set; }

    }

    public class Settings : ISettings
    {
        public AuthType AuthType { get; set; }
        public double DefaultLatitude { get; set; }
        public double DefaultLongitude { get; set; }
        public double DefaultAltitude { get; set; }
        public string GoogleRefreshToken { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public ICollection<KeyValuePair<ItemId, int>> ItemRecycleFilter { get; set; }
        public bool FarmPokestops { get; set; }
        public bool CatchPokemons { get; set; }
        public bool ReciclyItems { get; set; }
        public int IvPercentageDiscart { get; set; }
        public bool QuickTransfer { get; set; }
        public int MaxPokeballs { get; set; }
        public int MaxGreatballs { get; set; }
        public int MaxUltraballs { get; set; }
        public int MaxMasterballs { get; set; }
        public int MaxRevives { get; set; }
        public int MaxTopRevives { get; set; }
        public int MaxPotions { get; set; }
        public int MaxSuperPotions { get; set; }
        public int MaxHyperPotions { get; set; }
        public int MaxTopPotions { get; set; }
        public int MaxBerrys { get; set; }
        public bool TransferDuplicates { get; set; }
        public double KeepMinCp { get; set; }
        public bool EvolvePokemon { get; set; }
        public bool UseLuckyEgg { get; set; }
        public bool UseIncense { get; set; }
        public bool UpdateLocation { get; set; }
        public double PlayerWalkingSpeed { get; set; }
        public int DelayBetweenActions { get; set; }
        public double PlayerMaxTravelInMeters { get; set; }
    }
}
