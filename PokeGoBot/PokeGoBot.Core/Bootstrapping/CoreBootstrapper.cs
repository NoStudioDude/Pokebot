using Microsoft.Practices.Unity;
using PokeGoBot.Core.Data;
using PokeGoBot.Core.Logging;
using PokeGoBot.Core.Logic;
using PokeGoBot.Core.Logic.Handlers;
using PokeGoBot.Core.Logic.Helpers;
using PokemonGo.RocketAPI.Extensions;

namespace PokeGoBot.Core.Bootstrapping
{
    public class CoreBootstrapper : UnityContainerExtension
    {
        protected override void Initialize()
        {
            Container.RegisterType<ISettingsHandler, SettingsHandler>(new ContainerControlledLifetimeManager());
            Container.RegisterType<ILogger, Logger>(new ContainerControlledLifetimeManager());
            
            Container.RegisterType<IApiFailureStrategy, ApiStrategyHandler>();

            Container.RegisterType<IPokemonHelper, PokemonHelper>();
            Container.RegisterType<IWalkingHandler, WalkingHandler>(new ContainerControlledLifetimeManager());
            Container.RegisterType<IPokemonItems, PokemonItems>(new ContainerControlledLifetimeManager());
            Container.RegisterType<ICatchPokemonHandler, CatchPokemonHandler>(new ContainerControlledLifetimeManager());
            Container.RegisterType<IPokestopsHandler, PokestopsHandler>(new ContainerControlledLifetimeManager());
            Container.RegisterType<ITransferPokemonHandler, TransferPokemonHandler>(new ContainerControlledLifetimeManager());
            Container.RegisterType<IRecycleItemsHandler, RecycleItemsHandler>(new ContainerControlledLifetimeManager());
            Container.RegisterType<IEvolvePokemonHandler, EvolvePokemonHandler>(new ContainerControlledLifetimeManager());

            Container.RegisterType<IGoBot, GoBot>(new ContainerControlledLifetimeManager());
        }
    }
}
