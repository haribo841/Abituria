# Historia zmian

W tym pliku są opisywane wyłącznie rzeczywiste wydania repozytorium `haribo841/Abituria`. Nie rekonstruujemy fikcyjnych wersji `0.1.0` ani `0.2.0` na podstawie historycznych snapshotów.

## [0.9.0-beta.1] - oczekuje na publikację

Pierwsza wersja przygotowana do publicznego wydania. Planowana data zostanie wpisana bezpośrednio przed utworzeniem tagu, po zaliczeniu poprzedzającej go części checklisty wydania.

### Dodano

- jeden przenośny kod AvaloniaUI dla Windows, Ubuntu i macOS;
- samowystarczalne paczki x64: ZIP dla Windows, `tar.gz` dla Ubuntu i ZIP z `Abituria.app` dla macOS Intel;
- lokalne profile gościa i konta chronione hasłem, odzyskiwanie konta oraz trwały postęp w SQLite;
- 18 tablic matematycznych, 7 dostępnych działów i 35 zadań matury poprawkowej 2021;
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

### Bezpieczeństwo łańcucha dostaw

- przypięto `SQLitePCLRaw.bundle_e_sqlite3` `2.1.12`;
- włączono audyt wszystkich zależności NuGet od poziomu `low`, a `NU1901`-`NU1904` są błędami;
- dodano lockfile projektu i testów oraz restore w trybie `--locked-mode`;
- paczki są kontrolowane pod kątem PDB, sekretów i starych snapshotów;
- tag musi wskazywać dokładny commit `origin/main`, a finalny smoke test potwierdza wersję i commit rzeczywistego pliku wykonywalnego;
- walidator odrzuca dodatkowe korzenie archiwum, niebezpieczne ścieżki i niespójne dowody licencyjne;
- zgodność danych jest sprawdzana na niezmiennej bazie utworzonej przez rzeczywisty kod .NET 9 i EF Core 9;
- każdy paczkowany zasób ma pozytywny wpis pochodzenia i podstawę dystrybucji, a twarda brama `releaseEligible` jest gotowa do kontroli przed tagiem; samo publiczne wydanie pozostaje nieopublikowane.

[0.9.0-beta.1]: https://github.com/haribo841/Abituria/releases/tag/v0.9.0-beta.1
