using AppFramework.Core.Commands;
using AppFramework.Core.Models;
using System;
using System.Threading;
using System.Threading.Tasks;
using Windows.ApplicationModel;

namespace RadioThermostat.Core.ViewModels
{
    public partial class ThermostatProgramViewModel : ViewModelBase
    {
        #region Properties

        public override string Title
        {
            get { return $"Program for {this.DisplayName}"; }
            protected set { base.Title = value; }
        }

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

        private NotifyTaskCompletion<RadioThermostat.Api.Models.ProgramModel> _ProgramHeatTask;
        public NotifyTaskCompletion<RadioThermostat.Api.Models.ProgramModel> ProgramHeatTask
        {
            get { return _ProgramHeatTask; }
            private set { this.SetProperty(ref _ProgramHeatTask, value); }
        }

        private NotifyTaskCompletion<RadioThermostat.Api.Models.ProgramModel> _ProgramCoolTask;
        public NotifyTaskCompletion<RadioThermostat.Api.Models.ProgramModel> ProgramCoolTask
        {
            get { return _ProgramCoolTask; }
            private set { this.SetProperty(ref _ProgramCoolTask, value); }
        }

        public CommandBase SaveCommand { get; private set; }

        #endregion

        #region Constructors

        public ThermostatProgramViewModel(string ipAddress = null)
        {
            if (DesignMode.DesignModeEnabled)
                return;

            this.RequiresAuthorization = false;

            this.Title = Strings.Resources.TextNotApplicable;
            this.IPAddress = ipAddress;

            this.ThermostatNameTask = new NotifyTaskCompletion<Api.Models.ThermostatName>(async (ct) => await this.GetThermostatNameAsync(ct));
            this.ThermostatNameTask.SuccessfullyCompleted += (o, e) =>
            {
                this.NotifyPropertyChanged(() => this.Title);
                this.NotifyPropertyChanged(() => this.DisplayName);
            };
            this.ProgramHeatTask = new NotifyTaskCompletion<Api.Models.ProgramModel>(async (ct) => await this.GetProgramHeatAsync(ct));
            this.ProgramCoolTask = new NotifyTaskCompletion<Api.Models.ProgramModel>(async (ct) => await this.GetProgramCoolAsync(ct));

            this.SaveCommand = new GenericCommand("SaveProgramCommand", async () => await this.SaveProgramsAsync());
        }

        #endregion

        #region Methods

        private async Task SaveProgramsAsync()
        {
            try
            {
                using (var client = new RadioThermostat.Api.ThermostatClient(this.IPAddress, ApiLogger.Current))
                {
                    this.ShowBusyStatus("Saving heat program...", true);
                    await client.SetProgramHeat(this.ProgramHeatTask.Result, CancellationToken.None);
                    this.ShowBusyStatus("Saving cool program...", true);
                    await client.SetProgramCool(this.ProgramCoolTask.Result, CancellationToken.None);
                }

                Platform.Navigation.GoBack();
            }
            finally
            {
                this.ClearStatus();
            }
        }
        
        protected override async Task OnRefreshAsync(bool forceRefresh, CancellationToken ct)
        {
            if (string.IsNullOrWhiteSpace(this.IPAddress))
                throw new UserFriendlyException("No IP Address was requested to be displayed.", UserFriendlyException.DisplayStyles.BlockingMessage);

            this.ShowBusyStatus("Retrieving heat and cool program settings...", true);

            try
            {
                this.ThermostatNameTask.Refresh(this.ThermostatNameTask.IsFaulted || this.UserForcedRefresh, ct);
                this.ProgramHeatTask.Refresh(this.ProgramHeatTask.IsFaulted || this.UserForcedRefresh, ct);
                this.ProgramCoolTask.Refresh(this.ProgramCoolTask.IsFaulted || this.UserForcedRefresh, ct);

                await this.WaitAllAsync(ct, this.ThermostatNameTask.Task, this.ProgramHeatTask.Task, this.ProgramCoolTask.Task);
            }
            catch
            {
                throw new UserFriendlyException("Could not retrieve program settings. Try again later.", UserFriendlyException.DisplayStyles.NonBlockingMessage);
            }
        }

        private async Task<Api.Models.ThermostatName> GetThermostatNameAsync(CancellationToken ct)
        {
            using (var client = new RadioThermostat.Api.ThermostatClient(this.IPAddress, ApiLogger.Current))
            {
                return await client.GetName(ct);
            }
        }

        private async Task<Api.Models.ProgramModel> GetProgramHeatAsync(CancellationToken ct)
        {
            using (var client = new RadioThermostat.Api.ThermostatClient(this.IPAddress, ApiLogger.Current))
            {
                return await client.GetProgramHeat(ct);
            }
        }

        private async Task<Api.Models.ProgramModel> GetProgramCoolAsync(CancellationToken ct)
        {
            using (var client = new RadioThermostat.Api.ThermostatClient(this.IPAddress, ApiLogger.Current))
            {
                return await client.GetProgramCool(ct);
            }
        }

        #endregion
    }

    /// <summary>
    /// 
    /// </summary>
    public partial class ThermostatProgramViewModel
    {
        /// <summary>
        /// Self-reference back to this ViewModel. Used for designtime datacontext on pages to reference itself with the same "ViewModel" accessor used 
        /// by x:Bind and it's ViewModel property accessor on the View class. This allows you to do find-replace on views for 'Binding' to 'x:Bind'.
        /// </summary>
        [Newtonsoft.Json.JsonIgnore()]
        [System.Runtime.Serialization.IgnoreDataMember()]
        public ThermostatProgramViewModel ViewModel { get { return this; } }
    }
}

namespace RadioThermostat.Core.ViewModels.Designer
{
    public sealed class ThermostatProgramViewModel : RadioThermostat.Core.ViewModels.ThermostatProgramViewModel
    {
        public ThermostatProgramViewModel()
        {
        }
    }
}