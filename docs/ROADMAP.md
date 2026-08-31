# Plan rozwoju Abiturii

Plan powstał z porównania wszystkich zachowanych wersji projektu. Ten dokument jest technicznym odpowiednikiem strony „Plan rozwoju” w aplikacji. Uzasadnienie produktu, interesariusze, ryzyka i etapy opisuje [analiza biznesowa](BUSINESS_ANALYSIS.md); roadmapa pozostaje źródłem technicznego statusu funkcji.

## Przeniesione

- shell AvaloniaUI 12 i .NET 10 LTS,
- konta SQLite, profile gościa i postęp,
- 18 tablic matematycznych,
- pełny kurs matematyki Formuły 2023: 4 grupy, 13 obszarów, 119 wymagań, 238 rozwiązanych przykładów i 357 ćwiczeń z filtrem poziomu oraz osobnym postępem,
- dział Wektory z ośmioma ilustracjami,
- materiały issue #35: liczby naturalne i indukcja, alfabet grecki, liczby rzeczywiste i zbiory, algebra, równania i nierówności, funkcja kwadratowa oraz logarytmy,
- matury główne 2022 PP i PR oraz poprawkowa 2022 PP w Formule 2015 zweryfikowane z publicznych archiwów: odpowiednio 35 zadań, 35 jednostek postępu i 45 punktów, 15 zadań, 15 jednostek postępu i 50 punktów oraz 35 zadań, 35 jednostek postępu i 45 punktów,
- matura główna 2023 PP zweryfikowana z arkuszem i zasadami oceniania CKE: 31 zadań, 34 jednostki postępu i 46 punktów,
- matura poprawkowa 2023 PP zweryfikowana z arkuszem CKE i dostępnym publicznym archiwum zasad oceniania: 33 zadania, 36 jednostek postępu i 46 punktów,
- matura główna 2023 PR zweryfikowana z arkuszem i zasadami oceniania CKE: 13 zadań, 14 jednostek postępu i 50 punktów,
- matura główna 2025 PP zweryfikowana z arkuszem i zasadami oceniania CKE: 31 zadań, 35 jednostek postępu i 50 punktów,
- matura główna 2025 PR zweryfikowana z arkuszem i zasadami oceniania CKE: 12 zadań, 13 jednostek postępu i 50 punktów,
- matura poprawkowa 2025 PP zweryfikowana z publicznym archiwum: 31 zadań, 36 jednostek postępu i 50 punktów,
- matura główna 2024 PP zweryfikowana z arkuszem i zasadami oceniania CKE: 31 zadań, 35 jednostek postępu i 46 punktów,
- matura główna 2024 PR zweryfikowana z arkuszem i zasadami oceniania CKE: 13 zadań, 14 jednostek postępu i 50 punktów,
- matura główna 2026 PP zweryfikowana z arkuszem i zasadami oceniania CKE: 33 zadania, 37 jednostek postępu i 50 punktów,
- matura główna 2026 PR zweryfikowana z arkuszem i zasadami oceniania CKE: 12 zadań, 13 jednostek postępu i 50 punktów,
- 35 zadań matury poprawkowej 2021 zachowanych z identyfikatorami `mp21-*`,
- matura poprawkowa 2024 PP zweryfikowana z publicznym archiwum: 30 zadań, 36 jednostek postępu i 46 punktów,
- wybór siedemnastu arkuszy i agregacja ich 473 jednostek postępu według 17 tematów,
- kalkulator ogólny z parserem wyrażeń, notacją naukową, Ans i historią sesji,
- kalkulator funkcji kwadratowej,
- losowanie zadań z całego arkusza i w obrębie wybranego tematu,
- automatyczne, samowystarczalne paczki portable x64 dla Windows, Ubuntu i macOS,
- dokumentacja wydania, GitHub Pages, sumy SHA-256, SBOM i atestacje pochodzenia.

## Zaplanowane

- generator wykresów i kalkulator funkcji trygonometrycznych,
- arkusze 2019, 2020 i matura podstawowa 2021,
- materiały wideo i wyszukiwanie zapisu matematycznego.

## Poza zakresem wersji beta

- natywne instalatory MSI, MSIX, DEB, RPM, DMG lub PKG,
- automatyczne aktualizacje,
- podpisywanie kodu i paczek,
- paczka macOS dla Apple Silicon.

Manifest `Content/provenance.json` ma obecnie `releaseEligible=false`. Rozszerzenia deklaracji właściciela z 3, 5, 10 i 12 sierpnia 2026 r. obejmują zatwierdzone arkusze i materiały CKE, ale nie matury główne 2022 PP i PR oraz poprawkową 2022 PP, maturę główną i poprawkową 2023 PP ani matury poprawkowe 2024 i 2025 PP, wraz z trzydziestoma sześcioma nowymi diagramami. Każdy przyszły publiczny prerelease wymaga pełnej checklisty wydania, zweryfikowanego workflow i uzupełnionej deklaracji praw.

## Zastąpione

- WPF, `NavigationWindow` i własne obramowanie okna przez AvaloniaUI,
- prototyp SQL Server LocalDB przez lokalne SQLite,
- 35 pustych ekranów E1-E35 przez generyczny model zadań,
- arbitralne limity 10 profili i 15 znaków przez aktualny model kont,
- niepodłączony prototyp edytora WPF-Math przez renderowanie CSharpMath; eksport SVG/PNG nie był częścią osiągalnego interfejsu.

Szczegółowe identyfikatory, konteksty i źródła pozycji są wersjonowane w `Content/roadmap.json`.
