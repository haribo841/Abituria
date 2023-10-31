using Abituria.Menu.Matura._2021p;
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
    /// Logika interakcji dla klasy PageMP21Z1.xaml
    /// </summary>
    public partial class PageMP21Z1 : Page
    {
        public PageMP21Z1()
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
        private void MP2021Exe1(object sender, RoutedEventArgs e)
        {
            PageMP21Z1 pageMP21Z1 = new PageMP21Z1();
            NavigationService.Navigate(pageMP21Z1);
        }
        private void MP2021Exe2(object sender, RoutedEventArgs e)
        {
            PageMP21Z2 pageMP21Z2 = new PageMP21Z2();
            NavigationService.Navigate(pageMP21Z2);
        }
        private void MP2021Exe3(object sender, RoutedEventArgs e)
        {
            PageMP21Z3 pageMP21Z3 = new PageMP21Z3();
            NavigationService.Navigate(pageMP21Z3);
        }
    }
}