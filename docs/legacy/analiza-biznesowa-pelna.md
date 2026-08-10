# Historyczna analiza biznesowa

> **Status:** historyczna transkrypcja przekazana przy Issue #9 9 sierpnia 2026 r.
>
> Treść poniżej nie jest źródłem prawdy dla bieżącej implementacji AvaloniaUI. Opisuje historyczne założenia, planowane technologie i terminy projektu. Aktualny stan opisują [analiza biznesowa](../BUSINESS_ANALYSIS.md), [wymagania](../REQUIREMENTS.md) oraz [architektura](../ARCHITECTURE.md).

---

# Analiza biznesowa

## 1. Cel analizy biznesowej

Celem analizy biznesowej projektu **Abituria** jest określenie potrzeb użytkowników, zakresu tworzonego systemu, sposobu jego realizacji i udostępnienia oraz warunków, które muszą zostać spełnione, aby projekt można było uznać za zakończony.

Analiza obejmuje:

* określenie celów projektu z punktu widzenia użytkownika i zamawiającego;
* identyfikację potrzeb użytkownika końcowego;
* określenie modelu udostępniania produktu;
* zdefiniowanie zakresu projektu;
* przygotowanie harmonogramu i kamieni milowych;
* określenie kryteriów akceptacji;
* ustalenie modelu licencyjnego;
* zebranie i uporządkowanie wymagań;
* opracowanie architektury systemu.

---

## 2. Charakterystyka projektu

**Abituria** jest aplikacją edukacyjną wspierającą samodzielną naukę matematyki, ze szczególnym uwzględnieniem materiału szkoły średniej oraz przygotowania do egzaminu maturalnego.

System ma łączyć w jednej aplikacji:

* materiały teoretyczne;
* przykłady rozwiązań;
* ćwiczenia;
* zadania maturalne;
* automatyczne sprawdzanie odpowiedzi;
* podpowiedzi prezentowane krok po kroku;
* narzędzia obliczeniowe i kalkulatory.

Podstawową wartością projektu jest stworzenie narzędzia, które nie tylko prezentuje wynik, lecz wspiera użytkownika w samodzielnym dochodzeniu do rozwiązania.

---

# 3. Cele projektu

## 3.1. Cel główny

Głównym celem projektu jest stworzenie łatwego w obsłudze narzędzia umożliwiającego uczniowi samodzielną naukę i utrwalanie wiedzy matematycznej.

System powinien pomagać zarówno w poznawaniu nowych zagadnień, jak i w uzupełnianiu braków z materiału omawianego wcześniej w szkole.

## 3.2. Cele szczegółowe

Projekt powinien umożliwiać:

1. uporządkowane przeglądanie materiałów matematycznych;
2. zapoznawanie się z teorią i wzorami;
3. analizowanie przykładów;
4. samodzielne rozwiązywanie zadań;
5. sprawdzanie poprawności odpowiedzi;
6. otrzymywanie informacji zwrotnej;
7. korzystanie ze stopniowanych podpowiedzi;
8. wykonywanie obliczeń za pomocą kalkulatora;
9. rozwiązywanie wybranych zadań maturalnych;
10. losowanie zadań z przygotowanej puli;
11. rozwijanie aplikacji o kolejne działy i funkcje.

---

# 4. Użytkownicy i ich potrzeby

## 4.1. Główny użytkownik

Podstawowym użytkownikiem systemu jest uczeń szkoły średniej, w szczególności osoba przygotowująca się do egzaminu maturalnego.

Aplikacja może być również wykorzystywana przez osoby chcące powtórzyć podstawowe zagadnienia matematyczne.

## 4.2. Potrzeby użytkownika

Użytkownik oczekuje przede wszystkim:

* prostego i zrozumiałego interfejsu;
* szybkiego dostępu do wybranego zagadnienia;
* czytelnych materiałów teoretycznych;
* możliwości ćwiczenia poznanego materiału;
* natychmiastowej informacji o poprawności rozwiązania;
* pomocy w przypadku problemów;
* możliwości nauki we własnym tempie;
* łatwego powrotu do wcześniejszych ekranów;
* stabilnego działania aplikacji;
* bezpłatnego dostępu do podstawowych funkcji.

