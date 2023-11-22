﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Abituria
{
    /// <summary>
    /// Interaction logic for MainWindowLogin.xaml
    /// </summary>
    public partial class MainWindowLogin : Window
    {
        public MainWindowLogin()
        {
            InitializeComponent();
            Login();
        }
        class MyClass
        {
            public int Id { get; set; }
            public string Name { get; set; }
        }
        private void Login()
        {
            string username = "";
            string path = @"user.txt";

            if (File.Exists(path))
            {
                using StreamWriter writer = File.AppendText(path);
            }
            else
            {
                MessageBox.Show("The user file does not exist or cannot be found.");
                comboBox1.ItemsSource = new List<string> { "TODO", "WIP"};
            }
        }

        private static void CreateProfile()
        {

        }
    }
}