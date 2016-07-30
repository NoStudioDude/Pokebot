using Microsoft.Practices.Unity;
using PokeGoBot.WPF.Handlers;

namespace PokeGoBot.WPF.Bootstrapping.Extensions
{
    public class HandlersExtension : UnityContainerExtension
    {
        protected override void Initialize()
        {
            Container.RegisterType<ISettingsHandler, SettingsHandler>(new ContainerControlledLifetimeManager());
        }
    }
}
