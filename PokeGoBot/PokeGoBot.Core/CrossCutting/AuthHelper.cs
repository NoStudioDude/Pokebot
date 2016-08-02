using PokemonGo.RocketAPI.Enums;

namespace PokeGoBot.Core.CrossCutting
{
    public class AuthHelper
    {
        public static bool IsGoogleAuth(int auth)
        {
            return (AuthType) auth == AuthType.Google;
        }
    }
}
