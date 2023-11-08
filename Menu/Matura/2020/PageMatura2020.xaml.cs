using System;
using System.Collections.Generic;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Abituria
{
    /// <summary>
    /// Logika interakcji dla klasy PageMatura2020.xaml
    /// </summary>
    public partial class PageMatura2020 : Page
    {
        public PageMatura2020()
        {
            InitializeComponent();
        }
        private void ButtonAbituria(object sender, RoutedEventArgs e)
        {
            var mainWin = new MainWindow();
            mainWin.Show();

        }
        private void ButtonCalculator(object sender, RoutedEventArgs e)
        {
            var calculator = new Calculator();
            calculator.Show();
        }
        private void ButtonMatura(object sender, RoutedEventArgs e)
        {
            PageMaturaYears pageMaturaYears = new PageMaturaYears();
            NavigationService.Navigate(pageMaturaYears);
        }
        private void ButtonChapters(object sender, RoutedEventArgs e)
        {
            PageChapters pageChapters = new PageChapters();
            NavigationService.Navigate(pageChapters);
        }
        private void ButtonExercises(object sender, RoutedEventArgs e)
        {
            PageExercises pageExercises = new PageExercises();
            NavigationService.Navigate(pageExercises);
        }
        private void ButtonVideo(object sender, RoutedEventArgs e)
        {
            PageVideo pageVideo = new PageVideo();
            NavigationService.Navigate(pageVideo);
        }
        private void ButtonZad1(object sender, RoutedEventArgs e)
        {
            InitializeComponent();
        }

        private void ButtonZad2(object sender, RoutedEventArgs e)
        {
            InitializeComponent();
        }

        private void ButtonZad3(object sender, RoutedEventArgs e)
        {
            InitializeComponent();
        }
    }
}