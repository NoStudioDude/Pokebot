using Microsoft.Practices.Unity;
using PokeGoBot.WPF.Bootstrapping.Extensions;
using PokeGoBot.WPF.Logging;

namespace PokeGoBot.WPF.Bootstrapping
{
    public class Bootstrapper : UnityContainerExtension
    {
        protected override void Initialize()
        {
            Container.RegisterType<ILogger, Logger>(new ContainerControlledLifetimeManager());

            Container.AddNewExtension<HandlersExtension>();
            Container.AddNewExtension<BotExtensions>();

            Container.AddNewExtension<ViewModelExtension>();
        }
    }
}
