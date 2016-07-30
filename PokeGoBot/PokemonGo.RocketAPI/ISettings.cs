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
        bool CatchPokemons { get; set; }
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
        public bool CatchPokemons { get; set; }
    }
}
