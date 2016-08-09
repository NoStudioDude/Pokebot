using Microsoft.Practices.Unity;
using PokeGoBot.Core.Bootstrapping;
using PokeGoBot.WPF.Bootstrapping.Extensions;

namespace PokeGoBot.WPF.Bootstrapping
{
    public class Bootstrapper : UnityContainerExtension
    {
        protected override void Initialize()
        {
            Container.AddNewExtension<CoreBootstrapper>();
            Container.AddNewExtension<UtilsExtension>();
            Container.AddNewExtension<ViewModelExtension>();
        }
    }
}
