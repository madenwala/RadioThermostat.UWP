namespace RadioThermostat.Core.ViewModels
{
    public partial class TermsOfServiceViewModel : WebViewModel
    {
        public TermsOfServiceViewModel() : base(new AppFramework.Core.Models.WebViewArguments("http://go.microsoft.com/fwlink/?LinkID=206977", false, Strings.Resources.ViewTitleTermsOfService))
        {
        }
    }
}