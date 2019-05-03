using System;
using System.Threading;
using System.Threading.Tasks;

namespace RadioThermostat.Core.ViewModels
{
    public partial class ViewModelBase
    {
        protected async Task DeleteThermostatAsync(string displayName, string ipAddress)
        {
            try
            {
                var result = await this.ShowMessageBoxAsync(
                    CancellationToken.None,
                    string.Format(Strings.Resources.TextPromptDeleteThermostatMessage, displayName),
                    Strings.Resources.TextPromptDeleteThermostatTitle,
                    new string[] { Strings.Resources.TextYes, Strings.Resources.TextNo });

                if (result == 0)
                {
                    Platform.Current.ViewModel.RemoveThermostat(ipAddress);
                    Platform.Current.Navigation.Home();
                }
            }
            catch (Exception ex)
            {
                await this.HandleExceptionAsync(ex, $"Error deleting '{displayName}' thermostat");
            }
        }
    }
}