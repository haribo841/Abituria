using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Abituria
{
    [DebuggerDisplay("{" + nameof(DebuggerDisplay) + "(),nq}")]
    class HintsClass
    {
        public static string AnswerButtonChange(object sender, bool ansChecked)
        {
            string message = "";
            Button? button = sender as Button;
            switch (ansChecked)
            {
                case true:
                    button.Background = Brushes.LimeGreen;
                    message = "To prawidłowa odpowiedź!";
                    break;
                default:
                    button.Background = Brushes.IndianRed;
                    message = "Odpowiedź jest niepoprawna. Spróbuj jeszcze raz.";
                    break;
            }
            return message;
        }
        public static string Hint(int counter, string[] hintsArray)
        {
            string hint = hintsArray[0];
            for (int i = 1; i < counter; i++)
            {
                if (i < hintsArray.Length)
                {
                    hint = hint + @" \\ " + hintsArray[i];
                }
            }
            return hint;
        }
        private string DebuggerDisplay => ToString();
    }
}