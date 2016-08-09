using PokeGoBot.Core.Logging;
using PokeGoBot.WPF.Utils;
using Prism.Mvvm;

namespace PokeGoBot.WPF.Viewmodels
{
    public class MainViewModel : BindableBase
    {
        private readonly IGitVersionChecker _githubChecker;
        public IGeneralViewModel GeneralViewModel { get; }

        public MainViewModel(IGeneralViewModel generalViewModel, IGitVersionChecker githubChecker)
        {
            _githubChecker = githubChecker;
            GeneralViewModel = generalViewModel;

            _githubChecker.CheckVersion();
        }
    }
}
