using AppFramework.Core;
using AppFramework.Core.Commands;
using AppFramework.Core.Extensions;
using AppFramework.Core.Models;
using System;
using System.Threading;
using System.Threading.Tasks;
using Windows.ApplicationModel;
using Windows.UI.Xaml;

namespace RadioThermostat.Core.ViewModels
{
    public partial class ThermostatViewModel : ViewModelBase
    {
        #region Variables

        private const int SUBMIT_CHANGES_MILLISECONDS_DELAY = 1300;
        private DispatcherTimer _statusTimer = null;

        #endregion

        #region Properties

        public override string Title
        {
            get { return this.DisplayName; }
            protected set { base.Title = value; }
        }

        public RadioThermostat.Api.Models.ThermostatStatus CurrentStatus { get; private set; }
        
        private RadioThermostat.Api.Models.ThermostatStatus _status = null;
        public RadioThermostat.Api.Models.ThermostatStatus Status
        {
            get { return _status; }
            protected set
            {
                if (this.SetProperty(ref _status, value))
                {
                    this.CurrentStatus.CopyFrom(value);
                    this.NotifyPropertyChanged(() => this.Title);
                    if(this.Status != null)
                        this.Status.PropertyChanged += Status_PropertyChanged;
                }
            }
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

        private bool _hasFailedConnection = false;
        public bool HasFailedConnection
        {
            get { return _hasFailedConnection; }
            private set{ this.SetProperty(ref _hasFailedConnection, value); }
        }

        private CommandBase _raiseTemperatureCommand;
        public CommandBase RaiseTemperatureCommand
        {
            get { return _raiseTemperatureCommand; }
            private set { this.SetProperty(ref _raiseTemperatureCommand, value); }
        }

        private CommandBase _lowerTemperatureCommand;
        public CommandBase LowerTemperatureCommand
        {
            get { return _lowerTemperatureCommand; }
            private set { this.SetProperty(ref _lowerTemperatureCommand, value); }
        }

        private NotifyTaskCompletion<string> _ProgramStatusTask;
        public NotifyTaskCompletion<string> ProgramStatusTask
        {
            get { return _ProgramStatusTask; }
            private set { this.SetProperty(ref _ProgramStatusTask, value); }
        }

        public CommandBase DeleteCommand { get; private set; }


        private bool _ShowDeleteButton;
        public bool ShowDeleteButton
        {
            get { return _ShowDeleteButton; }
            private set { this.SetProperty(ref _ShowDeleteButton, value); }
        }

        #endregion

        #region Constructors

        public ThermostatViewModel(string ipAddress = null)
        {
            if (DesignMode.DesignModeEnabled)
                return;

            this.RequiresAuthorization = false;

            this.Title = Strings.Resources.TextNotApplicable;
            this.IPAddress = ipAddress;

            this.CurrentStatus = new Api.Models.ThermostatStatus();
            this.RaiseTemperatureCommand = new GenericCommand("RaiseTemperatureCommand", () => this.RaiseTemperature());
            this.LowerTemperatureCommand = new GenericCommand("LowerTemperatureCommand", () => this.LowerTemperature());

            _statusTimer = new DispatcherTimer();
            _statusTimer.Tick += _statusTimer_Tick;

            this.ThermostatNameTask = new NotifyTaskCompletion<Api.Models.ThermostatName>(async (ct) => await this.GetThermostatNameAsync(ct));
            this.ThermostatNameTask.SuccessfullyCompleted += (o, e) =>
            {
                this.NotifyPropertyChanged(() => this.Title);
                this.NotifyPropertyChanged(() => this.DisplayName);
            };

            this.ProgramStatusTask = new NotifyTaskCompletion<string>(async (ct) => await this.GetProgramStatus(ct));
            this.DeleteCommand = new GenericCommand("DeleteThermostatCommand", async () => await this.DeleteThermostatAsync(this.DisplayName, this.IPAddress));
        }

        ~ThermostatViewModel()
        {
            try
            {
                _statusTimer?.Stop();
            }
            catch
            {

            }
            _statusTimer = null;
        }

        #endregion

        #region Methods

        protected override Task OnLoadStateAsync(LoadStateEventArgs e)
        {
            Platform.Navigation.ClearBackstack();
            return base.OnLoadStateAsync(e);
        }

        protected override async Task OnRefreshAsync(bool forceRefresh, CancellationToken ct)
        {
            if (string.IsNullOrWhiteSpace(this.IPAddress))
                throw new UserFriendlyException("No IP Address was requested to be displayed.");

            this.ShowBusyStatus(Strings.Resources.TextRetievingSettings, this.Status == null);
            this.ShowDeleteButton = false;

            this.ThermostatNameTask.Refresh(this.ThermostatNameTask.IsFaulted || this.UserForcedRefresh, CancellationToken.None);

            if (this.Status == null || forceRefresh || (this.Status.Time?.AsDateTime().AddMinutes(10) ?? DateTime.Now) < DateTime.Now )
            {
                try
                {
                    using (var client = new RadioThermostat.Api.ThermostatClient(this.IPAddress, ApiLogger.Current))
                    {
                        this.Status = await client.GetThermostatStatus(ct);
                        this.HasFailedConnection = false;
                    }
                }
                catch(Exception ex)
                {
                    this.ShowDeleteButton = this.Platform.IsXbox;
                    this.Platform.Logger.LogError(ex, $"Could not refresh {this.DisplayName} ({this.IPAddress})");
                    this.HasFailedConnection = true;
                    
                    throw new UserFriendlyException("Thermostat could not be reached. Check your network connection and try again.", Platform.IsXbox ? UserFriendlyException.DisplayStyles.NonBlockingMessage : UserFriendlyException.DisplayStyles.BlockingMessage);
                }
            }

            this.ProgramStatusTask.Refresh(this.ProgramStatusTask.IsFaulted || this.UserForcedRefresh || _needsProgramStatusUpdate || (this.Status.Time?.AsDateTime().AddMinutes(60) ?? DateTime.Now) < DateTime.Now, CancellationToken.None);
        }
        
        private async Task<Api.Models.ThermostatName> GetThermostatNameAsync(CancellationToken ct)
        {
            using (var client = new RadioThermostat.Api.ThermostatClient(this.IPAddress, ApiLogger.Current))
            {
                return await client.GetName(ct);
            }
        }

        private void RaiseTemperature()
        {
            this.Status.TargetTemperature += Platform.Current.AppSettingsRoaming.EnableHalfDegreeChanges ? .5 : 1;

            if (!Platform.Current.AppSettingsRoaming.EnableHalfDegreeChanges)
                this.Status.TargetTemperature = Math.Round(this.Status.TargetTemperature, 0);
        }

        private void LowerTemperature()
        {
            this.Status.TargetTemperature -= Platform.Current.AppSettingsRoaming.EnableHalfDegreeChanges ? .5 : 1;

            if (!Platform.Current.AppSettingsRoaming.EnableHalfDegreeChanges)
                this.Status.TargetTemperature = Math.Round(this.Status.TargetTemperature, 0);
        }

        private async Task SubmitThermostatChangesAsync()
        {
            try
            {
                this.ShowBusyStatus("Sending changes to thermostat...");

                _statusTimer.Stop();
                _changesCTS = new CancellationTokenSource();
                
                using (var client = new RadioThermostat.Api.ThermostatClient(this.IPAddress, ApiLogger.Current))
                {
                    this.Status = await client.SetThermostatStatus(this.Status, _changesCTS.Token);

                    if (_needsProgramStatusUpdate)
                        this.ProgramStatusTask.Refresh(true, CancellationToken.None);
                }

                this.ClearStatus();
            }
            catch (OperationCanceledException)
            {
                this.ClearStatus();
            }
            catch (Exception ex)
            {
                await this.HandleExceptionAsync(ex, $"Error submitted changes to '{this.DisplayName}' thermostat");
            }
            finally
            {
                this.ClearChangesToken();
            }
        }

        private void ClearChangesToken(bool cancel = false)
        {
            if (_changesCTS != null)
            {
                if (cancel)
                    _changesCTS.Cancel();
                _changesCTS.Dispose();
                _changesCTS = null;
            }
        }

        private async Task<string> GetProgramStatus(CancellationToken ct)
        {
            _needsProgramStatusUpdate = false;

            if (this.Status == null)
                return null;
            else if (this.Status.Hold)
                return "Hold maintains your current target temperature. Your schedule will not run while Hold is enabled.";

            try
            {
                using (var client = new RadioThermostat.Api.ThermostatClient(this.IPAddress, ApiLogger.Current))
                {
                    RadioThermostat.Api.Models.ProgramModel program = null;

                    if (this.Status.Mode == Api.Models.ThermostatModes.Heat)
                        program = await client.GetProgramHeat(ct);
                    else if (this.Status.Mode == Api.Models.ThermostatModes.Cool)
                        program = await client.GetProgramCool(ct);

                    return this.GetNextProgramChange(program);
                }
            }
            catch(Exception ex)
            {
                Platform.Current.Logger.LogError(ex, "Failure during GetProgramStatus");
                return null;
            }
        }

        private string GetNextProgramChange(RadioThermostat.Api.Models.ProgramModel program)
        {
            if (program == null)
                return null;

            var data = program.GetNextProgramChange(this.Status.Time.AsDateTime());

            if (data != null)
                return string.Format("At {0} the target will be set to {1}º", data.Item1, data.Item2);
            else
                return null;
        }

        #endregion

        #region Events

        private CancellationTokenSource _changesCTS = null;

        private async void _statusTimer_Tick(object sender, object e)
        {
            await this.SubmitThermostatChangesAsync();
        }

        private void Status_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            this.ClearChangesToken(true);
            _statusTimer.Stop();
            int ms = SUBMIT_CHANGES_MILLISECONDS_DELAY;
            if (e.PropertyName == nameof(this.Status.Mode))
                ms = 100;
            _statusTimer.Interval = TimeSpan.FromMilliseconds(ms);
            _statusTimer.Start();

            if (e.PropertyName == nameof(this.Status.Hold)
                || e.PropertyName == nameof(this.Status.Mode))
                _needsProgramStatusUpdate = true;

        }

        private bool _needsProgramStatusUpdate = false;

        #endregion
    }

    public partial class ThermostatViewModel
    {
        /// <summary>
        /// Self-reference back to this ViewModel. Used for designtime datacontext on pages to reference itself with the same "ViewModel" accessor used 
        /// by x:Bind and it's ViewModel property accessor on the View class. This allows you to do find-replace on views for 'Binding' to 'x:Bind'.
        [Newtonsoft.Json.JsonIgnore()]
        [System.Runtime.Serialization.IgnoreDataMember()]
        public ThermostatViewModel ViewModel { get { return this; } }
    }
}

namespace RadioThermostat.Core.ViewModels.Designer
{
    public sealed class ThermostatViewModel : RadioThermostat.Core.ViewModels.ThermostatViewModel
    {
        public ThermostatViewModel()
            : base()
        {
        }
    }
}