using Microsoft.Practices.Unity;
using PokeGoBot.Core.Data;
using PokeGoBot.Core.Logging;
using PokeGoBot.Core.Logic;
using PokeGoBot.Core.Logic.Handlers;
using PokeGoBot.Core.Logic.Helpers;

namespace PokeGoBot.Core.Bootstrapping
{
    public class CoreBootstrapper : UnityContainerExtension
    {
        protected override void Initialize()
        {
            Container.RegisterType<ILogger, Logger>(new ContainerControlledLifetimeManager());
            Container.RegisterType<ISettingsHandler, SettingsHandler>(new ContainerControlledLifetimeManager());

            Container.RegisterType<IPokemonHelper, PokemonHelper>();
            Container.RegisterType<IWalkingHandler, WalkingHandler>();
            Container.RegisterType<IPokemonItems, PokemonItems>();
            Container.RegisterType<ICatchPokemonHandler, CatchPokemonHandler>();
            Container.RegisterType<IPokestopsHandler, PokestopsHandler>();
            Container.RegisterType<ITransferPokemonHandler, TransferPokemonHandler>();
            Container.RegisterType<IRecycleItemsHandler, RecycleItemsHandler>();
            Container.RegisterType<IEvolvePokemonHandler, EvolvePokemonHandler>();

            Container.RegisterType<IGoBot, GoBot>();
        }
    }
}
