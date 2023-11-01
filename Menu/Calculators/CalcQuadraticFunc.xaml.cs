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

namespace Abituria.Menu.Calculators
{
    /// <summary>
    /// Logika interakcji dla klasy CalcQuadraticFunc.xaml
    /// </summary>
    public partial class CalcQuadraticFunc : Window
    {
        public CalcQuadraticFunc()
        {
            InitializeComponent();
        }
        private void Calculate(object sender, RoutedEventArgs e)
        {
            switch (StandardGroup.Visibility)
            {
                case Visibility.Visible:
                    {
                        float warA = float.Parse(fieldA.Text);
                        StandardForm(warA, 20, 10);
                        break;
                    }

                default:
                    MessageBox.Show("WIP");
                    break;
            }
        }

        private void Reset(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("WIP");
        }
        private void StandardForm(float a, float b, float c)
        {

            float result = a + b + c;

            MessageBox.Show("WIP " + result);
        }
        private void ButtonStandard(object sender, RoutedEventArgs e)
        {
            switch (pOgolna.Visibility)
            {
                case Visibility.Collapsed when buttonPrzelicz.Visibility == Visibility.Collapsed && buttonReset.Visibility == Visibility.Collapsed:
                    pOgolna.Visibility = Visibility.Visible;
                    buttonPrzelicz.Visibility = Visibility.Visible;
                    buttonReset.Visibility = Visibility.Visible;
                    break;
                default:
                    pOgolna.Visibility = Visibility.Collapsed;
                    buttonPrzelicz.Visibility = Visibility.Collapsed;
                    buttonReset.Visibility = Visibility.Collapsed;
                    break;
            }
        }
        private void ButtonFactored(object sender, RoutedEventArgs e)
        {
            InitializeComponent();
        }
        private void ButtonVertex(object sender, RoutedEventArgs e)
        {
            InitializeComponent();
        }
    }
}
