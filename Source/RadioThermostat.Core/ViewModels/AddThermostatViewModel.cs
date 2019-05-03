using AppFramework.Core;
using AppFramework.Core.Commands;
using RadioThermostat.Core.SSDP;
using System;
using System.Linq;
using System.Threading.Tasks;
using Windows.ApplicationModel;

namespace RadioThermostat.Core.ViewModels
{
    public partial class AddThermostatViewModel : ViewModelBase
    {
        #region Variables

        const string WM_DISCOVERMessage = "TYPE: WM-DISCOVER\r\nVERSION: 1.0\r\n\r\nservices: com.marvell.wm.system*\r\n\r\n";
        private ISsdp _ssdp;

        #endregion

        #region Properties

        /// <summary>
        /// Gets the title to be displayed on the view consuming this ViewModel.
        /// </summary>
        public override string Title
        {
            get { return Strings.Resources.ViewTitleAddThermostat; }
        }

        private string _IPAddress;
        public string IPAddress
        {
            get { return _IPAddress; }
            set { this.SetProperty(ref _IPAddress, value); }
        }

        public bool IsSearching
        {
            get { return _ssdp != null; }
        }

        private bool _CanSearch = true;
        public bool CanSearch
        {
            get { return _CanSearch; }
            private set { if (this.SetProperty(ref _CanSearch, value)) this.BeginSearchCommand.RaiseCanExecuteChanged(); }
        }

        public CommandBase AddCommand { get; private set; }

        public CommandBase BeginSearchCommand { get; private set; }

        private int _devicesAdded = 0;

        private string _SearchStatus;
        public string SearchStatus
        {
            get { return _SearchStatus; }
            private set { this.SetProperty(ref _SearchStatus, value); }
        }

        #endregion

        #region Constructors

        public AddThermostatViewModel()
        {
            if (DesignMode.DesignModeEnabled)
                return;

            this.RequiresAuthorization = false;
            this.AddCommand = new GenericCommand("AddThermostatCommand", async () => await this.AddThermostatAsync());
            this.BeginSearchCommand = new GenericCommand("BeginSearchCommand", this.BeginSearch, ()=>this.CanSearch);
        }

        #endregion

        #region Methods

        protected override Task OnSaveStateAsync(SaveStateEventArgs e)
        {
            if(this.IsSearching)
                this.EndSearch();
            return base.OnSaveStateAsync(e);
        }

        private async Task AddThermostatAsync()
        {
            try
            {
                this.ShowBusyStatus("Adding thermostat...", true);
                if (!string.IsNullOrWhiteSpace(this.IPAddress))
                {
                    ThermostatViewModel vm = this.CreateThermostatViewModel(this.IPAddress);
                    await vm.RefreshAsync(true);
                    Platform.Current.Navigation.Thermostat(vm);
                }
                else
                    Platform.Navigation.Home();
            }
            finally
            {
                this.ClearStatus();
            }
        }

        private ThermostatViewModel CreateThermostatViewModel(string ipAddress)
        {
            ThermostatViewModel vm = Platform.Current.ViewModel.Thermostats.FirstOrDefault(f => f.IPAddress == ipAddress);
            if (vm == null)
            {
                Platform.Current.Logger.Log(LogLevels.Warning, "CreateThermostatViewModel_Added {0}", ipAddress);
                Platform.Current.Analytics.Event("CreateThermostatViewModel_Added", ipAddress);
                Platform.Current.AppSettingsRoaming.IPAddresses.Add(ipAddress);
                Platform.Current.SaveSettings();
                vm = new ThermostatViewModel(ipAddress);
                Platform.Current.ViewModel.Thermostats.Add(vm);
            }
            else
            {
                Platform.Current.Logger.Log(LogLevels.Warning, "CreateThermostatViewModel_AlreadyExists {0}", ipAddress);
                Platform.Current.Analytics.Event("CreateThermostatViewModel_AlreadyExists", ipAddress);
            }

            return vm;
        }

