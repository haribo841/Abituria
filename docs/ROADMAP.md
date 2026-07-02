# Plan rozwoju Abiturii

Plan powstał z porównania wszystkich zachowanych wersji projektu. Ten dokument jest technicznym odpowiednikiem strony „Plan rozwoju” w aplikacji.

## Przeniesione

- shell AvaloniaUI i .NET 9,
- konta SQLite, profile gościa i postęp,
- 18 tablic matematycznych,
- dział Wektory z ośmioma ilustracjami,
- 35 zadań matury poprawkowej 2021 zweryfikowanych z materiałami CKE,
- kalkulator ogólny z parserem wyrażeń, Ans i historią sesji,
- kalkulator funkcji kwadratowej.

## Zaplanowane

- generator wykresów i kalkulator funkcji trygonometrycznych,
- rozbudowa sześciu zachowanych działów oraz materiały o liczbach naturalnych i alfabecie greckim,
- arkusze 2019, 2020 i matura podstawowa 2021,
- losowanie zadań, materiały wideo i wyszukiwanie zapisu matematycznego,
- dystrybucja aplikacji i dokumentacja wydania.

## Zastąpione

- WPF, `NavigationWindow` i własne obramowanie okna przez AvaloniaUI,
- prototyp SQL Server LocalDB przez lokalne SQLite,
- 35 pustych ekranów E1-E35 przez generyczny model zadań,
- arbitralne limity 10 profili i 15 znaków przez aktualny model kont,
- niepodłączony prototyp edytora WPF-Math przez renderowanie CSharpMath; eksport SVG/PNG nie był częścią osiągalnego interfejsu.

Szczegółowe identyfikatory, konteksty i źródła pozycji są wersjonowane w `Content/roadmap.json`.
