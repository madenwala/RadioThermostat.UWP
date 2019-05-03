using AppFramework.Core;
using AppFramework.Core.Extensions;
using AppFramework.UI;
using RadioThermostat.Core;
using RadioThermostat.Core.ViewModels;
using System;
using System.Threading.Tasks;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Navigation;

namespace RadioThermostat.UI.Views
{
    public abstract class ShellViewBase : ViewBase<ShellViewModel>
    {
    }
    
    public sealed partial class ShellView : ShellViewBase
    {
        #region Constructors

        public ShellView()
        {
            this.InitializeComponent();

            SuspensionManager.RegisterFrame(bodyFrame, "ShellBodyFrame");

            this.Loaded += (sender, e) =>
            {
                Platform.Current.NotifyShellMenuToggle += Current_NotifyShellMenuToggle;
                bodyFrame.Navigated += BodyFrame_Navigated;
                this.UpdateSelectedMenuItem();
                this.Current_NotifyShellMenuToggle(null, false);
            };

            this.Unloaded += (sender, e) =>
            {
                bodyFrame.Navigated -= BodyFrame_Navigated;
                Platform.Current.NotifyShellMenuToggle -= Current_NotifyShellMenuToggle;
            };

        }

        #endregion

        #region Methods

        protected override void OnApplicationResuming()
        {
            // Set the child frame to the navigation service so that it can appropriately perform navigation of pages to the desired frame.
            this.Frame.SetChildFrame(bodyFrame);
            bodyFrame.IsMenuHidden = svMain.DisplayMode == SplitViewDisplayMode.Overlay;

            base.OnApplicationResuming();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            // Set the child frame to the navigation service so that it can appropriately perform navigation of pages to the desired frame.
            this.Frame.SetChildFrame(bodyFrame);
            bodyFrame.IsMenuHidden = svMain.DisplayMode == SplitViewDisplayMode.Overlay;

            base.OnNavigatedTo(e);
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            base.OnNavigatedFrom(e);

            // Remove the bodyFrame as the childFrame
            this.Frame.SetChildFrame(null);
            bodyFrame.IsMenuHidden = false;
        }

        protected override Task OnLoadStateAsync(LoadStateEventArgs e)
        {
            if (e.NavigationEventArgs.NavigationMode == NavigationMode.New || this.ViewModel == null)
                this.SetViewModel(Platform.Current.ViewModel);

            return base.OnLoadStateAsync(e);
        }

        /// <summary>
        /// Update the selected item in the shell navigation menu based on watching the body frame navigated event.
        /// </summary>
        private void UpdateSelectedMenuItem()
        {
            var view = bodyFrame.Content;
            if (view is SettingsView)
                btnSettings.IsChecked = true;
            else if (view is AddThermostatView)
                btnAddThermostat.IsChecked = true;
        }

        #endregion

        #region Event Handlers

        private async void BodyFrame_Navigated(object sender, NavigationEventArgs e)
        {
            // Update the selected item when a page navigation occurs in the body frame
            await this.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                this.Current_NotifyShellMenuToggle(null, false);
                this.UpdateSelectedMenuItem();
            });
        }

        protected override void OnKeyUp(KeyRoutedEventArgs e)
        {
            if (e.Key == Windows.System.VirtualKey.GamepadMenu || e.Key == Windows.System.VirtualKey.Home)
            {
                this.Current_NotifyShellMenuToggle(null, null);
                e.Handled = true;
            }
            base.OnKeyUp(e);
        }
        
        private void Current_NotifyShellMenuToggle(object sender, bool? e)
        {
            if (svMain.DisplayMode != SplitViewDisplayMode.Inline && svMain.DisplayMode != SplitViewDisplayMode.CompactInline)
            {
                if (e == null)
                    this.ViewModel.IsMenuOpen = !this.ViewModel.IsMenuOpen;
                else
                    this.ViewModel.IsMenuOpen = e.Value;
            }
        }

        #endregion

        private void ListView_ItemClick(object sender, ItemClickEventArgs e)
        {
            Platform.Current.Navigation.Model(e.ClickedItem);
        }

        private void Grid_RightTapped(object sender, RightTappedRoutedEventArgs e)
        {
            FlyoutBase.ShowAttachedFlyout((FrameworkElement)sender);
        }
    }
}