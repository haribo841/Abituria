using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Markup;

namespace Abituria.valueconverters
{
    public abstract class BaseValueConverter<T> : MarkupExtension, IValueConverter where T : class, new()///Bazowy konwerter wartości pozwala na bezpośrednie użycie XAML
    {
        private static T Converter = null;///Pojedyńcza statyczna instancja tego konwertera wartości
        public override object ProvideValue(IServiceProvider serviceProvider)///Rozszerzenie metody znacznika
        {
            return Converter ??= new T();///Jeżeli prywatny członek konwertera ma wartość null, to tworzymy instancje typu generycznego
        }
        public abstract object Convert(object value, Type targetType, object parameter, CultureInfo culture);///Metoda konwertuje jeden typ na drugi
        public abstract object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture);///Metoda konwertuje wartość na typ źródłowy
    }
}