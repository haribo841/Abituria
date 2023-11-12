﻿using Abituria.Menu.Calculators;
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

namespace Abituria.Menu
{
    /// <summary>
    /// Logika interakcji dla klasy CalculatorChoice.xaml
    /// </summary>
    public partial class CalculatorChoice : Page
    {
        public CalculatorChoice()
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
        private void ButtonCalcBasic(object sender, RoutedEventArgs e)
        {
            var calculator = new Calculator();
            calculator.Show();
        }
        private void ButtonCalcQuad(object sender, RoutedEventArgs e)
        {
            var calcQuad = new CalcQuadraticFunc();
            calcQuad.Show();
        }
    }
}
