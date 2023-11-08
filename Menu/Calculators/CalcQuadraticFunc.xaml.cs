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
        private void ButtonReset(object sender, RoutedEventArgs e) => Reset();
        private void Reset()
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
            string aTerm = (a == 1) ? "x^2" : $"{a}x^2";
            string bTerm = (b == 0) ? "" : (b > 0 ? $" + {b}x" : $" - {Math.Abs(b)}x");
            string cTerm = (c == 0) ? "" : (c > 0 ? $" + {c}" : $" - {Math.Abs(c)}");

            string equation = $"y = {aTerm}{bTerm}{cTerm}";

            StandardGroup.Text = equation;
        }

    }
}