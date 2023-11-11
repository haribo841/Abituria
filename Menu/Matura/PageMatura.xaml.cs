using Abituria.Menu;
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
    /// Logika interakcji dla klasy PageMatura.xaml
    /// </summary>
    public partial class PageMatura : Page
    {
        public PageMatura()
        {
            InitializeComponent();
        }
        private void ButtonAbituria(object sender, RoutedEventArgs e)
        {
            Window currentWindow = Window.GetWindow(this);
            currentWindow.Close();
            MainWindow mainWindow = new MainWindow();
            mainWindow.Show();
        }
        private void ButtonCalculator(object sender, RoutedEventArgs e)
        {
            CalculatorChoice calculator = new CalculatorChoice();
            NavigationService.Navigate(calculator);
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
            PageFormulas pageVideo = new PageFormulas();
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