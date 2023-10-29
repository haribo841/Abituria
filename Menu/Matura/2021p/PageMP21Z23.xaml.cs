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

namespace Abituria.Menu.Matura._2021p
{
    /// <summary>
    /// Logika interakcji dla klasy PageMP21Z23.xaml
    /// </summary>
    public partial class PageMP21Z23 : Page
    {
        public PageMP21Z23()
        {
            InitializeComponent();
        }
        private void ButtonAbituria(object sender, RoutedEventArgs e)
        {
            var mainWin = new MainWindow();
            mainWin.Show();

        }
        private void ButtonKalkulator(object sender, RoutedEventArgs e)
        {
            var calculator = new Calculator();
            calculator.Show();
        }
        private void ButtonMatura(object sender, RoutedEventArgs e)
        {
            PageMaturaYears pageMaturaYears = new PageMaturaYears();
            NavigationService.Navigate(pageMaturaYears);
        }
        private void ButtonDzialy(object sender, RoutedEventArgs e)
        {
            PageChapters pageChapters = new PageChapters();
            NavigationService.Navigate(pageChapters);
        }
        private void ButtonZadania(object sender, RoutedEventArgs e)
        {
            PageExercises pageExercises = new PageExercises();
            NavigationService.Navigate(pageExercises);
        }
        private void ButtonWideo(object sender, RoutedEventArgs e)
        {
            PageVideo pageVideo = new PageVideo();
            NavigationService.Navigate(pageVideo);
        }
        private void Button2021(object sender, RoutedEventArgs e)
        {
            PageMatura2021 pageMatura2021 = new PageMatura2021();
            NavigationService.Navigate(pageMatura2021);
        }
        private void Button2020(object sender, RoutedEventArgs e)
        {
            PageMatura2020 pageMatura2020 = new PageMatura2020();
            NavigationService.Navigate(pageMatura2020);
        }
        private void Button2019(object sender, RoutedEventArgs e)
        {
            PageMatura2019 pageMatura2019 = new PageMatura2019();
            NavigationService.Navigate(pageMatura2019);
        }
    }
}