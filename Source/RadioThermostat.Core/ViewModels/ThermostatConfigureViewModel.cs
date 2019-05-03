using AppFramework.Core.Commands;
using AppFramework.Core.Models;
using System;
using System.Threading;
using System.Threading.Tasks;
using Windows.ApplicationModel;

namespace RadioThermostat.Core.ViewModels
{
    public partial class ThermostatConfigureViewModel : ViewModelBase
    {
        #region Properties

        public override string Title
        {
            get { return $"{this.DisplayName} Configuration Info"; }
            protected set { base.Title = value; }
        }

        public CommandBase DeleteCommand { get; private set; }

        private string _IPAddress;
        public string IPAddress
        {
            get { return _IPAddress; }
            private set
            {
                if (this.SetProperty(ref _IPAddress, value))
                {
                    this.NotifyPropertyChanged(() => this.DisplayName);
                    this.NotifyPropertyChanged(() => this.Title);
                }
            }
        }

        public string DisplayName
        {
            get
            {
                return this.ThermostatNameTask.Result?.Name ?? this.IPAddress;
            }
        }

        private NotifyTaskCompletion<RadioThermostat.Api.Models.ThermostatName> _ThermostatNameTask;
        public NotifyTaskCompletion<RadioThermostat.Api.Models.ThermostatName> ThermostatNameTask
        {
            get { return _ThermostatNameTask; }
            private set { this.SetProperty(ref _ThermostatNameTask, value); }
        }

        private NotifyTaskCompletion<RadioThermostat.Api.Models.ThermostatModel> _ModelTask;
        public NotifyTaskCompletion<RadioThermostat.Api.Models.ThermostatModel> ModelTask
        {
            get { return _ModelTask; }
            private set { this.SetProperty(ref _ModelTask, value); }
        }

        private NotifyTaskCompletion<RadioThermostat.Api.Models.SystemInfo> _SystemTask;
        public NotifyTaskCompletion<RadioThermostat.Api.Models.SystemInfo> SystemTask
        {
            get { return _SystemTask; }
            private set { this.SetProperty(ref _SystemTask, value); }
        }

        private NotifyTaskCompletion<RadioThermostat.Api.Models.Network> _NetworkTask;
        public NotifyTaskCompletion<RadioThermostat.Api.Models.Network> NetworkTask
        {
            get { return _NetworkTask; }
            private set { this.SetProperty(ref _NetworkTask, value); }
        }

        #endregion

        #region Constructors

        public ThermostatConfigureViewModel(string ipAddress = null)
        {
            if (DesignMode.DesignModeEnabled)
                return;

            this.RequiresAuthorization = false;

            this.Title = Strings.Resources.TextNotApplicable;
            this.IPAddress = ipAddress;
            this.DeleteCommand = new GenericCommand("DeleteThermostatCommand", async () => await this.DeleteThermostatAsync(this.DisplayName, this.IPAddress));
            
            this.ThermostatNameTask = new NotifyTaskCompletion<Api.Models.ThermostatName>(async (ct) => await this.GetThermostatNameAsync(ct));
            this.ThermostatNameTask.SuccessfullyCompleted += (o, e) =>
            {
                this.NotifyPropertyChanged(() => this.Title);
                this.NotifyPropertyChanged(() => this.DisplayName);
            };
            this.NetworkTask = new NotifyTaskCompletion<Api.Models.Network>(async (ct) => await this.GetNetworkAsync(ct));
            this.ModelTask = new NotifyTaskCompletion<Api.Models.ThermostatModel>(async (ct) => await this.GetModelAsync(ct));
            this.SystemTask = new NotifyTaskCompletion<Api.Models.SystemInfo>(async (ct) => await this.GetSystemAsync(ct));
        }

        #endregion

        #region Methods

        protected override async Task OnRefreshAsync(bool forceRefresh, CancellationToken ct)
        {
            if (string.IsNullOrWhiteSpace(this.IPAddress))
                throw new UserFriendlyException("No IP Address was requested to be displayed.");

            this.ShowBusyStatus(Strings.Resources.TextRetievingSettings);

            this.ThermostatNameTask.Refresh(this.ThermostatNameTask.IsFaulted || this.UserForcedRefresh, CancellationToken.None);
            this.ModelTask.Refresh(this.ModelTask.IsFaulted || this.UserForcedRefresh, CancellationToken.None);
            this.SystemTask.Refresh(this.SystemTask.IsFaulted || this.UserForcedRefresh, CancellationToken.None);
            this.NetworkTask.Refresh(this.NetworkTask.IsFaulted || this.UserForcedRefresh, CancellationToken.None);

            await this.WaitAllAsync(ct, this.ThermostatNameTask.Task, this.ModelTask.Task, this.SystemTask.Task, this.NetworkTask.Task);
        }



        private async Task<Api.Models.ThermostatName> GetThermostatNameAsync(CancellationToken ct)
        {
            using (var client = new RadioThermostat.Api.ThermostatClient(this.IPAddress, ApiLogger.Current))
            {
                return await client.GetName(ct);
            }
        }

        private async Task<Api.Models.ThermostatModel> GetModelAsync(CancellationToken ct)
        {
            using (var client = new RadioThermostat.Api.ThermostatClient(this.IPAddress, ApiLogger.Current))
            {
                return await client.GetModelInfo(ct);
            }
        }

        private async Task<Api.Models.SystemInfo> GetSystemAsync(CancellationToken ct)
        {
            using (var client = new RadioThermostat.Api.ThermostatClient(this.IPAddress, ApiLogger.Current))
            {
                return await client.GetSystemInfo(ct);
            }
        }

        private async Task<Api.Models.Network> GetNetworkAsync(CancellationToken ct)
        {
            using (var client = new RadioThermostat.Api.ThermostatClient(this.IPAddress, ApiLogger.Current))
            {
                return await client.GetNetworkInfo(ct);
            }
        }

        #endregion
    }

    public partial class ThermostatConfigureViewModel
    {
        /// <summary>
        /// Self-reference back to this ViewModel. Used for designtime datacontext on pages to reference itself with the same "ViewModel" accessor used 
        /// by x:Bind and it's ViewModel property accessor on the View class. This allows you to do find-replace on views for 'Binding' to 'x:Bind'.
        [Newtonsoft.Json.JsonIgnore()]
        [System.Runtime.Serialization.IgnoreDataMember()]
        public ThermostatConfigureViewModel ViewModel { get { return this; } }
    }
}

namespace RadioThermostat.Core.ViewModels.Designer
{
    public sealed class ThermostatConfigureViewModel : RadioThermostat.Core.ViewModels.ThermostatConfigureViewModel
    {
        public ThermostatConfigureViewModel()
            : base()
        {
        }
    }
}