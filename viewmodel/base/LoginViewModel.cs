using System;
using System.Windows;
using System.Windows.Input;
using System.ComponentModel;
using System.Runtime.InteropServices;
using PropertyChanged;
using Abituria.pages;
using Abituria.viewmodel;
using System.Windows.Controls;
using System.Security;
using System.Threading.Tasks;
using System.Runtime.CompilerServices;
using Abituria.datamodels;
using Abituria.security;

namespace Abituria
{
    [ImplementPropertyChanged]
    public class LoginViewModel : BaseViewModel///Model widoku dla niestandardowego okna
    {
        private static string uName;
        public string UserName///Nazwa użytkownika
        {
            get { return uName; }
            set { uName = value; }
        }
        public string Greeting { get; } = "Dzień dobry, " + uName;
        public bool LoginIsRunning { get; set; }///Flaga wskazująca, czy proces Login trwa
        public SecureString Password { get; set; }///Hasło użytkownika, set; nie działa
        public ICommand LoginCommand { get; set; }///Komenda do logowania
        public ICommand GoToLoginPage { get; set; }///Komenda przechodzi do strony logowania
        public ICommand GoToRegisterPage { get; set; }///Komenda przechodzi do strony RegisterPage
        public ICommand GoToMenuPage { get; set; }///Komenda przechodzi do strony MenuPage
        public ICommand GoToFinalExamPage { get; set; }///Komenda przechodzi do strony MenuPage
        public ICommand GoToCalculatorPage { get; set; }///Komenda przechodzi do strony MenuPage
        public ICommand GoToChaptersPage { get; set; }///Komenda przechodzi do strony MenuPage
        public ICommand GoToExercisesPage { get; set; }///Komenda przechodzi do strony MenuPage
        public ICommand GoToEquationsPage { get; set; }///Komenda przechodzi do strony MenuPage
        public ICommand GoToQuadraticPage { get; set; }
        public ICommand GoToVectorsPage { get; set; }
        public ICommand GoToFER21Page { get; set; }
        public ICommand GoToE1Page { get; set; }
        public ICommand GoToE2Page { get; set; }
        public ICommand GoToE3Page { get; set; }
        public ICommand GoToE4Page { get; set; }
        public ICommand GoToE5Page { get; set; }
        public ICommand GoToE6Page { get; set; }
        public ICommand GoToE7Page { get; set; }
        public ICommand GoToE8Page { get; set; }
        public ICommand GoToE9Page { get; set; }
        public ICommand GoToE10Page { get; set; }
        public ICommand GoToE11Page { get; set; }
        public ICommand GoToE12Page { get; set; }
        public ICommand GoToE13Page { get; set; }
        public ICommand GoToE14Page { get; set; }
        public ICommand GoToE15Page { get; set; }
        public ICommand GoToE16Page { get; set; }
        public ICommand GoToE17Page { get; set; }
        public ICommand GoToE18Page { get; set; }
        public ICommand GoToE19Page { get; set; }
        public ICommand GoToE20Page { get; set; }
        public ICommand GoToE21Page { get; set; }
        public ICommand GoToE22Page { get; set; }
        public ICommand GoToE23Page { get; set; }
        public ICommand GoToE24Page { get; set; }
        public ICommand GoToE25Page { get; set; }
        public ICommand GoToE26Page { get; set; }
        public ICommand GoToE27Page { get; set; }
        public ICommand GoToE28Page { get; set; }
        public ICommand GoToE29Page { get; set; }
        public ICommand GoToE30Page { get; set; }
        public ICommand GoToE31Page { get; set; }
        public ICommand GoToE32Page { get; set; }
        public ICommand GoToE33Page { get; set; }
        public ICommand GoToE34Page { get; set; }
        public ICommand GoToE35Page { get; set; }
        public ICommand GoToEqPage { get; set; }///Komenda przechodzi do strony Equations
        public ICommand GoToEq2Page { get; set; }///Komenda przechodzi do strony Equations
        public ICommand GoToEq3Page { get; set; }///Komenda przechodzi do strony Equations
        public ICommand GoToEq4Page { get; set; }///Komenda przechodzi do strony Wzorye
        public ICommand GoToEq5Page { get; set; }///Komenda przechodzi do strony 
        public ICommand GoToEq6Page { get; set; }
        public ICommand GoToEq7Page { get; set; }
        public ICommand GoToEq8Page { get; set; }
        public ICommand GoToEq9Page { get; set; }
        public ICommand GoToEq10Page { get; set; }
        public ICommand GoToEq11Page { get; set; }
        public ICommand GoToEq12Page { get; set; }
        public ICommand GoToEq13Page { get; set; }
        public ICommand GoToEq14Page { get; set; }
        public ICommand GoToEq15Page { get; set; }
        public ICommand GoToEq16Page { get; set; }
        public ICommand GoToEq17Page { get; set; }
        public ICommand GoToEq18Page { get; set; }
        public LoginViewModel()///Standardowy konstruktor
        {
            LoginCommand = new RelayParametrizedCommand((parameter) => Login(parameter));///Tworzenie komend
            GoToLoginPage = new RelayCommand(() => LoginP());
            GoToRegisterPage = new RelayCommand(() => Register());
            GoToMenuPage = new RelayCommand(() => MainMenu());
            GoToFinalExamPage = new RelayCommand(() => FinalExam());
            GoToCalculatorPage = new RelayCommand(() => Calculator());
            GoToChaptersPage = new RelayCommand(() => Chapters());
            GoToExercisesPage = new RelayCommand(() => Exercises());
            GoToEquationsPage = new RelayCommand(() => Equations());
            GoToQuadraticPage = new RelayCommand(() => Quadratic());
            GoToVectorsPage = new RelayCommand(() => Vectors());
            GoToFER21Page = new RelayCommand(() => FER21());
            GoToE1Page = new RelayCommand(() => E1());
            GoToE2Page = new RelayCommand(() => E2());
            GoToE3Page = new RelayCommand(() => E3());
            GoToE4Page = new RelayCommand(() => E4());
            GoToE5Page = new RelayCommand(() => E5());
            GoToE6Page = new RelayCommand(() => E6());
            GoToE7Page = new RelayCommand(() => E7());
            GoToE8Page = new RelayCommand(() => E8());
            GoToE9Page = new RelayCommand(() => E9());
            GoToE10Page = new RelayCommand(() => E10());
            GoToE11Page = new RelayCommand(() => E11());
            GoToE12Page = new RelayCommand(() => E12());
            GoToE13Page = new RelayCommand(() => E13());
            GoToE14Page = new RelayCommand(() => E14());
            GoToE15Page = new RelayCommand(() => E15());
            GoToE16Page = new RelayCommand(() => E16());
            GoToE17Page = new RelayCommand(() => E17());
            GoToE18Page = new RelayCommand(() => E18());
            GoToE19Page = new RelayCommand(() => E19());
            GoToE20Page = new RelayCommand(() => E20());
            GoToE21Page = new RelayCommand(() => E21());
            GoToE22Page = new RelayCommand(() => E22());
            GoToE23Page = new RelayCommand(() => E23());
            GoToE24Page = new RelayCommand(() => E24());
            GoToE25Page = new RelayCommand(() => E25());
            GoToE26Page = new RelayCommand(() => E26());
            GoToE27Page = new RelayCommand(() => E27());
            GoToE28Page = new RelayCommand(() => E28());
            GoToE29Page = new RelayCommand(() => E29());
            GoToE30Page = new RelayCommand(() => E30());
            GoToE31Page = new RelayCommand(() => E31());
            GoToE32Page = new RelayCommand(() => E32());
            GoToE33Page = new RelayCommand(() => E33());
            GoToE34Page = new RelayCommand(() => Z34());
            GoToE35Page = new RelayCommand(() => E35());
            GoToEqPage = new RelayCommand(() => Eq());
            GoToEq2Page = new RelayCommand(() => Eq2());
            GoToEq3Page = new RelayCommand(() => Eq3());
            GoToEq4Page = new RelayCommand(() => Eq4());
            GoToEq5Page = new RelayCommand(() => Eq5());
            GoToEq6Page = new RelayCommand(() => Eq6());
            GoToEq7Page = new RelayCommand(() => Eq7());
            GoToEq8Page = new RelayCommand(() => Eq8());
            GoToEq9Page = new RelayCommand(() => Eq9());
            GoToEq10Page = new RelayCommand(() => Eq10());
            GoToEq11Page = new RelayCommand(() => Eq11());
            GoToEq12Page = new RelayCommand(() => Eq12());
            GoToEq13Page = new RelayCommand(() => Eq13());
            GoToEq14Page = new RelayCommand(() => Eq14());
            GoToEq15Page = new RelayCommand(() => Eq15());
            GoToEq16Page = new RelayCommand(() => Eq16());
            GoToEq17Page = new RelayCommand(() => Eq17());
            GoToEq18Page = new RelayCommand(() => Eq18());
        }
        public async Task Login(object parameter)///Próba zalogowania użytkownika
        {
            await RunCommand(() => this.LoginIsRunning, async () =>
            {
                await Task.Delay(500);///Przekazuje SecureString
                var userName = this.UserName;
                var pass = (parameter as IHavePassword).SecurePassword.Unsecure();///ZMIENIĆ! Nigdy nie powinno się przewchowywać hasła w zmiennej!
                MainMenu();
            });
        }
        private void Register()
        {
            ((WindowViewModel)((MainWindow)Application.Current.MainWindow).DataContext).CurrentPage = ApplicationPage.Register;
        }
        private void MainMenu()
        {
            ((WindowViewModel)((MainWindow)Application.Current.MainWindow).DataContext).CurrentPage = ApplicationPage.Main;
        }
        private void LoginP()
        {
            ((WindowViewModel)((MainWindow)Application.Current.MainWindow).DataContext).CurrentPage = ApplicationPage.Login;
        }
        private void FinalExam()
        {
            ((WindowViewModel)((MainWindow)Application.Current.MainWindow).DataContext).CurrentPage = ApplicationPage.FinalExam;
        }
        private void Calculator()
        {
            ((WindowViewModel)((MainWindow)Application.Current.MainWindow).DataContext).CurrentPage = ApplicationPage.Calculator;
        }
        private void Chapters()
        {
            ((WindowViewModel)((MainWindow)Application.Current.MainWindow).DataContext).CurrentPage = ApplicationPage.Chapters;
        }
        private void Exercises()
        {
            ((WindowViewModel)((MainWindow)Application.Current.MainWindow).DataContext).CurrentPage = ApplicationPage.Exercises;
        }
        private void Equations()
        {
            ((WindowViewModel)((MainWindow)Application.Current.MainWindow).DataContext).CurrentPage = ApplicationPage.Equations;
        }
        private void Quadratic()
        {
            ((WindowViewModel)((MainWindow)Application.Current.MainWindow).DataContext).CurrentPage = ApplicationPage.Quadratic;
        }

