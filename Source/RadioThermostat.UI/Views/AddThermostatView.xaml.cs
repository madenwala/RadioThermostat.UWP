using AppFramework.Core;
using RadioThermostat.Core.ViewModels;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Navigation;

namespace RadioThermostat.UI.Views
{
    public abstract class AddThermostatViewBase : ViewBase<AddThermostatViewModel>
    {
    }

    public sealed partial class AddThermostatView : AddThermostatViewBase
    {
        public AddThermostatView()
        {
            this.InitializeComponent();
        }

        protected override async Task OnLoadStateAsync(LoadStateEventArgs e)
        {
            if (e.NavigationEventArgs.NavigationMode == NavigationMode.New || this.ViewModel == null)
                this.SetViewModel(new AddThermostatViewModel());

#if DEBUG
            this.ViewModel.IPAddress = "24.12.144.130:10";
#endif

            await base.OnLoadStateAsync(e);
        }

        protected override void OnLoaded(RoutedEventArgs e)
        {
            btnBeginSearch.Focus(FocusState.Programmatic);
            base.OnLoaded(e);
        }
    }
}