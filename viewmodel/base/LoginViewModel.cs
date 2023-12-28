using Abituria.viewmodel.;
using Abituria.viewmodel;
using PropertyChanged;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Abituria
{
    [ImplementPropertyChanged]
    public class LoginViewModel : BaseViewModel///Model widoku dla niestandardowego okna
    {
        public string UserName { get; set; }///Nazwa użytkownika
        public bool LoginIsRunning { get; set; }///Flaga wskazująca, czy proces Login trwa
        public SecureString Password { get; set; }///Hasło użytkownika, set; nie działa
        public ICommand LoginCommand { get; set; }///Komenda do logowania
        public ICommand GoToMenuPage { get; set; }///Komenda przechodzi do strony MenuPage
        public LoginViewModel()///Standardowy konstruktor
        {
            LoginCommand = new RelayParametrizedCommand((parameter) => Login(parameter));///Tworzenie komend
        }
        public async Task Login(object parameter)///Próba zalogowania użytkownika
        {
            // if (LoginIsRunning)
            //     return;
            //LoginIsRunning = true;
            //try
            //{
            //    throw new ArgumentNullException();
            await RunCommand(() => this.LoginIsRunning, async () =>//;
            {
                await Task.Delay(500);///Przekazuje SecureString
                var userName = this.UserName;
                var pass = (parameter as IHavePassword).SecurePassword.Unsecure();///ZMIENIĆ! Nigdy nie powinno się przewchowywać hasła w zmiennej!
            });
            //}
            //catch { }
            //finally
            //{
            //    LoginIsRunning = false;
            //}
        }
    }
}