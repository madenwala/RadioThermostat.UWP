using AppFramework.Core.Models;

namespace RadioThermostat.Core.ViewModels
{
    public interface IViewModel : AppFramework.Core.ViewModels.IViewModel
    {
        Platform Platform { get; }
    }

    public abstract partial class ViewModelBase : AppFramework.Core.ViewModels.ViewModelBase, IViewModel
    {
        #region Properties

        /// <summary>
        /// Gets access to all the platform services.
        /// </summary>
        [Newtonsoft.Json.JsonIgnore()]
        [System.Runtime.Serialization.IgnoreDataMember()]
        public Platform Platform { get { return Platform.Current; } }

        #endregion
    }

    public abstract class CollectionViewModelBase : AppFramework.Core.ViewModels.CollectionViewModelBase, IViewModel
    {
        #region Properties

        /// <summary>
        /// Gets access to all the platform services.
        /// </summary>
        [Newtonsoft.Json.JsonIgnore()]
        [System.Runtime.Serialization.IgnoreDataMember()]
        public Platform Platform { get { return Platform.Current; } }

        #endregion
    }

    public abstract class SettingsViewModelBase : AppFramework.Core.ViewModels.SettingsViewModelBase, IViewModel
    {
        #region Properties

        /// <summary>
        /// Gets access to all the platform services.
        /// </summary>
        [Newtonsoft.Json.JsonIgnore()]
        [System.Runtime.Serialization.IgnoreDataMember()]
        public Platform Platform { get { return Platform.Current; } }

        #endregion
    }

    public class WebViewModel : AppFramework.Core.ViewModels.WebViewModelBase, IViewModel
    {
        #region Properties

        /// <summary>
        /// Gets access to all the platform services.
        /// </summary>
        [Newtonsoft.Json.JsonIgnore()]
        [System.Runtime.Serialization.IgnoreDataMember()]
        public Platform Platform { get { return Platform.Current; } }

        #endregion

        #region Constructors

        public WebViewModel(WebViewArguments args) : base(args)
        {
        }

        #endregion
    }
}