        private void Vectors()
        {
            ((WindowViewModel)((MainWindow)Application.Current.MainWindow).DataContext).CurrentPage = ApplicationPage.Vectors;
        }

        private void FER21()
        {
            ((WindowViewModel)((MainWindow)Application.Current.MainWindow).DataContext).CurrentPage = ApplicationPage.FER21;
        }

        private void E1()
        {
            ((WindowViewModel)((MainWindow)Application.Current.MainWindow).DataContext).CurrentPage = ApplicationPage.E1;
        }

        private void E2()
        {
            ((WindowViewModel)((MainWindow)Application.Current.MainWindow).DataContext).CurrentPage = ApplicationPage.E2;
        }

        private void E3()
        {
            ((WindowViewModel)((MainWindow)Application.Current.MainWindow).DataContext).CurrentPage = ApplicationPage.E3;
        }

        private void E4()
        {
            ((WindowViewModel)((MainWindow)Application.Current.MainWindow).DataContext).CurrentPage = ApplicationPage.E4;
        }


        private void E5()
        {
            ((WindowViewModel)((MainWindow)Application.Current.MainWindow).DataContext).CurrentPage = ApplicationPage.E5;
        }

        private void E6()
        {
            ((WindowViewModel)((MainWindow)Application.Current.MainWindow).DataContext).CurrentPage = ApplicationPage.E6;
        }

