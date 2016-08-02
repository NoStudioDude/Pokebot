using System.Windows.Input;
using PokeGoBot.Core.Logging;
using Prism.Commands;
using Prism.Mvvm;

namespace PokeGoBot.WPF.Viewmodels
{
    public class MainViewModel : BindableBase
    {
        public ILogger Logger { get; set; }
        public IConfigurationViewModel ConfigurationViewModel { get; }
        public IGeneralViewModel GeneralViewModel { get; }

        public MainViewModel(IGeneralViewModel generalViewModel, 
                             IConfigurationViewModel configurationViewModel,
                             ILogger logger)
        {
            Logger = logger;
            ConfigurationViewModel = configurationViewModel;
            GeneralViewModel = generalViewModel;
        }
    }
}
