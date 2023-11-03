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
                        double a = double.Parse(fieldA.Text);
                        double b = double.Parse(fieldB.Text);
                        double c = double.Parse(fieldC.Text);
                        StandardForm(a, b, c);
                        break;
                    }

                case Visibility.Hidden:
                    break;
                case Visibility.Collapsed:
                    break;
                default:
                    MessageBox.Show("WIP");
                    break;
            }
        }

        private void Reset(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("WIP");
        }
        private void StandardForm(double a, double b, double c)
        {
            double delta = Math.Pow(b, 2) - 4 * a * c;
            double x0 = -b / (2 * a);
            double x1 = Math.Round((-b - Math.Sqrt(delta)) / (2 * a), 2);
            double x2 = Math.Round((-b + Math.Sqrt(delta)) / (2 * a), 2);


            switch (delta)
            {
                case < 0:
                    {
                        string resultTxt = "Δ < 0, funkcja nie posiada miejsc zerowych";
                        result.Text = resultTxt;
                        break;
                    }

                case 0:
                    {
                        string resultTxt = "Δ = 0, funkcja posiada jedno miejsce zerowe, gdzie wierzchołek dotyka osi x";
                        result.Text = resultTxt;
                        break;
                    }

                default:
                    {
                        string resultTxt = $"Δ > 0, funkcja posiada dwa miejsca zerowe: x1 = {x1} i x2 = {x2}";
                        result.Text = resultTxt;
                        break;
                    }
            }

            this.resultHead.Visibility = Visibility.Visible;
            this.result.Visibility = Visibility.Visible;
        }
        private void ButtonStandard(object sender, RoutedEventArgs e)
        {
            switch (StandardGroup.Visibility)
            {
                case Visibility.Collapsed when buttonCalculate.Visibility == Visibility.Collapsed && buttonReset.Visibility == Visibility.Collapsed:
                    StandardGroup.Visibility = Visibility.Visible;
                    buttonCalculate.Visibility = Visibility.Visible;
                    buttonReset.Visibility = Visibility.Visible;
                    break;
                default:
                    StandardGroup.Visibility = Visibility.Collapsed;
                    buttonCalculate.Visibility = Visibility.Collapsed;
                    buttonReset.Visibility = Visibility.Collapsed;
                    this.resultHead.Visibility = Visibility.Collapsed;
                    this.result.Visibility = Visibility.Collapsed;
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
