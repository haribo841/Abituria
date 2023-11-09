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
            MainFrame.NavigationService.Navigate(new Uri("Menu/CalculatorChoice.xaml", UriKind.Relative));
        }
        private void ButtonMatura(object sender, RoutedEventArgs e)
        {
            NavigationWindow window = new NavigationWindow();
            window.Source = new Uri("Menu/Matura/PageMaturaYears.xaml", UriKind.Relative);
            window.Show();
            this.Visibility = Visibility.Hidden;
        }
        private void ButtonChapters(object sender, RoutedEventArgs e)
        {
            NavigationWindow window = new NavigationWindow();
            window.Source = new Uri("Menu/PageChapters.xaml", UriKind.Relative);
            window.Show();
            this.Visibility = Visibility.Hidden;
        }
        private void ButtonExercises(object sender, RoutedEventArgs e)
        {
            NavigationWindow window = new NavigationWindow();
            window.Source = new Uri("Menu/PageExercises.xaml", UriKind.Relative);
            window.Show();
            this.Visibility = Visibility.Hidden;
        }
        private void ButtonVideo(object sender, RoutedEventArgs e)
        {
            NavigationWindow window = new NavigationWindow();
            window.Source = new Uri("Menu/PageVideo.xaml", UriKind.Relative);
            window.Show();
            this.Visibility = Visibility.Hidden;
        }
    }
}