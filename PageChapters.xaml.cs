﻿using System;
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
            //MaturaFrame.NavigationService.Navigate(new Uri("PageMatura.xaml", UriKind.Relative));
            //MaturaFrame.NavigationService.Navigate(new PageMatura());
            //MaturaFrame.Content = new PageMatura();
            PageMatura pageMatura = new PageMatura();
            NavigationService.Navigate(pageMatura);
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
    }
}