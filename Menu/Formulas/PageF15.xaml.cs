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

namespace Abituria.Menu.Formulas
{
    /// <summary>
    /// Interaction logic for PageF15.xaml
    /// </summary>
    public partial class PageF15 : Page
    {
        public PageF15()
        {
            InitializeComponent();
        }
        private void ButtonAbituria(object sender, RoutedEventArgs e)
        {
            Window currentFindow = Window.GetWindow(this);
            currentFindow.Close();
            MainWindow mainFindow = new MainWindow();
            mainFindow.Show();
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
        private void F1(object sender, RoutedEventArgs e)
        {
            PageF1 pageF1 = new PageF1();
            NavigationService.Navigate(pageF1);
        }
        private void F2(object sender, RoutedEventArgs e)
        {
            PageF2 pageF2 = new PageF2();
            NavigationService.Navigate(pageF2);
        }
        private void F3(object sender, RoutedEventArgs e)
        {
            PageF3 pageF3 = new PageF3();
            NavigationService.Navigate(pageF3);
        }
        private void F4(object sender, RoutedEventArgs e)
        {
            PageF4 pageF4 = new PageF4();
            NavigationService.Navigate(pageF4);
        }
        private void F5(object sender, RoutedEventArgs e)
        {
            PageF5 pageF5 = new PageF5();
            NavigationService.Navigate(pageF5);
        }
        private void F6(object sender, RoutedEventArgs e)
        {
            PageF6 pageF6 = new PageF6();
            NavigationService.Navigate(pageF6);
        }
        private void F7(object sender, RoutedEventArgs e)
        {
            PageF7 pageF7 = new PageF7();
            NavigationService.Navigate(pageF7);
        }
        private void F8(object sender, RoutedEventArgs e)
        {
            PageF8 pageF8 = new PageF8();
            NavigationService.Navigate(pageF8);
        }
        private void F9(object sender, RoutedEventArgs e)
        {
            PageF9 pageF9 = new PageF9();
            NavigationService.Navigate(pageF9);
        }
        private void F10(object sender, RoutedEventArgs e)
        {
            PageF10 pageF10 = new PageF10();
            NavigationService.Navigate(pageF10);
        }
        private void F11(object sender, RoutedEventArgs e)
        {
            PageF11 pageF11 = new PageF11();
            NavigationService.Navigate(pageF11);
        }
        private void F12(object sender, RoutedEventArgs e)
        {
            PageF12 pageF12 = new PageF12();
            NavigationService.Navigate(pageF12);
        }
        private void F13(object sender, RoutedEventArgs e)
        {
            PageF13 pageF13 = new PageF13();
            NavigationService.Navigate(pageF13);
        }
        private void F14(object sender, RoutedEventArgs e)
        {
            PageF14 pageF14 = new PageF14();
            NavigationService.Navigate(pageF14);
        }
        private void F15(object sender, RoutedEventArgs e)
        {
            PageF15 pageF15 = new PageF15();
            NavigationService.Navigate(pageF15);
        }
        private void F16(object sender, RoutedEventArgs e)
        {
            PageF16 pageF16 = new PageF16();
            NavigationService.Navigate(pageF16);
        }
        private void F17(object sender, RoutedEventArgs e)
        {
            PageF17 pageF17 = new PageF17();
            NavigationService.Navigate(pageF17);
        }
        private void F18(object sender, RoutedEventArgs e)
        {
            PageF18 pageF18 = new PageF18();
            NavigationService.Navigate(pageF18);
        }
    }
}