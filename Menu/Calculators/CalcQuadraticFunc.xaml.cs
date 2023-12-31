﻿using System;
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
        private readonly string variable = "𝑥";
        private readonly string square = "²";
        private void ShowResult(object sender, RoutedEventArgs e)
        {
            if (!TryParseInput(out double a, out double b, out double c))
            {
                DisplayErrorMessage("Ups, coś poszło nie tak. Sprawdź, czy wprowadzone dane są prawidłowe i spróbuj jeszcze raz.");
                Reset();
                return;
            }
            else
            {
                if (a == 0)
                {
                    DisplayErrorMessage("Eq każdej funkcji kwadratowej współczynnik a powinien być liczbą rzeczywistą różną od 0! Spróbuj jeszcze raz.");
                    Reset();
                    return;
                }
                else
                {
                    QuadraticFunctionStandardForm(a, b, c);
                    groupResult.Visibility = Visibility.Visible;
                }
            }
        }

        private bool TryParseInput(out double a, out double b, out double c)
        {
            a = b = c = 0;
            return double.TryParse(fieldA.Text, out a) && double.TryParse(fieldB.Text, out b) && double.TryParse(fieldC.Text, out c);
        }

        private void DisplayErrorMessage(string message)
        {
            MessageBox.Show(message, "Nieprawidłowa wartość!");
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
                        string resultTxt = "\nΔ < 0, funkcja nie posiada miejsc zerowych";
                        result.Text = resultTxt;
                        break;
                    }
                case 0:
                    {
                        string resultTxt = $"\nΔ = 0, funkcja posiada jedno miejsce zerowe, gdzie wierzchołek dotyka osi x: \n 𝑥₀ = {x0}";
                        result.Text = resultTxt;
                        break;
                    }
                default:
                    {
                        string resultTxt = $"\nΔ > 0, funkcja posiada dwa miejsca zerowe: \n 𝑥₁ = {x1} i 𝑥₂ = {x2}";
                        result.Text = resultTxt;
                        break;
                    }
            }
            double p = Math.Round(x0, 2);
            double q = Math.Round((-delta) / (4 * a), 2);
            string vertex = $"𝑊 = ({p} ; {q})";
            this.result.Visibility = Visibility.Visible;
            StandardFormShow(a, b, c);
            VertexFormShow(a, p, q);
            FactoredFormShow(a, x1, x2, delta, vertex);
            Explanation(a, b, c, delta, vertex, x0, x1, x2, p, q);
        }
        private void StandardFormShow(double a, double b, double c)
        {
            string GetTerm(double coefficient, string variable)
            {
                return coefficient switch
                {
                    1 => variable,
                    -1 => $"-{variable}",
                    _ => $"{coefficient}{variable}"
                };
            }

            string FormatTerm(double coefficient, string variable)
            {
                return coefficient > 0 ? $" + {GetTerm(coefficient, variable)}" :
                       coefficient < 0 ? $" - {GetTerm(-coefficient, variable)}" : "";
            }

            string equation = $"𝑓(𝑥) = {GetTerm(a, variable)}{square}";
            equation += FormatTerm(b, variable);
            equation += FormatTerm(c, "");

            StandardGroup.Text = equation;
        }

        private void VertexFormShow(double a, double p, double q)
        {
            string VertexForm = $"𝑓(𝑥) = {a}(𝑥 {(p >= 0 ? "-" : "+")} {Math.Abs(p)})² {(q >= 0 ? "+" : "-")} {Math.Abs(q)}";
            VertexGroup.Text = VertexForm;
        }
        private void FactoredFormShow(double a, double x1, double x2, double delta, string vertex)
        {
            string Factored = "";
            switch (delta)
            {
                case < 0:
                    Factored = "Funkcja nie ma miejsc zerowych, nie ma też zatem postaci iloczynowej!";
                    break;
                case 0:
                    Factored = $"𝑓(𝑥) = {a}(𝑥 {(x1 < 0 ? '+' : '-')} {Math.Abs(x1)})" + square;
                    break;
                default:
                    Factored += $"𝑓(𝑥) = {a}(𝑥 {(x1 >= 0 ? '-' : '+')} {Math.Abs(x1)})(𝑥 {(x2 >= 0 ? '-' : '+')} {Math.Abs(x2)})";
                    break;
            }

            FactoredGroup.Text = Factored;

            string parable = a > 0 ? "Ramiona paraboli skierowane są do góry, ponieważ współczynnik 𝒂 jest dodatni: ⎝⎠" :
                  "Ramiona paraboli skierowane są do dołu, ponieważ współczynnik 𝒂 jest ujemny: ⎛⎞";
            string vertexInfo = $"Współrzędne wierzchołka paraboli znajdują się w punkcie 𝑊(𝑝;𝑞), czyli {vertex}";
            pParable.Text = $"\n{parable}\n{vertexInfo}";
        }
        private void Explanation(double a, double b, double c, double delta, string vertex, double x0, double x1, double x2, double p, double q)
        {
            string[] specialScript = { "₀", "₁", "₂", "²", "√" };

            string deltaText = GetDeltaText(delta, x0, x1, x2, specialScript);

            string explained1 = $@"
Znając wzór na postać ogólną funkcji kwadratowej, zaczynamy od wyliczenia wartości Δ (delty, inaczej wyróżnika funkcji kwadratowej). Użyjemy wzoru:
";
            explanation1.Text = explained1;
            string explained2 = $"Δ = 𝑏{specialScript[3]} − 4⋅𝑎⋅𝑐";
            explanation2.Text = explained2;
            string explained3 = $"Δ = ({b}){specialScript[3]} - 4⋅({a})⋅({c}) = {delta}";
            explanation3.Text = explained3;
            string explained4 = $@"
Sama znajomość delty da nam już bardzo dużo, bo dowiemy się ile pierwiastków trójmianu kwadratowego (to znaczy miejsc zerowych funkcji kwadratowej) znajdziemy w naszej konkretnej funkcji.

Pod uwagę bierzemy zawsze jeden z trzech przypadków.

Eq tym przypadku, {deltaText}.
Znając współczynniki funkcji kwadratowej, możemy przekształcić jej postać ogólną do postaci kanonicznej lub iloczynowej. Postać kanoniczna funkcji kwadratowej wyrażona jest wzorem:
";
            explanation4.Text = explained4;
            string explained5 = $"𝑓(𝑥) = 𝑎(𝑥 − 𝑝){specialScript[3]} + 𝑞";
            explanation5.Text = explained5;
            string explained6 = $"\nDo uzupełnienia wzoru brakuje nam współrzędnych wierzchołka paraboli, 𝑝 i 𝑞. Możesz zauważyć, iż współczynnik 𝑝 można obliczyć używając takiego samego wzoru, jak w przypadku 𝑥{specialScript[0]}. Dla 𝑝 i 𝑞 istnieją następujące wzory:";
            explanation6.Text = explained6;
            string explained8 = $@"Wierzchołek {vertex}, wobec czego po podstawieniu:
                              𝑓(𝑥) = ({a})(𝑥 − ({p})){specialScript[3]} + ({q})

Dla dobra przekształcenia naszej funkcji w postać iloczynową ponownie sięgniemy po znalezione wcześniej miejsca zerowe.

Postać iloczynowa funkcji kwadratowej wyrażona jest wzorem:
";
            explanation8.Text = explained8;
            string explained9 = $"𝑓(𝑥) = 𝑎(𝑥 − 𝑥{specialScript[1]})(𝑥 − 𝑥{specialScript[2]})";
            explanation9.Text = explained9;
            switch (delta)
            {
                case < 0:
                    {
                        string explained10 = "Funkcja nie ma miejsc zerowych, nie istnieje zatem jej postać iloczynowa!";
                        explanation10.Text = explained10;
                        break;
                    }

                case 0:
                    {
                        string explained10 = $"Δ = 0, wystarczy więc policzyć x{specialScript[0]} i skrócić zapis: \n𝑓(𝑥) = ({a})(𝑥 − ({x1})){specialScript[3]}\n";
                        explanation10.Text = explained10;
                        break;
                    }

                default:
                    {
                        string explained10 = $"𝑓(𝑥) = ({a})(𝑥 − ({x1}))(𝑥 − ({x2}))\n";
                        explanation10.Text = explained10;
                        break;
                    }
            }

        }

        private string GetDeltaText(double delta, double x0, double x1, double x2, string[] specialScript)
        {
            return delta switch
            {
                < 0 => "Δ < 0 i funkcja nie posiada miejsc zerowych",
                0 => $"Δ = 0, funkcja posiada jedno miejsce zerowe: 𝑥{specialScript[0]} = {x0}",
                _ => $"Δ > 0, funkcja posiada dwa miejsca zerowe: 𝑥{specialScript[1]} = {x1} oraz 𝑥{specialScript[2]} = {x2}"
            };
        }

    }
}