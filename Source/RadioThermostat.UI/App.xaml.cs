using AppFramework.Core.Services.Analytics;
using RadioThermostat.Core;
using RadioThermostat.UI.Services;

namespace RadioThermostat.UI
{
    /// <summary>
    /// Provides application-specific behavior to supplement the default Application class.
    /// </summary>
    sealed partial class App : AppFramework.UI.App
    {
        #region Constructor

        /// <summary>
        /// Initializes the singleton application object.  This is the first line of authored code
        /// executed, and as such is the logical equivalent of main() or WinMain().
        /// </summary>
        public App()
        {
            this.InitializeComponent();
            
            // Initalize the platform object which is the singleton instance to access various services
            Platform.Current.Navigation = new NavigationManager();
            Platform.Current.Analytics.Register(new FlurryAnalyticsService("Y7F7Q9Q3VTWKVXS5BC6Y"));
            Platform.Current.Analytics.Register(new HockeyAppService("a6c9e0b4339643718ed8e98d58f0ee9f", "adenwala@outlook.com"));
        }

        #endregion
    }
}