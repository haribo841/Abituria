using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows;

namespace Abituria.attachedproperites
{
    public class MonitorPasswordProperty : BaseAttachedProperty<MonitorPasswordProperty, bool>///Właściwość dołączona hasła
    {
        public override void OnValueChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            var passwordBox = (sender as PasswordBox);///Ustaw wywołującego
            if (passwordBox == null)///Upewnij sie, że to password box
                return;
            passwordBox.PasswordChanged -= PasswordBox_PasswordChanged;///Usuń jakiekolwiek poprzednie wydarzenia
            if ((bool)e.NewValue)
            {
                HasTextProperty.SetValue(passwordBox);///Ustaw wartość domyślną
                passwordBox.PasswordChanged += PasswordBox_PasswordChanged;///Jeśli wywołujący ustawi MonitorPassword jako prawda, to nasłuchuj
            }
        }
        private static void PasswordBox_PasswordChanged(object sender, RoutedEventArgs e)///Odpalone, gdy wartość hasła w password boksie się zmienia
        {
            HasTextProperty.SetValue((PasswordBox)sender);///Ustawia właściwość dołączoną
        }
    }
    public class HasTextProperty : BaseAttachedProperty<HasTextProperty, bool>///Ustawia właściwość dołączoną jeżeli wywołujący PasswordBox ma jakikolwiek tekst
    {
        public static void SetValue(DependencyObject sender)
        {
            SetValue(sender, ((PasswordBox)sender).SecurePassword.Length > 0);
        }
    }
}