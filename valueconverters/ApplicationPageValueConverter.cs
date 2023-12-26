using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.IO;
using Abituria.pages;
using Abituria.datamodels;
using Abituria.controls;
using Abituria.viewmodel;

namespace Abituria.valueconverters
{
    public class ApplicationPageValueConverter : BaseValueConverter<ApplicationPageValueConverter>///Zamienia strone aplikacji na aktualną strone
    {
        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            switch ((ApplicationPage)value)///Znajdź odpowiednią strone
            {
                case ApplicationPage.Login:
                    return new LoginPage();
                case ApplicationPage.Main:
                    return new MainPage();
                case ApplicationPage.Menu:
                    return new HomeView();
                default:
                    Debugger.Break();
                    return null;
            }
        }
        public override object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}