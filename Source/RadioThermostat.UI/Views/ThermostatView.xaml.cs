using AppFramework.Core;
using RadioThermostat.Core;
using RadioThermostat.Core.ViewModels;
using System.Linq;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Navigation;

namespace RadioThermostat.UI.Views
{
    public abstract class ThermostatViewBase : ViewBase<ThermostatViewModel>
    {
    }

    public sealed partial class ThermostatView : ThermostatViewBase
    {

        public ThermostatView()
        {
            this.InitializeComponent();
            this.SizeChanged += ThermostatView_SizeChanged;
            //this.NavigationCacheMode = NavigationCacheMode.Disabled;
        }

        private void ThermostatView_SizeChanged(object sender, Windows.UI.Xaml.SizeChangedEventArgs e)
        {
            controlsRow.MaxHeight = e.NewSize.Height / 4;
            controlsRow.Height = new Windows.UI.Xaml.GridLength(e.NewSize.Width / 4);
        }

        protected override Task OnLoadStateAsync(LoadStateEventArgs e)
        {
            if (e.NavigationEventArgs.NavigationMode == NavigationMode.New || this.ViewModel == null)
            {
                var vm = Platform.Current.ViewModel.Thermostats.FirstOrDefault(f => f.IPAddress.Equals(e.Parameter?.ToString(), System.StringComparison.CurrentCultureIgnoreCase));
                this.SetViewModel(vm);
            }

            return base.OnLoadStateAsync(e);
        }

        protected override void OnLoaded(RoutedEventArgs e)
        {
            btnTempUp.Focus(FocusState.Programmatic);
            base.OnLoaded(e);
        }
    }
}