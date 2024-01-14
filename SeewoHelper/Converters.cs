using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Media;

namespace SeewoHelper
{
    public class AQIToBrushConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var aqiValue = (double)value;
            int r = 0, g = 0, b = 0;
            if (0 <= aqiValue && aqiValue <= 60)
            {
                r = GetColorValue(0, 255, aqiValue);
                g = GetColorValue(153, 222, aqiValue);
                b = GetColorValue(102, 51, aqiValue);
            }
            if (60 < aqiValue && aqiValue <= 120)
            {
                r = 255;
                g = GetColorValue(222, 153, aqiValue-60);
                b = 51;
            }
            if (120 < aqiValue && aqiValue <= 180)
            {
                r = GetColorValue(255, 204, aqiValue-120);
                g = GetColorValue(153, 0, aqiValue-120);
                b = 51;
            }
            if (180 < aqiValue && aqiValue <= 240)
            {
                r = GetColorValue(204, 102, aqiValue-180);
                g = 0;
                b = GetColorValue(51, 153, aqiValue-180);
            }
            if (240 < aqiValue && aqiValue <= 300)
            {
                r = GetColorValue(102, 126, aqiValue-240);
                g = 0;
                b = GetColorValue(153, 35, aqiValue-240);
            }
            if (300 < aqiValue)
            {
                r = 126;
                g = 0;
                b = 35;
            }
            return new SolidColorBrush(Color.FromRgb(
                System.Convert.ToByte(r),
                System.Convert.ToByte(g),
                System.Convert.ToByte(b)));
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        private int GetColorValue(double colorValueStart, double colorValueEnd, double value, double range = 60)
        {
            return (int)(value / range * (colorValueEnd - colorValueStart) + colorValueStart);
        }
    }
}
