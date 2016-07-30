using Microsoft.Practices.Unity;
using PokeGoBot.WPF.Bot;
using PokeGoBot.WPF.Bot.Handlers;
using PokeGoBot.WPF.Bot.Helpers;

namespace PokeGoBot.WPF.Bootstrapping.Extensions
{
    public class BotExtensions : UnityContainerExtension
    {
        protected override void Initialize()
        {
            Container.RegisterType<IPokemonHelper, PokemonHelper>();
            Container.RegisterType<IPokemonItems, PokemonItems>();
            Container.RegisterType<ICatchPokemonHandler, CatchPokemonHandler>();
            Container.RegisterType<IPokestopsHandler, PokestopsHandler>();
            Container.RegisterType<ITransferPokemonHandler, TransferPokemonHandler>();

            Container.RegisterType<IGoBot, GoBot>();
        }
    }
}