Szczególnie istotny jest mechanizm podpowiedzi. Użytkownik nie powinien otrzymywać od razu całego rozwiązania. Powinien móc stopniowo odsłaniać kolejne etapy, dzięki czemu nadal samodzielnie uczestniczy w procesie rozwiązywania zadania.

---

# 5. Model biznesowy

Abituria jest projektem edukacyjnym przeznaczonym do bezpłatnego udostępnienia użytkownikom.

Podstawowy model zakłada:

* brak opłat za korzystanie z aplikacji;
* publiczne udostępnienie wersji programu;
* dostęp do kodu źródłowego;
* rozwój projektu w modelu open source;
* możliwość dalszego rozwijania aplikacji przez autorów lub społeczność.

Projekt nie zakłada w podstawowym zakresie:

* płatnych subskrypcji;
* reklam;
* mikropłatności;
* sprzedaży danych użytkowników;
* płatnych funkcji premium.

Ewentualna komercjalizacja lub rozbudowa modelu biznesowego może zostać rozważona w przyszłości, ale nie stanowi podstawowego celu projektu.

---

# 6. Zakres projektu

## 6.1. Zakres funkcjonalny

Podstawowa wersja systemu obejmuje:

### Ekran główny

Użytkownik powinien mieć dostęp do podstawowych segmentów aplikacji:

* kalkulatora;
* działów matematyki;
* ćwiczeń;
* zadań maturalnych;
* opcjonalnie materiałów wideo.

### Kalkulator

Kalkulator powinien obsługiwać co najmniej:

* dodawanie;
* odejmowanie;
* mnożenie;
* dzielenie;
* nawiasy;
* potęgowanie;
* pierwiastkowanie.

Wielokrotne wybranie kalkulatora nie powinno tworzyć nieograniczonej liczby jego okien.

### Materiały matematyczne

Planowany zakres obejmuje między innymi:

* liczby naturalne;
* aksjomatykę liczb naturalnych;
* indukcję matematyczną;
* działania na liczbach naturalnych;
* alfabet grecki;
* wyrażenia algebraiczne;
* wzory skróconego mnożenia;
* równania i nierówności;
* równania kwadratowe;
* potęgi i pierwiastki;
* przedziały liczbowe;
* procenty;
* logarytmy;
* liczby rzeczywiste i zbiory.

### Zadania

System powinien umożliwiać:

* rozwiązywanie zadań wprowadzających;
* rozwiązywanie wybranych zadań maturalnych;
* sprawdzanie odpowiedzi;
* otrzymywanie informacji zwrotnej;
* korzystanie z kolejnych podpowiedzi;
* losowanie zadań z dostępnej puli.

---

# 7. Elementy opcjonalne

W zależności od dostępnego czasu projekt może zostać rozszerzony o:

* system logowania;
* konta użytkowników;
* zapisywanie postępów;
* historię rozwiązanych zadań;
* materiały wideo;
* bardziej zaawansowane kalkulatory;
* personalizację poziomu trudności;
* statystyki nauki;
* dodatkowe działy matematyczne.

Funkcje te nie powinny opóźniać ukończenia obowiązkowego zakresu projektu.

---

# 8. Elementy poza zakresem

W podstawowej wersji poza zakresem znajdują się:

* aplikacja mobilna;
* pełna wersja internetowa;
* system płatności;
* rozbudowany system kont użytkowników;
* synchronizacja danych pomiędzy urządzeniami;
* rozbudowany backend internetowy;
* automatyczne pobieranie zadań z zewnętrznych serwisów;
* pełna platforma do hostowania materiałów wideo;
* automatyczne generowanie całego materiału edukacyjnego.

---

# 9. Metodyka realizacji

Projekt jest rozwijany **iteracyjnie i przyrostowo**.

Oznacza to, że zamiast tworzyć cały system jednocześnie, kolejne części aplikacji powstają w następnych przyrostach. Po każdym przyroście otrzymywana jest działająca wersja możliwa do zaprezentowania i przetestowania.

