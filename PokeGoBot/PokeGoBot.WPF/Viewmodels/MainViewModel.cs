using Prism.Mvvm;

namespace PokeGoBot.WPF.Viewmodels
{
    public class MainViewModel : BindableBase
    {
        public IConfigurationViewModel ConfigurationViewModel { get; }
        public IGeneralViewModel GeneralViewModel { get; }

        public MainViewModel(IGeneralViewModel generalViewModel, IConfigurationViewModel configurationViewModel)
        {
            ConfigurationViewModel = configurationViewModel;
            GeneralViewModel = generalViewModel;
        }
    }
}
