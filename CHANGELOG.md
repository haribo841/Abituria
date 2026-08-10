# Historia zmian

W tym pliku są opisywane wyłącznie rzeczywiste wydania repozytorium `haribo841/Abituria`. Nie rekonstruujemy fikcyjnych wersji `0.1.0` ani `0.2.0` na podstawie historycznych snapshotów.

## [0.9.1] - 2026-08-10

Wydanie beta uzupełniające dostępność ćwiczeń, wydawanie pojedynczego pliku Windows i dokumentację produktu.

### Dodano

- podpowiedzi dla wszystkich trybów zadań, z bezpiecznym komunikatem zastępczym, gdy źródłowe dane zadania nie definiują własnych kroków;
- pełne przechowywanie historii podpowiedzi w widoku ćwiczenia oraz testy zachowania dla zadań matur i kursu;
- transkrypcję matury głównej CKE 2024 na poziomie podstawowym i rozszerzonym wraz z wektorowymi diagramami Avalonia;
- pełną transkrypcję historycznej analizy biznesowej jako materiał archiwalny oraz aktywną analizę śledzącą Issue #9;
- osobiste rozszerzenie deklaracji redystrybucji dla przykładów z informatorów CKE i matur 2024, opisane w `ASSET_RIGHTS_DECLARATION.md`.

### Zmieniono

- hasło lokalnego konta musi być niepuste, ale nie ma minimalnej długości;
- Windows otrzymuje jeden uruchamialny plik EXE, a ZIP pozostaje wyłącznie pakietem audytowym;
- widoki ćwiczeń i źródeł przewijają się do końca, a tryb pełnego ekranu ukrywa systemowy pasek zadań;
- dokumentację proweniencji, instalacji, wymagań i procesu wydania dla publicznego prerelease `v0.9.1`;
- manifest proweniencji ma `releaseEligible=true` po wskazanej deklaracji właściciela.

[0.9.1]: https://github.com/haribo841/Abituria/releases/tag/v0.9.1

## [0.9.0-beta.1] - 2026-08-05

Pierwsze publiczne wydanie beta Abiturii po migracji do AvaloniaUI.

### Dodano

- jeden przenośny kod AvaloniaUI dla Windows, Ubuntu i macOS;
- samowystarczalne paczki x64: ZIP dla Windows, `tar.gz` dla Ubuntu i ZIP z `Abituria.app` dla macOS Intel;
- lokalne profile gościa i konta chronione hasłem, odzyskiwanie konta oraz trwały postęp w SQLite;
- 18 tablic matematycznych, pełny kurs Formuły 2023 w 13 obszarach i 35 zadań matury poprawkowej 2021;
- maturę główną 2025 na poziomie podstawowym: 31 oficjalnych zadań, 35 jednostek postępu, 50 punktów, rozwiązania, kryteria oceniania i dziewięć dostępnych diagramów wektorowych;
- maturę główną 2025 na poziomie rozszerzonym: 12 oficjalnych zadań, 13 jednostek postępu, 50 punktów, rozwiązania i kryteria oceniania;
- maturę główną 2026 na poziomie podstawowym: 33 oficjalne zadania, 37 jednostek postępu, 50 punktów, rozwiązania, kryteria oceniania i siedem dostępnych diagramów wektorowych;
- maturę główną 2026 na poziomie rozszerzonym: 12 oficjalnych zadań, 13 jednostek postępu, 50 punktów, rozwiązania, kryteria oceniania i trzy dostępne diagramy wektorowe;
- kalkulator ogólny z parserem złożonych wyrażeń, historią, `Ans`, potęgami, pierwiastkami, notacją naukową i powtarzanym `=`;
- kalkulator funkcji kwadratowej;
- losowanie zadań z całego arkusza albo z wybranego tematu, z zachowaniem kontekstu nawigacji;
- bezinterfejsowe polecenie `--release-smoke-test --data-directory` korzystające z izolowanej bazy;
- ekran „O programie” pokazujący wersję, commit, licencję, autora i repozytorium;
- generowane sumy SHA-256, osobne SBOM SPDX i attestation pochodzenia artefaktów;
- pakietowy manifest dowodów licencyjnych NuGet z zachowanymi nuspec i dostępnymi plikami notices;
- dokumentację użytkownika i techniczną budowaną przez DocFX oraz publikowaną przez GitHub Pages;
- maszynowy inwentarz pochodzenia paczkowanych treści, fontów i obrazów.
- formalne testy wydajności, pamięci i obciążenia oraz protokoły użyteczności, instalacji niezależnej i odbioru końcowego.
- osobne protokoły odbioru przyrostów I-IV, rejestr dwóch rund użyteczności i protokół legalnej formy przekazania dla Issue #43;
- generator nieuruchamialnego pakietu dokumentacyjnego dla komisji z własnym manifestem i SHA-256.
- oświadczenie właściciela projektu potwierdzające prawo do publicznej redystrybucji ewidencjonowanych zasobów jako części Abiturii.
- protokół publicznej obrony z 17 stycznia 2022 r. z pełnym składem komisji, wynikiem bardzo dobrym i odnośnikiem do zachowanego nagrania dla Issue #44.
- protokół kryteriów akceptacji i oceny dla Issue #45 z pełną macierzą, ograniczeniami dowodowymi oraz gotowym komentarzem zamykającym.

### Zmieniono

- docelową platformę z .NET 9 na .NET 10 LTS;
- interfejs z historycznego WPF na AvaloniaUI 12;
- wersje Entity Framework Core i Microsoft Dependency Injection na `10.0.10`;
- wersjonowanie tak, aby assembly, paczki, tag, changelog, ekran „O programie” i strona używały `0.9.0-beta.1`;
- przechowywanie bazy poza katalogiem programu, aby aktualizacja portable nie usuwała danych użytkownika.
- nazwy automatyzacji symbolicznych przycisków i pól oraz dynamiczne regiony wyników dla technologii asystujących.
- obsługę pięciu aktywnych arkuszy i 133 jednostek postępu, odpowiedzi złożonych, agregację zadań w 17 tematach oraz osobny licznik postępu dla każdego arkusza.

### Bezpieczeństwo łańcucha dostaw

- przypięto `SQLitePCLRaw.bundle_e_sqlite3` `2.1.12`;
- włączono audyt wszystkich zależności NuGet od poziomu `low`, a `NU1901`-`NU1904` są błędami;
- dodano lockfile projektu i testów oraz restore w trybie `--locked-mode`;
- paczki są kontrolowane pod kątem PDB, sekretów i starych snapshotów;
- tag musi wskazywać dokładny commit `origin/main`, a finalny smoke test potwierdza wersję i commit rzeczywistego pliku wykonywalnego;
- walidator odrzuca dodatkowe korzenie archiwum, niebezpieczne ścieżki i niespójne dowody licencyjne;
- zgodność danych jest sprawdzana na niezmiennej bazie utworzonej przez rzeczywisty kod .NET 9 i EF Core 9;
- każdy paczkowany zasób ma pozytywny wpis pochodzenia i podstawę dystrybucji, a twarda brama `releaseEligible` jest obowiązkowo kontrolowana przed tagiem.

[0.9.0-beta.1]: https://github.com/haribo841/Abituria/releases/tag/v0.9.0-beta.1