Proces obejmuje:

1. zebranie wymagań;
2. przygotowanie prototypu;
3. implementację;
4. testowanie;
5. odbiór przyrostu;
6. zebranie uwag;
7. wprowadzenie poprawek;
8. rozpoczęcie następnej iteracji.

Realizacja kolejnych wersji uwzględnia również testy użyteczności.

---

# 10. Harmonogram i kamienie milowe

## Przyrost I - połowa maja

Pierwszy działający prototyp.

Powinien prezentować:

* podstawowy interfejs;
* nawigację;
* fragment kalkulatora;
* przykładowy moduł edukacyjny.

**Kamień milowy M1:** działający prototyp możliwy do zaprezentowania prowadzącemu.

---

## Przyrost II - do 11 czerwca

W pełni działający produkt pokrywający część wymagań zaakceptowaną przez prowadzącego.

Powinien zawierać między innymi:

* stabilną nawigację;
* kalkulator;
* wybrane materiały;
* podstawowe zadania;
* sprawdzanie odpowiedzi;
* obsługę błędów.

**Kamień milowy M2:** działający produkt realizujący uzgodnioną część wymagań.

---

## Przyrost III - połowa semestru

Rozszerzenie systemu o:

* kolejne materiały;
* zadania;
* podpowiedzi;
* poprawki interfejsu;
* wyniki testów użyteczności;
* poprawki błędów.

**Kamień milowy M3:** rozszerzona i przetestowana wersja aplikacji.

---

## Przyrost IV - wersja końcowa

Końcowa wersja powinna realizować całość zatwierdzonych wymagań.

Powinna być:

* w pełni działająca;
* przetestowana;
* udokumentowana;
* przygotowana do wdrożenia.

**Kamień milowy M4:** końcowa wersja produktu.

---

## Początek stycznia

Dostarczenie dokumentacji technicznej dla komisji.

**Kamień milowy M5:** kompletna dokumentacja techniczna zgodna z końcową wersją systemu.

---

## Po 17 stycznia 2022

Publiczna obrona projektu przed komisją.

**Kamień milowy M6:** prezentacja oraz publiczna obrona działającego produktu.

---

# 11. Kryteria akceptacji

Produkt można uznać za spełniający wymagania, jeżeli:

1. aplikacja poprawnie się uruchamia;
2. użytkownik może poruszać się pomiędzy podstawowymi modułami;
3. kalkulator wykonuje zadeklarowane operacje;
4. użytkownik może korzystać z materiałów teoretycznych;
5. użytkownik może rozwiązywać zadania;
6. system sprawdza odpowiedzi;
7. dostępne są podpowiedzi;
8. aplikacja prawidłowo reaguje na błędne dane;
9. nie występują błędy krytyczne;
10. interfejs jest spójny i czytelny;
11. aplikacja została przetestowana;
12. dokumentacja odpowiada rzeczywistej implementacji;
13. wersja końcowa została przygotowana do wdrożenia.

Projekt aspirujący do oceny bardzo dobrej musi być zarówno **przetestowany, jak i wdrożony**.

---

# 12. Model licencyjny

Kod źródłowy projektu Abituria pozostaje udostępniany na licencji **MIT**.

Licencja MIT została przyjęta ze względu na:

* prostotę;
* liberalne zasady korzystania;
* możliwość modyfikowania kodu;
* możliwość jego dalszego rozpowszechniania;
* zgodność z dotychczasowym modelem repozytorium.

Licencja projektu nie oznacza automatycznie, że wszystkie materiały znajdujące się w repozytorium mogą być rozpowszechniane na tych samych zasadach.

Treści powinny być rozpatrywane indywidualnie:

* własny kod - MIT;
* odziedziczone materiały MIT - zgodnie z pierwotną licencją;
* własna dokumentacja i nowe własne materiały - możliwość osobnego licencjonowania;
* materiały zewnętrzne - zgodnie z prawami i licencją ich właściciela;
* zasoby o niepotwierdzonych prawach - nie powinny znaleźć się w publicznym wydaniu.

---

# 13. Zebranie wymagań

