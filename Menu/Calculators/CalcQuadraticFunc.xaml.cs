using System;
using System.Buffers;
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
        private void ShowResult(object sender, RoutedEventArgs e)
        {
            double a = double.Parse(fieldA.Text);
            double b = double.Parse(fieldB.Text);
            double c = double.Parse(fieldC.Text);
            switch (a)
            {
                case 0:
                    MessageBox.Show("Pamiętaj, że w każdej funkcji kwadratowej współczynnik a jest liczbą rzeczywistą różną od 0!");
                    return;
                default:
                    QuadraticFunctionStandardForm(a, b, c);
                    break;
            }
        }
        private void Reset(object sender, RoutedEventArgs e)
        {
            fieldA.Text = "";
            fieldB.Text = "";
            fieldC.Text = "";
        }
        private void QuadraticFunctionStandardForm(double a, double b, double c)
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
            this.result.Visibility = Visibility.Visible;
            StandardFormShow(a, b, c);
        }
        private void StandardFormShow(double a, double b, double c)
        {
            switch (b)
            {
                case > 0:
                    switch (c)
                    {
                        case > 0:
                            StandardGroup.Text = a switch
                            {
                                1 => b switch
                                {
                                    1 => $"y = x2 + x + {c}",
                                    _ => $"y = x2 + {b}x + {c}",
                                },
                                _ => b switch
                                {
                                    1 => $"y = {a}x^2 + x + {c}",
                                    _ => $"y = {a}x^2 + {b}x + {c}",
                                },
                            };
                            break;
                        case < 0:
                            StandardGroup.Text = (a, b) switch
                            {
                                (1, _) => $"y = x^2 + x - {-c}",
                                (_, 1) => $"y = {a}x^2 + x - {-c}",
                                _ => $"y = {a}x^2 + {b}x - {-c}",
                            };
                            break;
                        case 0:
                            StandardGroup.Text = (a, b) switch
                            {
                                (1, 1) => "y = x^2 + x",
                                (1, _) => $"y = x^2 + {b}x",
                                (_, 1) => $"y = {a}x^2 + x",
                                _ => $"y = {a}x^2 + {b}x",
                            };
                            break;
                        default:
                            break;
                    }
                    break;
                case < 0:
                    this.StandardGroup.Text = a switch
                    {
                        1 => c > 0 ? $"y = x^2 - {-b}x + {c}" :
                             c < 0 ? $"y = x^2 - {-b}x - {-c}" :
                             $"y = x^2 - {-b}x",
                        _ => c > 0 ? $"y = {a}x^2 - {-b}x + {c}" :
                             c < 0 ? $"y = {a}x^2 - {-b}x - {-c}" :
                             $"y = {a}x^2 - {-b}x",
                    };
                    break;
                case 0:
                    switch (c)
                    {
                        case > 0:
                            StandardGroup.Text = a switch
                            {
                                1 => $"y = x2 + {c}",
                                _ => $"y = {a}x2 + {c}",
                            };
                            break;
                        case < 0:
                            StandardGroup.Text = a switch
                            {
                                1 => $"y = x2 - {c * -1}",
                                _ => $"y = {a}x2 - {c * -1}",
                            };
                            break;
                        case 0:
                            StandardGroup.Text = a switch
                            {
                                1 => $"y = x2",
                                _ => $"y = {a}x2",
                            };
                            break;
                        default:
                            break;
                    }
                    break;
                default:
                    break;
            }
        }
    }
}