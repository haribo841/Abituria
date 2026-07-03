# Macierz testów kalkulatora

## Cel

Zestaw chroni obecny kalkulator przed regresjami opisanymi w historycznych issues:

- [#1](https://github.com/Projekt-Inzynierski-AK-AS-FD/Projekt-Inzynierski/issues/1) - `FormatException` podczas dzielenia fragmentów parsowanych przez `double.Parse`;
- [#2](https://github.com/Projekt-Inzynierski-AK-AS-FD/Projekt-Inzynierski/issues/2) - problemy starej nawigacji WPF opartej na `Page` i `NavigationWindow`;
- [#3](https://github.com/Projekt-Inzynierski-AK-AS-FD/Projekt-Inzynierski/issues/3) - `FormatException` podczas sprawdzania zera w złożonych wyrażeniach, w tym `0√0:0√0`;
- [#4](https://github.com/Projekt-Inzynierski-AK-AS-FD/Projekt-Inzynierski/issues/4) - błędy `1/x`, `NaN`, konkatenacja cyfr po wyniku i niepoprawne składanie odwrotności z operatorami.
- [#5](https://github.com/Projekt-Inzynierski-AK-AS-FD/Projekt-Inzynierski/issues/5) - pierwiastkowanie zwykłych i ekstremalnych liczb w notacji naukowej oraz pierwiastki po operatorach i wyniku.
- [#7](https://github.com/Projekt-Inzynierski-AK-AS-FD/Projekt-Inzynierski/issues/7) - niedozwolone `1/∞` oraz zapętlenie `1/0` i `1/x`.
- [#8](https://github.com/Projekt-Inzynierski-AK-AS-FD/Projekt-Inzynierski/issues/8) - `FormatException` dla `1/x` z pierwiastkiem, potęgowanie liczb zmiennoprzecinkowych i odtwarzanie historii.
- [#9](https://github.com/Projekt-Inzynierski-AK-AS-FD/Projekt-Inzynierski/issues/9) - fałszywe dzielenie przez zero i niekanoniczne liczby z wieloma zerami wiodącymi.

## Warstwy pokrycia

| Warstwa | Zakres |
|---|---|
| Regresje historyczne | Dokładne teksty z issues i komentarzy, między innymi `0√0:0√0`, `x:(1/x)`, `0:1/x`, `1/NaN9` i `9*(1/9)` |
| Macierze operatorów | Wszystkie pary przygotowanych reprezentatywnych operandów dla `+`, `-`, `*`, `×`, `/`, `÷` i `^` |
| Pierwszeństwo | Każda para operatorów porównana z jawnym, oczekiwanym grupowaniem nawiasami |
| Pierwiastki | Stopnie od 2 do 15, wartości dodatnie i dozwolone ujemne, wszystkie notacje oraz połączenia z operatorami |
| Duże pierwiastki | Bezpośrednie przypadki do `1E308`, `double.MaxValue`, duże wartości ujemne dla stopni nieparzystych i kontrolowane przekroczenie zakresu |
| Znaki unarne | Wszystkie 1020 kombinacji od jednego do ośmiu znaków `+` i `-` dla potęgi i pierwiastka |
| Tokeny i błędy | Około 204 tysiące kombinacji fragmentów tokenów do długości czterech, bez niekontrolowanych wyjątków |
| Fuzzing deterministyczny | 20 tysięcy uszkodzonych napisów i 10 tysięcy generowanych drzew poprawnych wyrażeń |
| Zakres `double` | 20 tysięcy losowych skończonych wzorców bitowych sprawdzonych przez zapis round-trip |
| Współbieżność | 50 tysięcy równoległych obliczeń na jednej instancji parsera |
| `1/x` | Setki kolejnych odwrotności dla siatki liczb małych, dużych, ujemnych, pierwiastków i notacji naukowej |
| `∞` | Symbol `∞` i aliasy tekstowe w operandach, potęgach, nawiasach, pierwiastkach oraz na każdej pozycji reprezentatywnych wyrażeń |
| `1/x` po błędzie | 250 kolejnych kliknięć dla każdego wariantu dzielenia przez zero bez zmiany wyrażenia, `Ans`, historii i kodu błędu |
| Wielokrotne `=` | Powtarzanie ostatniej operacji dla wszystkich operatorów, złożonych prawych argumentów, `Ans`, błędów, przepełnienia i limitu historii |
| `Ans` i historia | Wszystkie operatory i pierwiastki z `Ans`, błędy bez zmiany stanu, limit historii oraz długa sesja mieszana |
| Odtwarzanie historii | Pierwotny kontekst `Ans`, potęgi zmiennoprzecinkowe, wszystkie operatory, złożone pierwiastki, `1/x`, kolejne `=` i odrzucone nieaktualne wpisy |
| Zera wiodące | Dokładne przypadki issue #9, normalizacja każdego literału, komunikat użytkownika, klawiatura ekranowa, wklejanie, historia i rzeczywiste dzielenie przez zero |
| `Ans` z pierwiastkiem | `1/(Ans∛(Ans))` dla wartości dodatnich, ujemnych, zera, małych liczb, brakującego `Ans` i niedomiaru zakresu |
| `x²` | Wynik, całe wyrażenie, zaznaczenie, pusty szablon, historia, kolejne `=`, liczby zmiennoprzecinkowe i przepełnienie |
| Kultury systemowe | Identyczne wyniki dla `pl-PL`, `en-US`, `de-DE`, `fr-FR` i `tr-TR` |
| Nawigacja | Brak zależności WPF `Page` i `NavigationWindow`, wszystkie widoki jako `UserControl`, jeden skalowalny `ShellHost` i minimalny rozmiar `960x640` |

Przestrzeń wszystkich możliwych napisów jest nieskończona, dlatego zestaw łączy wyczerpujące macierze ograniczonych alfabetów z generowaniem deterministycznym, testami granic i dokładnymi regresjami.

## Bramy

```powershell
dotnet test Abituria.sln --configuration Release
dotnet test Abituria.sln --configuration Debug
dotnet format Abituria.sln whitespace --verify-no-changes
git diff --check
```
