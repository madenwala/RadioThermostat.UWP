using AppFramework.Core;
using RadioThermostat.Api.Models;
using RadioThermostat.Core.ViewModels;
using System.Linq;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace RadioThermostat.UI.Views
{
    public abstract class ThermostatProgramViewBase : ViewBase<ThermostatProgramViewModel>
    {
    }

    public sealed partial class ThermostatProgramView : ThermostatProgramViewBase
    {
        public ThermostatProgramView()
        {
            this.InitializeComponent();
        }

        protected override async Task OnLoadStateAsync(LoadStateEventArgs e)
        {
            if (e.NavigationEventArgs.NavigationMode == NavigationMode.New || this.ViewModel == null)
                this.SetViewModel(new ThermostatProgramViewModel(e.Parameter?.ToString()));

            await base.OnLoadStateAsync(e);
        }

        protected override void OnLoaded(RoutedEventArgs e)
        {
            pivot.Focus(FocusState.Programmatic);
            base.OnLoaded(e);
        }

        private void btnCopy_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            var btn = sender as Button;
            var currentProgramDay = btn?.DataContext as ProgramDayModel;
            var sp = btn?.Parent as StackPanel;
            var lst = sp?.Children.FirstOrDefault(f => f is ListBox) as ListBox;

            var pi = pivot.SelectedItem as PivotItem;
            var currentProgram = pi?.DataContext as ProgramModel;

            if (lst != null && currentProgramDay != null)
            {               
                for (int i = 0; i < lst.Items.Count; i++)
                {
                    var item = lst.Items[i] as ListBoxItem;
                    if (item.IsSelected)
                        currentProgram.CopyProgram(currentProgramDay.Day, (Days)i);
                }
            }

            (btn?.Tag as Flyout)?.Hide();
        }
    }
}