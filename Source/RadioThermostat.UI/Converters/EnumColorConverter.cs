using System;
using Windows.UI;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Media;

namespace RadioThermostat.UI.Converters
{
    public sealed class BackgroundColorConverter : IValueConverter
    {
        public SolidColorBrush ModeOffBrush { get; set; }
        public SolidColorBrush ModeHeatBrush { get; set; }
        public SolidColorBrush ModeCoolBrush { get; set; }
        public SolidColorBrush ModeAutoBrush { get; set; }

        public SolidColorBrush ModeFanOnBrush { get; set; }

        public SolidColorBrush HoldOnBrush { get; set; }


        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value is RadioThermostat.Api.Models.ThermostatModes)
            {
                switch((RadioThermostat.Api.Models.ThermostatModes)value)
                {
                    case Api.Models.ThermostatModes.Off: return this.ModeOffBrush;
                    case Api.Models.ThermostatModes.Heat: return this.ModeHeatBrush;
                    case Api.Models.ThermostatModes.Cool: return this.ModeCoolBrush;
                    case Api.Models.ThermostatModes.Auto: return this.ModeAutoBrush;
                }
            }
            else if(value is RadioThermostat.Api.Models.FanOperatingModes)
            {
                switch ((RadioThermostat.Api.Models.FanOperatingModes)value)
                {
                    case Api.Models.FanOperatingModes.Auto: return this.ModeOffBrush;
                    default: return this.ModeFanOnBrush;
                }
            }
            else if(value is bool)
            {
                if ((bool)value == true)
                    return this.HoldOnBrush;
                else
                    return this.ModeOffBrush;
            }

            return this.ModeOffBrush;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }

    public sealed class ForgroundColorConverter : IValueConverter
    {
        public SolidColorBrush OffBrush { get; set; }
        public SolidColorBrush OnBrush { get; set; }


        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value is RadioThermostat.Api.Models.ThermostatModes)
            {
                switch ((RadioThermostat.Api.Models.ThermostatModes)value)
                {
                    case Api.Models.ThermostatModes.Off: return this.OffBrush;
                    default: return this.OnBrush;
                }
            }
            else if (value is RadioThermostat.Api.Models.FanOperatingModes)
            {
                switch ((RadioThermostat.Api.Models.FanOperatingModes)value)
                {
                    case Api.Models.FanOperatingModes.Auto: return this.OffBrush;
                    default: return this.OnBrush;
                }
            }
            else if (value is bool)
            {
                if ((bool)value == true)
                    return this.OnBrush;
                else
                    return this.OffBrush;
            }

            return this.OffBrush;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
