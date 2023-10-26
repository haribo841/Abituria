using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Abituria
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }
        private void ButtonCalculator(object sender, RoutedEventArgs e)
        {
            var calculator = new Calculator();
            calculator.Show();
        }
        private void ButtonMatura(object sender, RoutedEventArgs e)
        {
            NavigationWindow window = new NavigationWindow();
            window.Source = new Uri("PageMaturaYears.xaml", UriKind.Relative);
            window.Show();
            this.Visibility = Visibility.Hidden;
        }
        private void ButtonDzialy(object sender, RoutedEventArgs e)
        {
            NavigationWindow window = new NavigationWindow();
            window.Source = new Uri("PageChapters.xaml", UriKind.Relative);
            window.Show();
            this.Visibility = Visibility.Hidden;
        }
        private void ButtonZadania(object sender, RoutedEventArgs e)
        {
            NavigationWindow window = new NavigationWindow();
            window.Source = new Uri("PageExercises.xaml", UriKind.Relative);
            window.Show();
            this.Visibility = Visibility.Hidden;
        }
        private void ButtonWideo(object sender, RoutedEventArgs e)
        {
            NavigationWindow window = new NavigationWindow();
            window.Source = new Uri("PageVideo.xaml", UriKind.Relative);
            window.Show();
            this.Visibility = Visibility.Hidden;
        }
    }
}