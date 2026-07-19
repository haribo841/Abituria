# Plan rozwoju Abiturii

Plan powstał z porównania wszystkich zachowanych wersji projektu. Ten dokument jest technicznym odpowiednikiem strony „Plan rozwoju” w aplikacji. Uzasadnienie produktu, interesariusze, ryzyka i etapy opisuje [analiza biznesowa](BUSINESS_ANALYSIS.md); roadmapa pozostaje źródłem technicznego statusu funkcji.

## Przeniesione

- shell AvaloniaUI 12 i .NET 10 LTS,
- konta SQLite, profile gościa i postęp,
- 18 tablic matematycznych,
- dział Wektory z ośmioma ilustracjami,
- materiały issue #35: liczby naturalne i indukcja, alfabet grecki, liczby rzeczywiste i zbiory, algebra, równania i nierówności, funkcja kwadratowa oraz logarytmy,
- 35 zadań matury poprawkowej 2021 zweryfikowanych z materiałami CKE,
- kalkulator ogólny z parserem wyrażeń, notacją naukową, Ans i historią sesji,
- kalkulator funkcji kwadratowej,
- losowanie zadań z całego arkusza i w obrębie wybranego tematu,
- automatyczne, samowystarczalne paczki portable x64 dla Windows, Ubuntu i macOS,
- dokumentacja wydania, GitHub Pages, sumy SHA-256, SBOM i atestacje pochodzenia.

## Zaplanowane

- generator wykresów i kalkulator funkcji trygonometrycznych,
- uzupełnienie działów Ciągi liczbowe i Liczby pierwsze,
- arkusze 2019, 2020 i matura podstawowa 2021,
- materiały wideo i wyszukiwanie zapisu matematycznego.

## Poza zakresem wersji beta

- natywne instalatory MSI, MSIX, DEB, RPM, DMG lub PKG,
- automatyczne aktualizacje,
- podpisywanie kodu i paczek,
- paczka macOS dla Apple Silicon.

Manifest `Content/provenance.json` nie zawiera już zasobów o statusie `blocked`, ale publiczne paczki portable pozostają nieopublikowane do czasu wykonania kompletnej checklisty, utworzenia tagu i pomyślnego zakończenia workflow wydania.

## Zastąpione

- WPF, `NavigationWindow` i własne obramowanie okna przez AvaloniaUI,
- prototyp SQL Server LocalDB przez lokalne SQLite,
- 35 pustych ekranów E1-E35 przez generyczny model zadań,
- arbitralne limity 10 profili i 15 znaków przez aktualny model kont,
- niepodłączony prototyp edytora WPF-Math przez renderowanie CSharpMath; eksport SVG/PNG nie był częścią osiągalnego interfejsu.

Szczegółowe identyfikatory, konteksty i źródła pozycji są wersjonowane w `Content/roadmap.json`.