        private void E7()
        {
            ((WindowViewModel)((MainWindow)Application.Current.MainWindow).DataContext).CurrentPage = ApplicationPage.E7;
        }

        private void E8()
        {
            ((WindowViewModel)((MainWindow)Application.Current.MainWindow).DataContext).CurrentPage = ApplicationPage.E8;
        }

        private void E9()
        {
            ((WindowViewModel)((MainWindow)Application.Current.MainWindow).DataContext).CurrentPage = ApplicationPage.E9;
        }

        private void E10()
        {
            ((WindowViewModel)((MainWindow)Application.Current.MainWindow).DataContext).CurrentPage = ApplicationPage.E10;
        }

        private void E11()
        {
            ((WindowViewModel)((MainWindow)Application.Current.MainWindow).DataContext).CurrentPage = ApplicationPage.E11;
        }

        private void E12()
        {
            ((WindowViewModel)((MainWindow)Application.Current.MainWindow).DataContext).CurrentPage = ApplicationPage.E12;
        }

        private void E13()
        {
            ((WindowViewModel)((MainWindow)Application.Current.MainWindow).DataContext).CurrentPage = ApplicationPage.E13;
        }

        private void E14()
        {
            ((WindowViewModel)((MainWindow)Application.Current.MainWindow).DataContext).CurrentPage = ApplicationPage.E14;
        }

