using PokeGoBot.Core.Logging;

using Prism.Mvvm;

namespace PokeGoBot.WPF.Viewmodels
{
    public class MainViewModel : BindableBase
    {
        public IGeneralViewModel GeneralViewModel { get; }

        public MainViewModel(IGeneralViewModel generalViewModel)
        {
            GeneralViewModel = generalViewModel;
        }
    }
}