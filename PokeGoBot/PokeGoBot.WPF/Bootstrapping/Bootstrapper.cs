using Microsoft.Practices.Unity;
using PokeGoBot.WPF.Bootstrapping.Extensions;

namespace PokeGoBot.WPF.Bootstrapping
{
    public class Bootstrapper : UnityContainerExtension
    {
        protected override void Initialize()
        {
            Container.AddNewExtension<HandlersExtension>();
            Container.AddNewExtension<BotExtensions>();

            Container.AddNewExtension<ViewModelExtension>();
        }
    }
}
