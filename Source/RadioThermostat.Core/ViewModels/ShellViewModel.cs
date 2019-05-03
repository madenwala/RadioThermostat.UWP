using AppFramework.Core;
using AppFramework.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Windows.ApplicationModel;
using Windows.UI.Xaml.Navigation;

namespace RadioThermostat.Core.ViewModels
{
    public partial class ShellViewModel : ViewModelBase
    {
        #region Properties

        public string WelcomeMessage
        {
            get
            {
                return Strings.Account.TextWelcomeUnauthenticated;
            }
        }

        private bool _IsMenuOpen = true;
        public bool IsMenuOpen
        {
            get { return _IsMenuOpen; }
            set { this.SetProperty(ref _IsMenuOpen, value); }
        }

        private ModelList<ThermostatViewModel> _Thermostats = new ModelList<ThermostatViewModel>();
        public ModelList<ThermostatViewModel> Thermostats
        {
            get { return _Thermostats; }
            private set { this.SetProperty(ref _Thermostats, value); }
        }

        #endregion

        #region Constructors

        public ShellViewModel()
        {
            this.Title = Strings.Resources.ViewTitleWelcome;

            if (DesignMode.DesignModeEnabled)
                return;

            this.RequiresAuthorization = false;
        }

        #endregion Constructors

        #region Methods

        protected override Task OnLoadStateAsync(LoadStateEventArgs e)
        {
            if (!this.IsInitialized)
            {
                foreach (var ip in Platform.Current.AppSettingsRoaming.IPAddresses)
                    if(!this.Thermostats.Any(a=>a.IPAddress == ip))
                        this.Thermostats.Add(new ThermostatViewModel(ip));
            }

            // If the view parameter contains any navigation requests, forward on to the global navigation service
            if (e.NavigationEventArgs.NavigationMode == NavigationMode.New && e.Parameter is NavigationRequest)
                Platform.Current.Navigation.Navigate(e.Parameter as NavigationRequest);
            else
                Platform.Current.Navigation.Home();

            return base.OnLoadStateAsync(e);
        }

        protected override async Task OnRefreshAsync(bool forceRefresh, CancellationToken ct)
        {
            if (forceRefresh)
            {
                this.ShowBusyStatus(Strings.Resources.TextLoading, this.Thermostats == null || this.Thermostats.Count == 0);

                var tasks = new List<Task>();
                foreach (var t in this.Thermostats)
                    tasks.Add(t.RefreshAsync(forceRefresh));
                await Task.WhenAll(tasks);
            }
        }
        
        protected override async Task OnUserAuthenticatedChanged()
        {
            if (this.RequiresAuthorization && this.IsUserAuthenticated == false)
                await this.UserSignoutAsync(true);
        }

        public void RemoveThermostat(string ipAddress)
        {
            while(Platform.Current.AppSettingsRoaming.IPAddresses.Contains(ipAddress))
                Platform.Current.AppSettingsRoaming.IPAddresses.Remove(ipAddress);
            Platform.Current.SaveSettings();
            foreach (var vm in this.Thermostats.Where(w => w.IPAddress.Equals(ipAddress, StringComparison.CurrentCultureIgnoreCase)).ToArray())
                this.Thermostats.Remove(vm);
        }

        #endregion
    }

    public partial class ShellViewModel
    {
        /// <summary>
        /// Self-reference back to this ViewModel. Used for designtime datacontext on pages to reference itself with the same "ViewModel" accessor used 
        /// by x:Bind and it's ViewModel property accessor on the View class. This allows you to do find-replace on views for 'Binding' to 'x:Bind'.
        [Newtonsoft.Json.JsonIgnore()]
        [System.Runtime.Serialization.IgnoreDataMember()]
        public ShellViewModel ViewModel { get { return this; } }
    }
}

namespace RadioThermostat.Core.ViewModels.Designer
{
    public sealed class ShellViewModel : RadioThermostat.Core.ViewModels.ShellViewModel
    {
        public ShellViewModel()
        {
        }
    }
}