        private void E15()
        {
            ((WindowViewModel)((MainWindow)Application.Current.MainWindow).DataContext).CurrentPage = ApplicationPage.E15;
        }

        private void E16()
        {
            ((WindowViewModel)((MainWindow)Application.Current.MainWindow).DataContext).CurrentPage = ApplicationPage.E16;
        }

        private void E17()
        {
            ((WindowViewModel)((MainWindow)Application.Current.MainWindow).DataContext).CurrentPage = ApplicationPage.E17;
        }

        private void E18()
        {
            ((WindowViewModel)((MainWindow)Application.Current.MainWindow).DataContext).CurrentPage = ApplicationPage.E18;
        }

        private void E19()
        {
            ((WindowViewModel)((MainWindow)Application.Current.MainWindow).DataContext).CurrentPage = ApplicationPage.E19;
        }

        private void E20()
        {
            ((WindowViewModel)((MainWindow)Application.Current.MainWindow).DataContext).CurrentPage = ApplicationPage.E20;
        }

        private void E21()
        {
            ((WindowViewModel)((MainWindow)Application.Current.MainWindow).DataContext).CurrentPage = ApplicationPage.E21;
        }

        private void E22()
        {
            ((WindowViewModel)((MainWindow)Application.Current.MainWindow).DataContext).CurrentPage = ApplicationPage.E22;
        }

        private void E23()
        {
            ((WindowViewModel)((MainWindow)Application.Current.MainWindow).DataContext).CurrentPage = ApplicationPage.E23;
        }

        private void E24()
        {
            ((WindowViewModel)((MainWindow)Application.Current.MainWindow).DataContext).CurrentPage = ApplicationPage.E24;
        }

        private void E25()
        {
            ((WindowViewModel)((MainWindow)Application.Current.MainWindow).DataContext).CurrentPage = ApplicationPage.E25;
        }

        private void E26()
        {
            ((WindowViewModel)((MainWindow)Application.Current.MainWindow).DataContext).CurrentPage = ApplicationPage.E26;
        }

        private void E27()
        {
            ((WindowViewModel)((MainWindow)Application.Current.MainWindow).DataContext).CurrentPage = ApplicationPage.E27;
        }

        private void E28()
        {
            ((WindowViewModel)((MainWindow)Application.Current.MainWindow).DataContext).CurrentPage = ApplicationPage.E28;
        }

        private void E29()
        {
            ((WindowViewModel)((MainWindow)Application.Current.MainWindow).DataContext).CurrentPage = ApplicationPage.E29;
        }

        private void E30()
        {
            ((WindowViewModel)((MainWindow)Application.Current.MainWindow).DataContext).CurrentPage = ApplicationPage.E30;
        }

        private void E31()
        {
            ((WindowViewModel)((MainWindow)Application.Current.MainWindow).DataContext).CurrentPage = ApplicationPage.E31;
        }

        private void E32()
        {
            ((WindowViewModel)((MainWindow)Application.Current.MainWindow).DataContext).CurrentPage = ApplicationPage.E32;
        }

