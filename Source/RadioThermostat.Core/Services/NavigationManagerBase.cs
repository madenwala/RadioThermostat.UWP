using AppFramework.Core.Commands;
using RadioThermostat.Core.Services;

namespace RadioThermostat.Core
{
    public partial class Platform
    {
        /// <summary>
        /// Gets the ability to navigate to different parts of an application specific to the platform currently executing.
        /// </summary>
        public NavigationManagerBase Navigation
        {
            get { return GetService<NavigationManagerBase>(); }
            set { SetService(value); }
        }
    }
}

namespace RadioThermostat.Core.Services
{
    public abstract partial class NavigationManagerBase : AppFramework.Core.Services.NavigationManagerBase
    {
        #region Abstract Methods

        public abstract void Thermostat(object parameter);

        public abstract void AddThermostat(object parameter);

        public abstract void ThermostatConfigure(object parameter);

        public abstract void ThermostatProgram(object parameter);

        private CommandBase _navigateToAddThermostatCommand = null;
        public CommandBase NavigateToAddThermostatCommand
        {
            get { return _navigateToAddThermostatCommand ?? (_navigateToAddThermostatCommand = new NavigationCommand("NavigateToAddThermostatCommand", Platform.Current.Navigation.AddThermostat)); }
        }

        private CommandBase _navigateThermostatConfigureCommand = null;
        public CommandBase NavigateToThermostatConfigureCommand
        {
            get { return _navigateThermostatConfigureCommand ?? (_navigateThermostatConfigureCommand = new NavigationCommand("NavigateToThermostatConfigureCommand", Platform.Current.Navigation.ThermostatConfigure)); }
        }

        private CommandBase _navigateThermostatProgramCommand = null;
        public CommandBase NavigateToThermostatProgramCommand
        {
            get { return _navigateThermostatProgramCommand ?? (_navigateThermostatProgramCommand = new NavigationCommand("NavigateToThermostatProgramCommand", Platform.Current.Navigation.ThermostatProgram)); }
        }

        #endregion
    }
}