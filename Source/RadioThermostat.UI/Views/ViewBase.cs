namespace RadioThermostat.UI.Views
{
    /// <summary>
    /// Base class for all pages in this app.
    /// </summary>
    /// <typeparam name="TViewModel"></typeparam>
    public abstract class ViewBase<TViewModel> : AppFramework.UI.Views.ViewBase<TViewModel> where TViewModel : AppFramework.Core.ViewModels.ViewModelBase
    {
    }
}