        private void E33()
        {
            ((WindowViewModel)((MainWindow)Application.Current.MainWindow).DataContext).CurrentPage = ApplicationPage.E33;
        }

        private void Z34()
        {
            ((WindowViewModel)((MainWindow)Application.Current.MainWindow).DataContext).CurrentPage = ApplicationPage.Z34;
        }

        private void E35()
        {
            ((WindowViewModel)((MainWindow)Application.Current.MainWindow).DataContext).CurrentPage = ApplicationPage.E35;
        }

        private void Eq()
        {
            ((WindowViewModel)((MainWindow)Application.Current.MainWindow).DataContext).CurrentPage = ApplicationPage.Eq;
        }

        private void Eq2()
        {
            ((WindowViewModel)((MainWindow)Application.Current.MainWindow).DataContext).CurrentPage = ApplicationPage.Eq2;
        }

        private void Eq3()
        {
            ((WindowViewModel)((MainWindow)Application.Current.MainWindow).DataContext).CurrentPage = ApplicationPage.Eq3;
        }

        private void Eq4()
        {
            ((WindowViewModel)((MainWindow)Application.Current.MainWindow).DataContext).CurrentPage = ApplicationPage.Eq4;
        }

        private void Eq5()
        {
            ((WindowViewModel)((MainWindow)Application.Current.MainWindow).DataContext).CurrentPage = ApplicationPage.Eq5;
        }

        private void Eq6()
        {
            ((WindowViewModel)((MainWindow)Application.Current.MainWindow).DataContext).CurrentPage = ApplicationPage.Eq6;
        }

        private void Eq7()
        {
            ((WindowViewModel)((MainWindow)Application.Current.MainWindow).DataContext).CurrentPage = ApplicationPage.Eq7;
        }

        private void Eq8()
        {
            ((WindowViewModel)((MainWindow)Application.Current.MainWindow).DataContext).CurrentPage = ApplicationPage.Eq8;
        }

        private void Eq9()
        {
            ((WindowViewModel)((MainWindow)Application.Current.MainWindow).DataContext).CurrentPage = ApplicationPage.Eq9;
        }

        private void Eq10()
        {
            ((WindowViewModel)((MainWindow)Application.Current.MainWindow).DataContext).CurrentPage = ApplicationPage.Eq10;
        }

        private void Eq11()
        {
            ((WindowViewModel)((MainWindow)Application.Current.MainWindow).DataContext).CurrentPage = ApplicationPage.Eq11;
        }

        private void Eq12()
        {
            ((WindowViewModel)((MainWindow)Application.Current.MainWindow).DataContext).CurrentPage = ApplicationPage.Eq12;
        }

        private void Eq13()
        {
            ((WindowViewModel)((MainWindow)Application.Current.MainWindow).DataContext).CurrentPage = ApplicationPage.Eq13;
        }

        private void Eq14()
        {
            ((WindowViewModel)((MainWindow)Application.Current.MainWindow).DataContext).CurrentPage = ApplicationPage.Eq14;
        }

        private void Eq15()
        {
            ((WindowViewModel)((MainWindow)Application.Current.MainWindow).DataContext).CurrentPage = ApplicationPage.Eq15;
        }
        private void Eq16()
        {
            ((WindowViewModel)((MainWindow)Application.Current.MainWindow).DataContext).CurrentPage = ApplicationPage.Eq16;
        }
        private void Eq17()
        {
            ((WindowViewModel)((MainWindow)Application.Current.MainWindow).DataContext).CurrentPage = ApplicationPage.Eq17;
        }
        private void Eq18()
        {
            ((WindowViewModel)((MainWindow)Application.Current.MainWindow).DataContext).CurrentPage = ApplicationPage.Eq18;
        }

        private ICommand makeAccountCommand;
        public ICommand MakeAccountCommand => makeAccountCommand ??= new RelayCommand(MakeAccount);

        private void MakeAccount()
        {
        }

        private bool SetProperty<T>(ref T field, T newValue, [CallerMemberName] string propertyName = null)
        {
            if (!Equals(field, newValue))
            {
                field = newValue;
                //PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
                return true;
            }

            return false;
        }

        private string registerUserName;

        public string RegisterUserName { get => registerUserName; set => SetProperty(ref registerUserName, value); }
    }
}