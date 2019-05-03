namespace RadioThermostat.Core.ViewModels
{
    public partial class GeneralSettingsViewModel : SettingsViewModelBase
    {
        #region Constructors

        public GeneralSettingsViewModel()
        {
            this.Title = Strings.Resources.TextTitleGeneral;
        }

        #endregion
    }

    public partial class GeneralSettingsViewModel
    {
        /// <summary>
        /// Self-reference back to this ViewModel. Used for designtime datacontext on pages to reference itself with the same "ViewModel" accessor used 
        /// by x:Bind and it's ViewModel property accessor on the View class. This allows you to do find-replace on views for 'Binding' to 'x:Bind'.
        [Newtonsoft.Json.JsonIgnore()]
        [System.Runtime.Serialization.IgnoreDataMember()]
        public GeneralSettingsViewModel ViewModel { get { return this; } }
    }
}

namespace RadioThermostat.Core.ViewModels.Designer
{
    public sealed class GeneralSettingsViewModel : RadioThermostat.Core.ViewModels.GeneralSettingsViewModel
    {
        public GeneralSettingsViewModel()
        {
        }
    }
}