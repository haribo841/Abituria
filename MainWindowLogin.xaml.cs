using System;
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

        private void Login()
        {
            string username = "";
            string path = @"user.txt";

            if (File.Exists(path))
            {
                string readText = File.ReadAllText(path);
                this.txt1.Text = readText;
            }
            else
            {
                MessageBox.Show("The user file does not exist or cannot be found.");
            }
        }

        private static void CreateProfile()
        {

        }
    }
}