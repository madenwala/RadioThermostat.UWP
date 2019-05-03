using RadioThermostat.Core;
using RadioThermostat.Core.ViewModels;
using System.Threading.Tasks;
using Windows.UI.Xaml.Navigation;
using System.Linq;
using Windows.UI.Xaml;
using AppFramework.Core;

namespace RadioThermostat.UI.Views
{
    public abstract class ThermostatConfigureViewBase : ViewBase<ThermostatConfigureViewModel>
    {
    }

    public sealed partial class ThermostatConfigureView : ThermostatConfigureViewBase
    {
        public ThermostatConfigureView()
        {
            this.InitializeComponent();
            //this.NavigationCacheMode = NavigationCacheMode.Disabled;
        }
        
        protected override Task OnLoadStateAsync(LoadStateEventArgs e)
        {
            if (e.NavigationEventArgs.NavigationMode == NavigationMode.New || this.ViewModel == null)
                this.SetViewModel(new ThermostatConfigureViewModel(e.Parameter?.ToString()));

            return base.OnLoadStateAsync(e);
        }

        protected override void OnLoaded(RoutedEventArgs e)
        {
            txtIPAddress.Focus(FocusState.Programmatic);
            base.OnLoaded(e);
        }

        private void ToggleMenuFlyoutItem_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {

        }
    }
}
