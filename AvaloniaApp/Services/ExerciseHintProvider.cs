using System;
using System.Collections.Generic;
using Abituria.Models;

namespace Abituria.Services;

/// <summary>
/// Provides the guidance displayed before a learner decides to reveal a full solution.
/// Authored hints from content take precedence. Exam transcriptions without separate hints
/// receive short, non-answer-revealing prompts based on their existing topic and answer mode.
/// </summary>
public static class ExerciseHintProvider
{
    private static readonly IReadOnlyDictionary<string, string> TopicHints =
        new Dictionary<string, string>(StringComparer.Ordinal)
        {
            ["powers"] = "Zapisz liczby i wyrażenia jako potęgi o wspólnej podstawie, a następnie zastosuj prawa działań na potęgach.",
            ["logarithms"] = "Najpierw zapisz warunek dziedziny logarytmu, a potem użyj definicji lub praw działań na logarytmach.",
            ["percentages"] = "Zamień procent na ułamek albo mnożnik dziesiętny i wyraźnie zaznacz, do której wielkości się odnosi.",
            ["identities"] = "Rozpoznaj wzór skróconego mnożenia i sprawdź znaki przy wyrazach środkowych.",
            ["equations"] = "Uporządkuj równanie tak, aby po jednej stronie był zero, a potem dobierz metodę do jego typu.",
            ["inequalities"] = "Zapisz warunki dziedziny i rozważ miejsca, w których znak wyrażenia może się zmienić.",
            ["linear-function"] = "Odczytaj lub wyznacz współczynnik kierunkowy i wyraz wolny, a potem zapisz zależność liniową krok po kroku.",
            ["quadratic-function"] = "Sprowadź wyrażenie do postaci wygodnej dla pytania: ogólnej, kanonicznej albo iloczynowej.",
            ["sequences"] = "Wypisz kilka kolejnych wyrazów lub różnice i sprawdź, czy pasuje wzór na ciąg arytmetyczny albo geometryczny.",
            ["trigonometry"] = "Narysuj pomocniczy trójkąt lub zaznacz kąt, a następnie dobierz definicję funkcji trygonometrycznej do danych boków.",
            ["plane-geometry"] = "Wykonaj czytelny rysunek pomocniczy i zaznacz dane kąty, długości oraz zależności równoległości lub podobieństwa.",
            ["lines-and-segments"] = "Zapisz współrzędne lub równanie prostej i wybierz zależność opisującą kierunek, odległość albo punkt przecięcia.",
            ["solid-geometry"] = "Szkic bryły uzupełnij o potrzebny przekrój albo trójkąt prostokątny, zanim zaczniesz liczyć długości lub objętość.",
            ["combinatorics"] = "Ustal, czy liczy się kolejność, i rozpisz kolejne etapy wyboru przed użyciem reguły mnożenia lub symbolu Newtona.",
            ["probability"] = "Nazwij zdarzenie, policz wszystkie jednakowo prawdopodobne wyniki i dopiero potem zlicz wyniki sprzyjające.",
            ["statistics"] = "Wypisz dane w uporządkowanej kolejności i sprawdź, czy pytanie dotyczy średniej, mediany, dominanty czy rozrzutu.",
            ["proofs"] = "Zapisz tezę i dane, a następnie wybierz znaną własność lub przekształcenie, które bezpośrednio przybliża do tezy."
        };

    public static IReadOnlyList<string> GetHints(LearningExercise exercise)
    {
        ArgumentNullException.ThrowIfNull(exercise);
        if (exercise.Hints.Count > 0)
            return exercise.Hints;

        return
        [
            TopicHints.GetValueOrDefault(exercise.TopicId, "Zapisz dane, oznacz niewiadomą i określ, czego dokładnie szukasz."),
            GetAnswerModeHint(exercise)
        ];
    }

    private static string GetAnswerModeHint(LearningExercise exercise)
    {
        if (exercise.IsMultipleChoice)
            return "Najpierw wykonaj własne obliczenia, a dopiero potem porównaj wynik z odpowiedziami A-D.";
        if (exercise.IsNumeric)
            return "Wykonaj obliczenia etapami i na końcu sprawdź znak, jednostkę oraz warunki otrzymanego wyniku.";
        if (exercise.IsCompound)
            return "Uzupełniaj części po kolei i po każdej sprawdzaj, czy nie ogranicza ona możliwych odpowiedzi w kolejnej części.";

        return "Zapisz kluczowe przekształcenie albo konstrukcję na brudnopisie, zanim świadomie ujawnisz pełne rozwiązanie.";
    }
}
