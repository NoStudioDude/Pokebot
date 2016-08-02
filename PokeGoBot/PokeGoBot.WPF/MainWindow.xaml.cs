using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using MahApps.Metro.Controls;
using Microsoft.Practices.Unity;
using PokeGoBot.Core;
using PokeGoBot.WPF.Viewmodels;

namespace PokeGoBot.WPF
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : MetroWindow
    {
        private ILoginViewModel _loginViewModel;
        private IConfigurationViewModel _configurationViewModel;
        
        public MainWindow(UnityContainer container)
        {
            InitializeComponent();

            this.Title = $"Poke Go Bot v{Configuration.Version()}";

            _loginViewModel = container.Resolve<ILoginViewModel>();
            FlyoutConnet.Content = _loginViewModel;
            _loginViewModel.OnLogin += () =>
            {
                flyConnect.IsOpen = false;
            };

            _configurationViewModel = container.Resolve<IConfigurationViewModel>();
            FlyoutSettings.Content = _configurationViewModel;
            _configurationViewModel.OnSave += () =>
            {
                flySettings.IsOpen = false;
            };
        }

        
        private void MainWindow_OnClosing(object sender, CancelEventArgs e)
        {
            Environment.Exit(0);
        }

        private void ButtonConnect_OnClick(object sender, RoutedEventArgs e)
        {
            flyConnect.IsOpen = !flyConnect.IsOpen;
            flySettings.IsOpen = false;
        }

        private void ButtonSettings_OnClick(object sender, RoutedEventArgs e)
        {
            flySettings.IsOpen = !flySettings.IsOpen;
            flyConnect.IsOpen = false;
        }
    }
}
