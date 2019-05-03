using AppFramework.Core.Models;
using RadioThermostat.Core;
using RadioThermostat.Core.Services;
using RadioThermostat.Core.ViewModels;
using RadioThermostat.UI.Views;
using System;
using System.Linq;

namespace RadioThermostat.UI.Services
{
    /// <summary>
    /// NavigationManager instance with implementations specific to this application.
    /// </summary>
    public sealed partial class NavigationManager : NavigationManagerBase
    {
        #region Navigation Methods

        public override void Home(object parameter = null)
        {
            //if(Platform.Current.AuthManager.IsAuthenticated() == false)
            //{
            //    this.Navigate(this.ParentFrame, typeof(WelcomeView), parameter);
            //    this.ClearBackstack();
            //}
            //else
            //{
                // User is authenticated
                if (this.ParentFrame.Content == null || !(this.ParentFrame.Content is ShellView))
                {
                    this.Navigate(this.ParentFrame, typeof(ShellView), null);
                    this.ClearBackstack();
                }
                else
                {
                    this.Frame.Content = null;
                    this.Frame.BackStack.Clear();
                    if (Platform.Current.ViewModel.Thermostats.Count > 0)
                        this.Thermostat(Platform.Current.ViewModel.Thermostats.First());
                    else
                        this.AddThermostat(null);
                    this.ClearBackstack();
                }
            //}
        }

        public override void Model(object parameter)
        {
            if (parameter == null)
                throw new ArgumentNullException(nameof(parameter));

            if (parameter is ThermostatViewModel)
                this.Thermostat(parameter);
            else
                throw new NotImplementedException("Navigation not implemented for type " + parameter.GetType().Name);
        }

        public override void Settings(object parameter = null)
        {
            this.Navigate(typeof(SettingsView), parameter);
        }

        public override void Thermostat(object parameter)
        {
            if (parameter is ThermostatViewModel)
                parameter = (parameter as ThermostatViewModel).IPAddress;

            if (this.IsChildFramePresent)
                this.Navigate(typeof(ThermostatView), parameter);
            else
                this.Home(new NavigationRequest(typeof(ThermostatView), parameter));
        }

        public override void ThermostatConfigure(object parameter)
        {
            this.Navigate(this.Frame, typeof(ThermostatConfigureView), parameter);
        }

        public override void AddThermostat(object parameter)
        {
            this.Navigate(this.Frame, typeof(AddThermostatView), parameter);
        }

        public override void ThermostatProgram(object parameter)
        {
            this.Navigate(this.Frame, typeof(ThermostatProgramView), parameter);
        }

        public override void Phone(object model)
        {
            throw new NotImplementedException();
        }

        public override void PrivacyPolicy(object parameter = null)
        {
            this.Settings(SettingsViews.PrivacyPolicy);
        }

        public override void TermsOfService(object parameter = null)
        {
            this.Settings(SettingsViews.TermsOfService);
        }

        protected override void SecondaryWindow(NavigationRequest request)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}