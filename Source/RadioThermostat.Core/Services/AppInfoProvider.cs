using RadioThermostat.Core.Strings;

namespace RadioThermostat.Core.Services
{
    /// <summary>
    /// Base class providing access to the application currently executing specific to the platform this app is executing on.
    /// </summary>
    public sealed partial class AppInfoProvider : AppFramework.Core.Services.AppInfoProviderBase
    {
        #region Properties

        public override string AppName => Resources.ApplicationName;
        public override string AppDescription => Resources.ApplicationDescription;
        public override string AppSupportEmailAddress => Resources.ApplicationSupportEmailAddress;
        public override string AppSupportTwitterAddress => Resources.ApplicationSupportTwitterUsername;
        public override string ProtocolPrefix => "thermostat://";

        #endregion
    }
}