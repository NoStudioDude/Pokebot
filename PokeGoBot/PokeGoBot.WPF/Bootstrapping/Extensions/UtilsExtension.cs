using Microsoft.Practices.Unity;
using PokeGoBot.WPF.Utils;

namespace PokeGoBot.WPF.Bootstrapping.Extensions
{
    public class UtilsExtension : UnityContainerExtension
    {
        protected override void Initialize()
        {
            Container.RegisterType<IGitVersionChecker, GitVersionChecker>();
        }
    }
}
