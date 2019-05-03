namespace RadioThermostat.Core.ViewModels
{
    public partial class PrivacyPolicyViewModel : WebViewModel
    {
        public PrivacyPolicyViewModel() : base(new AppFramework.Core.Models.WebViewArguments("http://go.microsoft.com/fwlink/?LinkId=521839", false, Strings.Resources.ViewTitlePrivacyPolicy))
        {
        }
    }
}