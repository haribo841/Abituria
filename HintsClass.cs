using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Abituria
{
    class HintsClass
    {
        public static string AnswerButtonChange(object sender, bool ansChecked)
        {
            string message = "";
            Button? button = sender as Button;
            switch (ansChecked)
            {
                case true:
                    button.Background = Brushes.Green;
                    message = "To prawidłowa odpowiedź!";
                    break;
                default:
                    button.Background = Brushes.Red;
                    message = "Odpowiedź jest niepoprawna. Spróbuj jeszcze raz.";
                    break;
            }
            return message;
        }
        public static string HintMP21Z1(int counter)
        {
            string hint = "";
            string[] hintsArray = { "Krok 1: podpowiedź", "Krok 2: podpowiedź", "Krok 3: podpowiedź", "Krok 4: podpowiedź" };
            hint = counter switch
            {
                1 => hintsArray[0],
                2 => hintsArray[0] + "\n" + hintsArray[1],
                3 => hintsArray[0] + "\n" + hintsArray[1] + "\n" + hintsArray[2],
                4 => hintsArray[0] + "\n" + hintsArray[1] + "\n" + hintsArray[2] + "\n" + hintsArray[3],
                _ => hintsArray[0] + "\n" + hintsArray[1] + "\n" + hintsArray[2] + "\n" + hintsArray[3],
            };
            return hint;
        }
    }
}