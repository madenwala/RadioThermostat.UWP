using AppFramework.Core.Models;
using System.Collections.Generic;

namespace RadioThermostat.Core.Models
{
    /// <summary>
    /// Container class for local application settings.  Create all your local app setting properties here.
    /// </summary>
    public sealed class AppSettingsLocal : AppSettingsLocalBase
    {
    }

    /// <summary>
    /// Container class for roaming application settings.  Create all your roaming app setting properties here.
    /// </summary>
    public sealed class AppSettingsRoaming : AppSettingsRoamingBase
    {
        private List<string> _IPAddresses = new List<string>();
        public List<string> IPAddresses
        {
            get { return _IPAddresses; }
            set { this.SetProperty(ref _IPAddresses, value); }
        }

        private bool _EnableHalfDegreeChanges = false;
        public bool EnableHalfDegreeChanges
        {
            get { return _EnableHalfDegreeChanges; }
            set { this.SetProperty(ref _EnableHalfDegreeChanges, value); }
        }
    }
}