        private void BeginSearch()
        {
            Platform.Current.Logger.Log(LogLevels.Warning, "SearchThermostats_Start");
            Platform.Current.Analytics.Event("SearchThermostats_Start");
            this.ShowBusyStatus("Searching for thermostats...");
            this.SearchStatus = "Searching...";
            _devicesAdded = 0;
            _ssdp = new Ssdp(WM_DISCOVERMessage);
            this.NotifyPropertyChanged(() => this.IsSearching);
            this.CanSearch = false;
            _ssdp.DeviceFound += new EventHandler<DeviceFoundEventArgs>(SSDP_DeviceFound);
            _ssdp.SearchStopped += SSDP_SearchStopped;
            _ssdp.StartSearch();
        }

        private void SSDP_SearchStopped(object sender, SearchStoppedEventArgs e)
        {
            Platform.Current.Logger.Log(LogLevels.Warning, "SearchThermostats_End");
            Platform.Current.Analytics.Event("SearchThermostats_End");
            this.InvokeOnUIThread(this.EndSearch);
        }

        private void EndSearch()
        {
            if (_devicesAdded > 0)
                this.SearchStatus = $"DONE...added {_devicesAdded} device(s)!";
            else
                this.SearchStatus = "No devices found. Ensure that WiFi is on, that you're connected to the same network as your thermostat, and that your device isn't connected to a VPN client currently.";
            
            this.ClearStatus();
            //_ssdp?.StopSearch(SearchStoppedReason.Aborted);
            _ssdp?.Cleanup();
            _ssdp?.Dispose();
            _ssdp = null;
            this.CanSearch = true;
            this.NotifyPropertyChanged(() => this.IsSearching);
        }

        public override void Dispose()
        {
            if (_ssdp != null)
            {
                _ssdp.Dispose();
                _ssdp = null;
            }
            base.Dispose();
        }

        #endregion

        #region Events

        private void SSDP_DeviceFound(object sender, DeviceFoundEventArgs e)
        {
            string location = string.Empty;
            string service = string.Empty;

            if (!e.Results.TryGetValue("SERVICE", out service))
                return;

            if (service.StartsWith("com.marvell.wm") && e.Results.TryGetValue("LOCATION", out location))
            {
                location = location.Replace("http://", "").Replace("/sys/", "");
                Platform.Current.Logger.Log(LogLevels.Warning, "SearchThermostats_Found {0}", location);
                Platform.Current.Analytics.Event("SearchThermostats_Found", location);

                this.InvokeOnUIThread(async () =>
                {
                    try
                    {
                        ThermostatViewModel vm = Platform.Current.ViewModel.Thermostats.FirstOrDefault(f => f.IPAddress == location);

                        if (vm == null)
                        {
                            vm = this.CreateThermostatViewModel(location);
                            _devicesAdded++;
                            await vm.RefreshAsync(true);
                            Platform.Current.Logger.Log(LogLevels.Warning, "SearchThermostats_Added {0}", location);
                            Platform.Current.Analytics.Event("SearchThermostats_Added", location);
                        }
                        else
                        {
                            Platform.Current.Logger.Log(LogLevels.Warning, "SearchThermostats_AlreadyExists {0}", location);
                            Platform.Current.Analytics.Event("SearchThermostats_AlreadyExists", location);
                        }

                        if (_devicesAdded > 0)
                            this.SearchStatus = $"Searching...devices found: {_devicesAdded}";
                    }
                    catch(Exception ex)
                    {
                        await this.HandleExceptionAsync(ex, $"Error while trying to add new thermostat at {location}");
                    }
                });
            }
        }

        #endregion
    }

    /// <summary>
    /// 
    /// </summary>
    public partial class AddThermostatViewModel
    {
        /// <summary>
        /// Self-reference back to this ViewModel. Used for designtime datacontext on pages to reference itself with the same "ViewModel" accessor used 
        /// by x:Bind and it's ViewModel property accessor on the View class. This allows you to do find-replace on views for 'Binding' to 'x:Bind'.
        /// </summary>
        [Newtonsoft.Json.JsonIgnore()]
        [System.Runtime.Serialization.IgnoreDataMember()]
        public AddThermostatViewModel ViewModel { get { return this; } }
    }
}

namespace RadioThermostat.Core.ViewModels.Designer
{
    public sealed class AddThermostatViewModel : RadioThermostat.Core.ViewModels.AddThermostatViewModel
    {
        public AddThermostatViewModel()
        {
        }
    }
}