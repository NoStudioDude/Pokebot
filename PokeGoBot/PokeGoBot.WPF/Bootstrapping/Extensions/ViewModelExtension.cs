using Microsoft.Practices.Unity;
using PokeGoBot.WPF.Viewmodels;

namespace PokeGoBot.WPF.Bootstrapping.Extensions
{
    public class ViewModelExtension : UnityContainerExtension
    {
        protected override void Initialize()
        {
            Container.RegisterType<IGeneralViewModel, GeneralViewModel>();
            Container.RegisterType<IConfigurationViewModel, ConfigurationViewModel>();
            Container.RegisterType<MainViewModel>();
        }
    }
}
