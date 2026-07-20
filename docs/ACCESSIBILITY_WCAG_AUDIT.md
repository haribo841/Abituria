# Audyt dostępności WCAG 2.2 A/AA

Wersja aplikacji: `0.9.0-beta.1`.

Data przeglądu: 20 lipca 2026 r.

Zakres: bieżąca aplikacja desktopowa Abituria w AvaloniaUI 12, jej główne widoki, własny pasek tytułu, dialogi, warianty motywu i zachowanie układu przy zmianie rozmiaru okna.

## Charakter i ograniczenia audytu

[WCAG 2.2](https://www.w3.org/TR/WCAG22/) jest standardem dostępności treści internetowych. Abituria jest aplikacją desktopową, dlatego kryteria zastosowano jako rygorystyczny wzorzec projektowy i testowy, z interpretacją odpowiednią dla kontrolek Avalonia, natywnego okna i systemowego drzewa automatyzacji. Ten dokument:

- przegląda wszystkie 55 obowiązujących kryteriów poziomów A i AA;
- nie obejmuje kryterium 4.1.1, które w WCAG 2.2 jest oznaczone jako usunięte;
- rozróżnia dowody automatyczne, przegląd techniczny, kryteria nieodpowiednie dla produktu oraz czynności wymagające człowieka;
- nie jest certyfikatem WCAG ani formalną deklaracją zgodności;
- nie twierdzi, że bieżąca wersja była testowana z czytnikiem ekranu, monitorem brajlowskim, sterowaniem głosowym lub przełącznikami;
- nie zastępuje testów z osobami korzystającymi z technologii asystujących.

Ocena kompletności audytu oznacza, że każde kryterium A/AA zostało rozpatrzone. Nie oznacza automatycznie pełnej zgodności AA. Zgodnie z modelem W3C poziom AA wymaga spełnienia wszystkich kryteriów A i AA w całym procesie, także we wszystkich wariantach responsywnych.

## Metoda i dowody

Przegląd łączy następujące źródła:

1. analizę `AppStyles.axaml`, `MainWindow.axaml`, widoków C# i właściwości `AutomationProperties`;
2. testy Avalonia Headless obejmujące nazwy automatyzacji, regiony live, widoczny fokus, motywy, własny chrome, breakpointy i renderowanie;
3. obliczenie kontrastu kolorów zgodnie z definicją [WCAG 2.2, 1.4.3](https://www.w3.org/WAI/WCAG22/Understanding/contrast-minimum.html) oraz [1.4.11](https://www.w3.org/WAI/WCAG22/Understanding/non-text-contrast.html);
4. przegląd logicznej kolejności kontrolek, obsługi klawiatury, walidacji i komunikatów dynamicznych;
5. techniczny przegląd układów przy szerokościach powyżej i poniżej breakpointów `860`, `780` i `900` pikseli niezależnych od urządzenia.

Legenda statusów:

| Status | Znaczenie |
| --- | --- |
| `P` | Pozytywna ocena techniczna na podstawie implementacji i powtarzalnego dowodu. |
| `M` | Implementacja ma wymagane mechanizmy, ale do pełnego potwierdzenia potrzebny jest test manualny lub technologia asystująca. |
| `N/D` | Kryterium nie dotyczy bieżącego produktu lub jest spełnione przez wyjątek zapisany w kryterium. |
| `L` | Otwarta luka. Taki status blokuje deklarację zgodności na wskazanym poziomie. |

## Zrealizowane mechanizmy dostępności

- aplikacja używa lokalnego kroju Mulish, a wzory matematyczne renderuje wyspecjalizowana kontrolka CSharpMath;
- motyw nie jest wymuszony na jasny: dostępne są ustawienie systemowe oraz warianty jasny, ciemny i wysoki kontrast;
- paleta jest oparta na zasobach dynamicznych, więc zmiana motywu aktualizuje istniejące widoki;
- przyciski, pola tekstowe i pola wyboru mają jawne stany `:pointerover`, `:pressed`, `:focus` i `:focus-visible`;
- stan fokusu ma kontrastową ramkę, a wynik renderowania jest chroniony testem headless;
- własny pasek tytułu ma opisane przyciski motywu, minimalizacji, maksymalizacji lub przywrócenia oraz zamknięcia;
- przenoszenie okna, dwuklik maksymalizujący i osiem uchwytów zmiany rozmiaru korzystają z natywnych operacji Avalonia;
- układ logowania przechodzi do jednej kolumny poniżej `860`, ekran Start poniżej `780`, a kalkulator ogólny poniżej `900`;
- dialogi kodu odzyskiwania są skalowalne, mają minimalne i maksymalne wymiary oraz przewijanie;
- komunikaty konta, sprawdzania zadania i kalkulatorów mają nazwy automatyzacji oraz ustawienie live `Polite`;
- symboliczne przyciski, obrazy treści i pola wejściowe otrzymują opisowe nazwy automatyzacji.
- placeholdery pól używają kontrastowego `TextMutedBrush`, a czarny napis w rastrowym logo jest prezentowany na stałej jasnej powierzchni także w motywie ciemnym i wysokiego kontrastu.

## Kontrast palet

Poniższe wartości są minimalnymi reprezentatywnymi parami sprawdzanymi dla zasobów aplikacji. Wartość dla zwykłego tekstu musi wynosić co najmniej `4,5:1`, a dla dużego tekstu, granic kontrolek i wskaźników stanu co najmniej `3:1`.

| Wariant i para | Współczynnik | Wynik |
| --- | ---: | --- |
| Jasny: tekst podstawowy `#18212B` na powierzchni `#FFFFFF` | `16,26:1` | PASS |
| Jasny: tekst pomocniczy `#52606D` na powierzchni `#FFFFFF` | `6,46:1` | PASS |
| Jasny: biały tekst na akcencie `#126782` | `6,39:1` | PASS |
| Jasny: granica `#667685` na powierzchni `#FFFFFF` | `4,67:1` | PASS |
| Ciemny: tekst podstawowy `#F5F7FA` na powierzchni `#1B2630` | `14,32:1` | PASS |
| Ciemny: tekst pomocniczy `#B9C4CE` na powierzchni `#1B2630` | `8,67:1` | PASS |
| Ciemny: tekst `#071318` na akcencie `#5FC3DE` | `9,28:1` | PASS |
| Ciemny: granica `#91A0AD` na powierzchni `#1B2630` | `5,73:1` | PASS |
| Wysoki kontrast: biały tekst na czarnym tle | `21:1` | PASS |
| Wysoki kontrast: czarny tekst na akcencie `#00FFFF` | `16,75:1` | PASS |
| Wysoki kontrast: fokus `#FFFF00` na czarnym tle | `19,56:1` | PASS |

Wartości dotyczą zdefiniowanych zasobów palety. Test renderowania pozostaje potrzebny, ponieważ lokalna wartość koloru w pojedynczej kontrolce mogłaby mieć wyższy priorytet niż styl globalny.

## Macierz kryteriów WCAG 2.2 A/AA

### 1. Postrzegalność

| Kryterium | Poziom | Status | Ocena i dowód |
| --- | --- | --- | --- |
| 1.1.1 Non-text Content | A | `M` | `UiFactory.AssetImage` ustawia tekst alternatywny, kafle mają nazwy i opisy automatyzacji, a przyciski symboliczne mają nazwy opisujące działanie. Należy potwierdzić na natywnym drzewie automatyzacji, że obrazy dekoracyjne nie są ogłaszane, a wszystkie ilustracje edukacyjne mają użyteczny opis. |
| 1.2.1 Audio-only and Video-only (Prerecorded) | A | `N/D` | Aplikacja nie zawiera audio ani wideo. |
| 1.2.2 Captions (Prerecorded) | A | `N/D` | Aplikacja nie zawiera zsynchronizowanych nagrań. |
| 1.2.3 Audio Description or Media Alternative (Prerecorded) | A | `N/D` | Aplikacja nie zawiera nagrań wideo. |
| 1.2.4 Captions (Live) | AA | `N/D` | Aplikacja nie transmituje dźwięku ani obrazu na żywo. |
| 1.2.5 Audio Description (Prerecorded) | AA | `N/D` | Aplikacja nie zawiera nagrań wideo. |
| 1.3.1 Info and Relationships | A | `M` | Nagłówki, grupy formularzy, listy i etykiety są czytelne wizualnie, a najważniejsze pola mają nazwy automatyzacji. Pełna semantyka nagłówków i relacji etykieta-pole wymaga inspekcji natywnego drzewa UI Automation. |
| 1.3.2 Meaningful Sequence | A | `P` | Kolejność logicznego drzewa odpowiada kolejności czytania. Breakpointy zmieniają pozycje w `Grid`, ale nie przestawiają elementów w drzewie logicznym. |
| 1.3.3 Sensory Characteristics | A | `P` | Instrukcje używają nazw działań i tekstowych komunikatów, nie wyłącznie koloru, położenia lub kształtu. Strzałki nawigacyjne mają opisowe nazwy automatyzacji. |
| 1.3.4 Orientation | AA | `N/D` | Aplikacja desktopowa nie blokuje orientacji urządzenia. Okno jest skalowalne w obu osiach. |
| 1.3.5 Identify Input Purpose | AA | `N/D` | Kryterium opiera się na programowych tokenach celu wejścia dostępnych w technologiach webowych. W aplikacji lokalnej pola mają opisowe nazwy automatyzacji, ale nie istnieje odpowiednik HTML `autocomplete`. |
| 1.4.1 Use of Color | A | `P` | Sukces, błąd, status roadmapy i ukończenie zadania mają tekst lub symbol oprócz koloru. |
| 1.4.2 Audio Control | A | `N/D` | Aplikacja nie odtwarza dźwięku. |
| 1.4.3 Contrast (Minimum) | AA | `P` | Reprezentatywne pary zwykłego tekstu i placeholderów we wszystkich trzech paletach przekraczają `4,5:1`. Czarny napis rastrowego logo ma dedykowane białe tło. Wartości są zapisane w tabeli kontrastu i chronione testem zasobów oraz właściwości kontrolek. |
| 1.4.4 Resize Text | AA | `M` | Układy zawijają tekst, przewijają treść i reagują na mniejszą szerokość. Należy wykonać ręczny przegląd przy systemowym skalowaniu tekstu i DPI do odpowiednika `200%` na każdej wspieranej platformie. |
| 1.4.5 Images of Text | AA | `P` | Tekst interfejsu jest renderowany jako tekst. Obraz logo korzysta z wyjątku dla logotypu, a grafiki matematyczne są częścią materiału edukacyjnego i mają nazwy alternatywne. |
| 1.4.10 Reflow | AA | `M` | Login, Start i kalkulator mają jawne breakpointy, pozostałe widoki zawijają tekst i przewijają się pionowo. Desktopowe minimum `720x520` nie jest równoważne wymaganiu webowemu `320` pikseli CSS, dlatego nie składamy ścisłej deklaracji dla tego kryterium. |
| 1.4.11 Non-text Contrast | AA | `P` | Granice, fokus i aktywne kontrolki mają co najmniej `3:1`; przykładowo jasna granica ma `4,67:1`, ciemna `5,73:1`, a fokus wysokiego kontrastu `19,56:1`. |
| 1.4.12 Text Spacing | AA | `M` | Mulish, zawijanie i elastyczne wysokości nie zakładają stałych metryk w głównych widokach. Należy potwierdzić brak ucięć po zwiększeniu odstępów liter, słów, linii i akapitów w środowisku pozwalającym wymusić takie ustawienia. |
| 1.4.13 Content on Hover or Focus | AA | `M` | Dodatkową treścią na hover są głównie natywne tooltipy paska okna i nawigacji zadań. Ich możliwość ukrycia, utrzymanie po najechaniu i trwałość do czasu odsunięcia fokusu wymagają kontroli manualnej na każdej platformie. |

### 2. Funkcjonalność

| Kryterium | Poziom | Status | Ocena i dowód |
| --- | --- | --- | --- |
| 2.1.1 Keyboard | A | `M` | Standardowe kontrolki są osiągalne klawiaturą, kalkulator obsługuje `Enter`, `Escape` i edycję, a funkcje własnego chrome mają przyciski. Pełny scenariusz klawiaturowy wszystkich widoków wymaga ręcznego przejścia na natywnym backendzie. |
| 2.1.2 No Keyboard Trap | A | `M` | Nie ma celowo blokujących kontrolek; dialogi są modalne i można je zamknąć przyciskiem. Należy potwierdzić przejście `Tab` i `Shift+Tab` przez oba dialogi oraz powrót fokusu do właściciela. |
| 2.1.4 Character Key Shortcuts | A | `P` | Jednoznakowe skróty kalkulatora działają wyłącznie w polu edycji wyrażenia, co spełnia wyjątek dla aktywnej kontrolki tekstowej. |
| 2.2.1 Timing Adjustable | A | `N/D` | Aplikacja nie nakłada limitów czasu na naukę, formularze ani sesję. |
| 2.2.2 Pause, Stop, Hide | A | `N/D` | Interfejs nie zawiera automatycznie poruszającej się, migającej ani aktualizowanej okresowo treści. |
| 2.3.1 Three Flashes or Below Threshold | A | `N/D` | Aplikacja nie zawiera migających animacji ani materiałów wideo. |
| 2.4.1 Bypass Blocks | A | `N/D` | To kryterium dotyczy powtarzanych bloków stron internetowych. W desktopowym shellu widok jest wymieniany wewnątrz jednego hosta, a nawigacja pozostaje osobną grupą kontrolek. |
| 2.4.2 Page Titled | A | `P` | Główne okno ma tytuł `Abituria`, a każdy widok ma jednoznaczny widoczny nagłówek. |
| 2.4.3 Focus Order | A | `M` | Kolejność tworzenia kontrolek odpowiada kolejności wizualnej także po reflow. Wymagany jest końcowy manualny zapis przejścia fokusu dla loginu, nawigacji, zadania, obu kalkulatorów i dialogów. |
| 2.4.4 Link Purpose (In Context) | A | `P` | Przyciski mają opisowe etykiety, a przyciski strzałek i własnego chrome dodatkowo nazwy automatyzacji i tooltipy. |
| 2.4.5 Multiple Ways | AA | `P` | Główne moduły są dostępne z górnej nawigacji i kafli Start. Listy, tematy oraz przyciski powrotu zapewniają przewidywalne alternatywne dojście do treści. |
| 2.4.6 Headings and Labels | AA | `P` | Nagłówki opisują cel widoku, a etykiety i placeholdery określają wymagany format lub działanie. |
| 2.4.7 Focus Visible | AA | `P` | Style zawierają jawne `:focus` i `:focus-visible`, a regresja renderowania sprawdza zmianę pikseli po uzyskaniu fokusu klawiaturowego. Zasada odpowiada wyjaśnieniu W3C dla [widocznego fokusu](https://www.w3.org/WAI/WCAG22/Understanding/focus-visible.html). |
| 2.4.11 Focus Not Obscured (Minimum) | AA | `M` | Widoki i dialogi przewijają zawartość, a pasek tytułu nie nakłada się na `ShellHost`. Należy ręcznie przejść wszystkie kontrolki przy minimalnym oknie i skalowaniu `200%`, aby potwierdzić automatyczne dosuwanie fokusu do widocznego obszaru. |
| 2.5.1 Pointer Gestures | A | `N/D` | Nie ma gestów wielopunktowych ani gestów zależnych od rysowanego toru. |
| 2.5.2 Pointer Cancellation | A | `P` | Akcje aplikacji korzystają ze standardowego zdarzenia aktywacji przycisku. Naciśnięcie paska lub uchwytu rozpoczyna wyłącznie odwracalną operację systemową przenoszenia albo zmiany rozmiaru. |
| 2.5.3 Label in Name | A | `P` | Nazwy automatyzacji zawierają widoczną etykietę lub dokładniej ją rozwijają, na przykład `Poprzednie zadanie: <tytuł>` i `Maksymalizuj okno`. |
| 2.5.4 Motion Actuation | A | `N/D` | Aplikacja nie wymaga ruchu urządzenia ani gestu kamery. |
| 2.5.7 Dragging Movements | AA | `N/D` | Treść aplikacji nie wymaga przeciągania. Przenoszenie i skalowanie okna to operacje menedżera okien, dostępne również z klawiaturowego menu systemowego. |
| 2.5.8 Target Size (Minimum) | AA | `M` | Kontrolki paska okna mają co najmniej `48x45`, a klawisze kalkulatora co najmniej `42` lub `46` pikseli wysokości. Należy wykonać końcowy pomiar wszystkich małych celów, w szczególności nawigacji zadań i pasków przewijania, na trzech backendach platformowych. |

### 3. Zrozumiałość

| Kryterium | Poziom | Status | Ocena i dowód |
| --- | --- | --- | --- |
| 3.1.1 Language of Page | A | `M` | Bootstrap ustawia `DefaultThreadCurrentUICulture` i `CurrentUICulture` na `pl-PL`, a treść i komunikaty są po polsku. Należy potwierdzić, że natywny peer okna przekazuje ten język technologii asystującej; sama kultura wątku nie jest wystarczającym dowodem. |
| 3.1.2 Language of Parts | AA | `N/D` | Nie występują dłuższe fragmenty w innym języku wymagające zmiany języka syntezy. Symbole, nazwy własne i krótkie oznaczenia matematyczne podlegają wyjątkowi. |
| 3.2.1 On Focus | A | `P` | Samo uzyskanie fokusu nie uruchamia nawigacji, logowania, obliczenia ani zamknięcia okna. |
| 3.2.2 On Input | A | `P` | Zmiana wartości pola nie wykonuje nieoczekiwanej operacji. Wybór profilu aktualizuje przewidywalnie stan formularza, a działania wymagają przycisku lub jawnego `Enter`. |
| 3.2.3 Consistent Navigation | AA | `P` | Ten sam górny pasek i kolejność modułów są używane we wszystkich widokach po zalogowaniu. |
| 3.2.4 Consistent Identification | AA | `P` | Powtarzane działania mają te same nazwy i style, w tym powrót, logowanie, obliczenie, podpowiedź i sterowanie oknem. |
| 3.2.6 Consistent Help | A | `N/D` | Bieżąca aplikacja nie osadza powtarzanego mechanizmu kontaktu lub pomocy w wielu widokach. Dokumentacja i wsparcie pozostają dostępne z repozytorium i ekranu O programie. |
| 3.3.1 Error Identification | A | `P` | Walidacja kont, kalkulatorów i zadań identyfikuje błąd tekstowo. Dynamiczne regiony mają nazwę automatyzacji i ustawienie live `Polite`. |
| 3.3.2 Labels or Instructions | A | `P` | Pola mają opisowe placeholdery lub nazwy automatyzacji, a reguły hasła i składnia kalkulatora są pokazane przed wprowadzeniem danych. |
| 3.3.3 Error Suggestion | AA | `P` | Komunikaty podają przyczynę i możliwą korektę, na przykład wymagany wybór odpowiedzi, zakres hasła, pozycję błędu wyrażenia lub zakaz dzielenia przez zero. |
| 3.3.4 Error Prevention (Legal, Financial, Data) | AA | `P` | Rejestracja i zmiana hasła mają walidację oraz powtórzenie hasła, a odpowiedź zamknięta jest sprawdzana przed zapisem postępu. Przycisk zadania otwartego jawnie zapowiada przed aktywacją oba skutki: pokazanie odpowiedzi i oznaczenie zadania jako ukończone. Nazwa automatyzacji przekazuje ten sam skutek. |
| 3.3.7 Redundant Entry | A | `P` | Dane nie są ponownie wymagane bez potrzeby. Powtórzenie hasła jest celowym zabezpieczeniem przed błędem i mieści się w wyjątku dotyczącym bezpieczeństwa. |
| 3.3.8 Accessible Authentication (Minimum) | AA | `M` | Logowanie nie używa testu poznawczego, CAPTCHA ani przepisywania kodu z obrazu. Standardowe pola pozwalają wkleić hasło i kod odzyskiwania. Należy potwierdzić współpracę ze schowkiem i menedżerem haseł na wspieranych platformach. |

### 4. Solidność

| Kryterium | Poziom | Status | Ocena i dowód |
| --- | --- | --- | --- |
| 4.1.2 Name, Role, Value | A | `M` | Automatyczne regresje sprawdzają nazwy pól, przycisków symbolicznych oraz dynamicznych wyników. Pełne role, wartości, stany wyboru, stan rozwinięcia i dostępność każdego widoku wymagają przeglądu przez Accessibility Insights, Inspect lub odpowiednik oraz czytnik ekranu. |
| 4.1.3 Status Messages | AA | `P` | Status konta, wynik kalkulatora, wynik funkcji kwadratowej, sprawdzenie zadania i podpowiedź używają nazw automatyzacji i `AutomationLiveSetting.Polite`, bez wymuszania zmiany fokusu. |

Kryterium 4.1.1 Parsing nie ma w tabeli, ponieważ [WCAG 2.2 usunęło je jako przestarzałe](https://www.w3.org/TR/WCAG22/#parsing). Poprawność AXAML i drzewa kontrolek pozostaje niezależnie sprawdzana przez build i testy uruchomieniowe.

## Manualna checklista przed deklaracją zgodności

Poniższe czynności nie blokują używania aplikacji ani technicznego zamknięcia zmian stylistycznych, ale blokują formalne twierdzenie o pełnej zgodności WCAG 2.2 AA:

1. przejść cały produkt wyłącznie klawiaturą, w obu kierunkach, we wszystkich czterech ustawieniach motywu;
2. powtórzyć przejście z NVDA na Windows oraz z natywnymi czytnikami na Ubuntu i macOS;
3. zapisać role, nazwy, wartości i kolejność ogłaszania głównych kontrolek;
4. sprawdzić okno `720x520`, systemowe skalowanie `200%` i zwiększone odstępy tekstu;
5. sprawdzić, czy fokus nie jest zasłonięty i czy przewijanie automatycznie pokazuje aktywną kontrolkę;
6. sprawdzić zachowanie tooltipów z klawiaturą, myszą i powiększeniem;
7. zmierzyć wszystkie cele wskaźnika mniejsze niż `24x24` albo udokumentować wyjątek odstępu;
8. sprawdzić schowek i menedżer haseł w polach logowania i odzyskiwania;
9. zweryfikować motyw systemowy oraz wykrywanie wysokiego kontrastu na każdym wspieranym systemie;
10. dołączyć wersję systemu, backend Avalonia, rozdzielczość, skalowanie i wynik do protokołu.

## Wniosek

Zmiana z dyskusji #49 nie jest wyłącznie kosmetyczna. Aktualna architektura obejmuje spójny krój pisma, dynamiczne motywy, mierzalny kontrast, widoczny fokus, dostępny własny chrome, responsywne widoki i skalowalne dialogi. Automatyczne regresje ograniczają ryzyko powrotu sztywnego, jasnego interfejsu lub niewidocznego fokusu.

Pełny przegląd kryteriów A/AA został wykonany, lecz repozytorium nie składa deklaracji zgodności WCAG 2.2 AA. Do takiej deklaracji brakuje udokumentowanych testów na natywnych drzewach automatyzacji, z czytnikami ekranu, przy powiększeniu i z alternatywnymi metodami wejścia. Wynik ten jest celowo bardziej zachowawczy niż samo przejście testów headless.