Wymagania projektu zostały podzielone na następujące grupy:

* wymagania funkcjonalne;
* wymagania niefunkcjonalne;
* wymagania dotyczące danych i prywatności;
* wymagania testowe;
* kryteria akceptacji;
* elementy poza zakresem.

## Przykładowe wymaganie funkcjonalne

**WF-01:** System powinien umożliwiać użytkownikowi wybór działu matematycznego.

**Kryteria akceptacji:**

* użytkownik widzi dostępne działy;
* może wybrać jeden z nich;
* po wyborze zostaje wyświetlona odpowiednia treść;
* może wrócić do poprzedniego widoku.

## Przykładowe wymaganie niefunkcjonalne

**WNF-01:** Aplikacja nie może tworzyć nieograniczonej liczby identycznych okien prowadzących do nadmiernego zużycia pamięci.

**Kryteria akceptacji:**

* wielokrotne wybranie tej samej funkcji nie powoduje niekontrolowanego tworzenia nowych instancji okien;
* podczas standardowej pracy nie następuje ciągły, nieuzasadniony wzrost wykorzystania pamięci.

---

# 14. Architektura systemu

System został zaprojektowany jako aplikacja desktopowa.

Podstawowe technologie:

* **C#** - język programowania;
* **.NET** - platforma wykonawcza;
* **WPF** - technologia interfejsu użytkownika pierwotnej wersji systemu;
* **XAML** - definiowanie interfejsu;
* **Visual Studio** - środowisko programistyczne;
* **NuGet** - obsługa bibliotek zewnętrznych;
* **Git** - kontrola wersji;
* **GitHub** - repozytorium i zarządzanie rozwojem projektu.

Architektura powinna rozdzielać:

### Warstwę prezentacji

Odpowiada za:

* ekrany;
* kontrolki;
* nawigację;
* prezentowanie wyników;
* komunikaty dla użytkownika.

### Warstwę logiki aplikacji

Odpowiada za:

* obliczenia matematyczne;
* sprawdzanie odpowiedzi;
* generowanie lub wybieranie zadań;
* zarządzanie podpowiedziami;
* sterowanie przebiegiem pracy aplikacji.

### Warstwę danych

Odpowiada za:

* materiały teoretyczne;
* zadania;
* odpowiedzi;
* podpowiedzi;
* konfigurację aplikacji;
* opcjonalnie dane dotyczące postępów użytkownika.

---

# 15. Ryzyka projektu

| Ryzyko | Możliwy skutek | Ograniczenie ryzyka |
| --- | --- | --- |
| Zbyt szeroki zakres | Nieukończenie podstawowej wersji | Priorytetyzacja funkcji |
| Niedokończona dokumentacja | Problemy przy odbiorze | Aktualizacja po każdym przyroście |
| Błędy w nawigacji | Niestabilność i problemy z pamięcią | Jeden spójny mechanizm nawigacji |
| Błędy matematyczne | Niepoprawne wyniki | Testy obliczeń |
| Brak testów regresyjnych | Powracające błędy | Testowanie przed każdym wydaniem |
| Niejasne prawa do treści | Brak możliwości publikacji | Ewidencja pochodzenia materiałów |
| Rozbudowa funkcji przed ukończeniem podstaw | Opóźnienie projektu | Funkcje dodatkowe dopiero po spełnieniu kryteriów podstawowych |

---

# 16. Podsumowanie analizy biznesowej

Abituria odpowiada na potrzebę stworzenia jednego, spójnego środowiska wspierającego naukę matematyki poprzez połączenie materiałów teoretycznych, ćwiczeń, automatycznego sprawdzania odpowiedzi, podpowiedzi i kalkulatora.

Projekt jest realizowany iteracyjnie, dzięki czemu funkcjonalność może być stopniowo rozwijana i weryfikowana podczas kolejnych odbiorów.

Priorytetem jest ukończenie stabilnej, przetestowanej i wdrożonej wersji realizującej zatwierdzone wymagania. Dopiero po osiągnięciu tego celu powinny być dodawane funkcje opcjonalne oraz kolejne treści edukacyjne.
