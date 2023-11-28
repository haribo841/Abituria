using Abituria.viewmodel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using System.Windows.Controls;

namespace Abituria
{
    public partial class MainWindowLogin : Window
    {
        string UsersFilePath = @"users.txt";

        public MainWindowLogin()
        {
            InitializeComponent();
            this.DataContext = new WindowViewModel(this);
        }
        private List<string> LoadUsersList(string filePath)
        {
            List<string> usersList = new List<string>();

            try
            {
                if (File.Exists(filePath))
                {
                    using (StreamReader reader = new StreamReader(filePath))
                    {
                        string user;
                        while ((user = reader.ReadLine()) != null && usersList.Count < 10)
                        {
                            usersList.Add(user);
                        }
                    }
                }
                else
                {
                    MessageBox.Show("Plik użytkowników nie istnieje.");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Wystąpił błąd podczas odczytu pliku użytkowników: {ex.Message}");
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

            LoadUsersList(UsersFilePath);
        }

        private void LoginConfirm(object sender, RoutedEventArgs e)
        {
            string username = comboBox1.SelectedItem as string;
            if (!string.IsNullOrEmpty(username))
            {
                MessageBox.Show(username);
                Window currentWindow = Window.GetWindow(this);
                currentWindow.Close();
                MainWindow mainWindow = new MainWindow();
                mainWindow.Show();
            }
            else
            {
                MessageBox.Show("Wybierz nazwę użytkownika!", "Brak wyboru");
            }
        }
        private void BtnCreateNew(object sender, RoutedEventArgs e)
        {
            btn1.Visibility = Visibility.Collapsed;
            btn2.Visibility = Visibility.Collapsed;
            inputGB.Visibility = Visibility.Visible;
        }

        private void AddUser(object sender, RoutedEventArgs e)
        {
            string newUsername = nameInput.Text.Trim();
            List<string> usersList = LoadUsersList(UsersFilePath);
            bool isTaken = usersList.Contains(newUsername);
            bool isValid = newUsername.Length <= 15 && !isTaken;

            if (!isValid)
            {
                if (isTaken)
                {
                    MessageBox.Show("Taki użytkownik już istnieje! Wybierz inną nazwę użytkownika", "Nazwa zajęta");
                }
                else
                {
                    MessageBox.Show("Wybrana nazwa jest za długa!", "Nazwa zbyt długa");
                }
                return;
            }

            CreateProfile(newUsername, UsersFilePath);

            void CreateProfile(string newUsername, string usersFile)
            {
                using (StreamWriter writer = File.AppendText(usersFile))
                {
                    writer.WriteLine(newUsername);
                }
            }
        }
    }
}