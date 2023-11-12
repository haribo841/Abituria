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
using Abituria.Menu;

namespace Abituria
{
    /// <summary>
    /// Logika interakcji dla klasy PageChapters.xaml
    /// </summary>
    public partial class PageChapters : Page
    {
        public PageChapters()
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
        private void ButtonFormulas(object sender, RoutedEventArgs e)
        {
            PageFormulas pageFormulas = new PageFormulas();
            NavigationService.Navigate(pageFormulas);
        }
        private void ButtonDzial1(object sender, RoutedEventArgs e)
        {
            PageChapter1 pageChapter1 = new PageChapter1();
            NavigationService.Navigate(pageChapter1);
        }
        private void ButtonDzial2(object sender, RoutedEventArgs e)
        {
            PageChapter2 pageChapter2 = new PageChapter2();
            NavigationService.Navigate(pageChapter2);
        }
        private void ButtonDzial3(object sender, RoutedEventArgs e)
        {
            PageChapter3 pageChapter3 = new PageChapter3();
            NavigationService.Navigate(pageChapter3);
        }
    }
}