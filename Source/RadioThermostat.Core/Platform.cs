using AppFramework.Core;
using RadioThermostat.Core.Models;
using RadioThermostat.Core.Services;
using RadioThermostat.Core.ViewModels;

namespace RadioThermostat.Core
{
    /// <summary>
    /// Singleton object which holds instances to all the services in this application.
    /// Also provides core app functionality for initializing and suspending your application,
    /// handling exceptions, and more.
    /// </summary>
    public sealed partial class Platform : PlatformBase<ShellViewModel, AppSettingsLocal, AppSettingsRoaming, WebViewModel>
    {
        #region Properties

        /// <summary>
        /// Provides access to application services.
        /// </summary>
        public static Platform Current { get { return PlatformBase.CurrentCore as Platform; } private set { PlatformBase.CurrentCore = value; } }

        /// <summary>
        /// Gets access to the app info service of the platform currently executing.
        /// </summary>
        public AppInfoProvider AppInfo
        {
            get { return GetService<AppInfoProvider>(); }
            private set { SetService(value); }
        }

        #endregion

        #region Constructors

        static Platform()
        {
            Current = new Platform();
        }

        private Platform()
        {
            // Instantiate all the application services.
            this.AppInfo = new AppInfoProvider();
        }

        #endregion
    }

    internal class ApiLogger : RadioThermostat.Api.Models.ILogger
    {
        public static ApiLogger Current { get; private set; }
        static ApiLogger()
        {
            Current = new ApiLogger();
        }

        private ApiLogger()
        {
        }

        public void Log(string message)
        {
            Platform.Current.Logger.Log(LogLevels.Debug, "API: " + message);
        }
    }
}