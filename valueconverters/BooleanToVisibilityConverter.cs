using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Abituria.valueconverters
{
    public class BooleanToVisibilityConverter : BaseValueConverter<BooleanToVisibilityConverter>///Konwerter wartości logicznej na typ Visibility
    {
        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)///Metoda konwertuje jeden typ na drugi
        {
            if (parameter == null)
                return (bool)value ? Visibility.Hidden : Visibility.Visible;
            else
                return (bool)value ? Visibility.Visible : Visibility.Hidden;
        }
        public override object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)///Metoda konwertuje wartość na typ źródłowy
        {
            throw new NotImplementedException();
        }
    }
}