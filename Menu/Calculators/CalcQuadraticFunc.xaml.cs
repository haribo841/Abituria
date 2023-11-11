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
            if (string.IsNullOrWhiteSpace(fieldA.Text) || string.IsNullOrWhiteSpace(fieldB.Text) || string.IsNullOrWhiteSpace(fieldC.Text))
            {
                MessageBox.Show("Wszystkie pola muszą być uzupełnione.");
                return;
            }
            else
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
                        this.groupResult.Visibility = Visibility.Visible;
                        break;
                }
            }
        }
        private void ButtonReset(object sender, RoutedEventArgs e) => Reset();
        private void Reset()
        {
            fieldA.Text = "";
            fieldB.Text = "";
            fieldC.Text = "";
            this.groupResult.Visibility = Visibility.Collapsed;
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
                        string resultTxt = $"Δ = 0, funkcja posiada jedno miejsce zerowe, gdzie wierzchołek dotyka osi x: \n x₀ = {x0}";
                        result.Text = resultTxt;
                        break;
                    }
                default:
                    {
                        string resultTxt = $"Δ > 0, funkcja posiada dwa miejsca zerowe: \n x₁ = {x1} i x₂ = {x2}";
                        result.Text = resultTxt;
                        break;
                    }
            }
            double p = Math.Round(x0, 2);
            double q = Math.Round((-delta) / (4 * a), 2);
            string vertex = $"({p} ; {q})";
            this.result.Visibility = Visibility.Visible;
            StandardFormShow(a, b, c);
            VertexFormShow(a, p, q);
            FactoredFormShow(a, x1, x2, delta);
            Explanation(a, b, c, delta, vertex, x1, x2);
        }
        private void StandardFormShow(double a, double b, double c)
        {
            string aTerm = (a == 1) ? "x²" : $"{a}x²";
            string bTerm = (b == 0) ? "" : (b > 0 ? $" + {b}x" : $" - {Math.Abs(b)}x");
            string cTerm = (c == 0) ? "" : (c > 0 ? $" + {c}" : $" - {Math.Abs(c)}");
            string equation = $"y = {aTerm}{bTerm}{cTerm}";
            StandardGroup.Text = equation;
        }
        private void VertexFormShow(double a, double p, double q)
        {
            string VertexForm = $"f(x) = {a}(x {(p >= 0 ? "-" : "+")} {Math.Abs(p)})² {(q >= 0 ? "+" : "-")} {Math.Abs(q)}";
            VertexGroup.Text = VertexForm;
        }
        private void FactoredFormShow(double a, double x1, double x2, double delta)
        {
            StringBuilder message = new StringBuilder();
            message.Append(delta switch
            {
                < 0 => "Funkcja nie ma miejsc zerowych, nie ma też zatem postaci iloczynowej!",
                0 => $"f(x) = {a}(x {(x1 < 0 ? '+' : '-')} {Math.Abs(x1):F2})²",
                _ => $"f(x) = {a}(x {(x1 >= 0 ? '-' : '+')} {Math.Abs(x1):F2})(x {(x2 >= 0 ? '-' : '+')} {Math.Abs(x2):F2})"
            });
            FactoredGroup.Text = message.ToString();
        }
        private void Explanation(double a, double b, double c, double delta, string vertex, double x1, double x2)
        {
            string explained = $"Znając wzór na postać ogólną funkcji kwadratowej, zaczynamy od policzenia Δ.\nUżyjemy wzoru Δ = b² − 4⋅a⋅c\n";
            explained += $"Δ = ({b}) - 4⋅({a})⋅({c}) = {delta}";
            explanation.Text = explained;
            string vertexExplanation = $"Współrzędne wierzchołka paraboli znajdują się w punkcie W(p, q), czyli W = {vertex}";
        }
    }
}