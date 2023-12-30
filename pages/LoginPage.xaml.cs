using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Abituria.pages
{
    /// <summary>
    /// Interaction logic for LoginPage.xaml
    /// </summary>
    public partial class LoginPage : Page
    {
        readonly string usersFile = @"users.txt";
        public LoginPage()
        {
            InitializeComponent();
            DataContext = new LoginViewModel();
        }
        public SecureString SecurePassword => PasswordText.SecurePassword;///Chronione hasło dla tej strony
        /*
        private List<string> SetUsersList(string usersFile)
        {
            List<string> usersList = new List<string>();

            using (StreamReader reader = new StreamReader(usersFile))
            {
                string user = "";
                int i = 0;

                while ((user = reader.ReadLine()) != null || i > 10)
                {
                    usersList.Add(user);
                    i++;
                }
            }

            comboBox1.ItemsSource = usersList;
            return usersList;
        }
        private void BtnAcntExists(object sender, RoutedEventArgs e)
        {
            btn1.Visibility = Visibility.Collapsed;
            btn2.Visibility = Visibility.Collapsed;
            btnConfirm.Visibility = Visibility.Visible;
            comboBox1.Visibility = Visibility.Visible;

            SetUsersList(usersFile);
        }
        private string ChosenUsername()
        {
            string username = comboBox1.SelectedItem as string;
            return username;
        }
        private void LoginConfirm(object sender, RoutedEventArgs e)
        {
            string username = ChosenUsername();
            MessageBox.Show(username);
            //do zmiany na lepsze, gdy ustawi się frame'y
            var mainWin = new MainWindow
            {
            };
            mainWin.ShowDialog();
        }

        private void BtnCreateNew(object sender, RoutedEventArgs e)
        {
            btn1.Visibility = Visibility.Collapsed;
            btn2.Visibility = Visibility.Collapsed;
            inputGB.Visibility = Visibility.Visible;
        }
        private void CreateProfile(string newUser, string usersFile, bool isValid)
        {
            MessageBox.Show(newUser);
            using (StreamWriter writer = File.AppendText(usersFile))
            {
                if (isValid)
                {
                    writer.WriteLine(newUser);

                    btn1.Visibility = Visibility.Visible;
                    btn2.Visibility = Visibility.Visible;
                    btnConfirm.Visibility = Visibility.Collapsed;
                    comboBox1.Visibility = Visibility.Collapsed;
                    inputGB.Visibility = Visibility.Collapsed;
                }
                else
                {
                    MessageBox.Show("Nie udało się utworzyć nowego profili", "Błąd!");
                }
            }
        }
        private void AddUser(object sender, RoutedEventArgs e)
        {
            string newUsername = nameInput.Text;
            List<string> usersList = SetUsersList(usersFile);
            int usersCount = usersList.Count;
            bool isSpaces = string.IsNullOrWhiteSpace(newUsername);
            bool isTaken = true;
            // WERYFIKACJA
            foreach (string user in usersList)
            {
                if (user == newUsername)
                {
                    MessageBox.Show("Taki użytkownik już istnieje! Wybierz inną nazwę użytkownika", "Nazwa zajęta");
                    isTaken = true;
                    break;
                }
                else
                {
                    isTaken = false;
                }
            }
            bool isValid;
            if (usersCount > 9)
            {
                MessageBox.Show("Wyczerpałeś już limit tworzenia profili. Wybierz istniejący:", "Przekroczono limit dostępnych miejsc");
                isValid = false;
                btn1.Visibility = Visibility.Visible;
                btn2.Visibility = Visibility.Visible;
                btnConfirm.Visibility = Visibility.Collapsed;
                comboBox1.Visibility = Visibility.Collapsed;
                inputGB.Visibility = Visibility.Collapsed;
            }
            else if (newUsername.Length > 15)
            {
                MessageBox.Show("Wybrana nazwa jest za długa!", "Nazwa zbyt długa");
                isValid = false;
                nameInput.Text = "";
            }
            else if (newUsername.Length < 1 || isSpaces)
            {
                MessageBox.Show("Wybrana nazwa jest nieprawidłowa! Spróbuj jeszcze raz", "Nieprawidłowa nazwa:");
                isValid = false;
                nameInput.Text = "";
            }
            else
            {
                isValid = !isTaken;
            }
            CreateProfile(newUsername, usersFile, isValid);
        }
        private void UserCancel(object sender, RoutedEventArgs e)
        {
            var mainWin = new LoginPage
            {
            };
        }
        private void ButtonAbituria(object sender, RoutedEventArgs e)
        {
            InitializeComponent();
        }
        */
    }
}