# Pochodzenie treści i zasobów wydania

Licencja MIT kodu Abiturii nie oznacza automatycznie prawa do redystrybucji każdego tekstu edukacyjnego, fontu, obrazu lub ikony. Publiczna paczka może powstać wyłącznie z zasobów mających udokumentowanego autora, źródło oraz licencję albo inną podstawę dystrybucji.

Maszynowym źródłem prawdy jest [`Content/provenance.json`](https://github.com/haribo841/Abituria/blob/main/Content/provenance.json). Dokumentacja opisuje jego kontrakt, ale nie zastępuje danych manifestu.

## Kontrakt manifestu

Manifest ma `schemaVersion` równe `1`, pole `releaseEligible` i tablicę `assets`. Każda grupa w `assets` musi zawierać:

| Pole | Znaczenie |
| --- | --- |
| `id` | stabilny, unikalny identyfikator grupy |
| `paths[]` | co najmniej jeden dokładny wzorzec paczkowanych plików |
| `author` | autor albo jawna informacja, że autor pozostaje nieustalony |
| `source` | źródło i sposób pozyskania zasobu |
| `license` | licencja albo opisana podstawa dystrybucji |
| `distributionStatus` | wyłącznie `approved` albo `blocked` |
| `evidence[]` | istniejące w repozytorium pliki będące dowodem pochodzenia lub licencji |
| `blockedReason` | obowiązkowa przyczyna dla statusu `blocked` |

`releaseEligible` musi być `true` dokładnie wtedy, gdy żadna grupa nie ma statusu `blocked`.

## Statusy

- `approved` - istnieje wystarczająca, wersjonowana podstawa umieszczenia zasobu w publicznej paczce;
- `blocked` - podstawa jest niepełna, niejednoznaczna albo zabrania dystrybucji; zasób blokuje publiczne wydanie.

Status `approved` nie może opierać się wyłącznie na tym, że plik znajdował się w historycznym repozytorium. Potrzebny jest dowód obejmujący konkretny zasób. Brak informacji nie oznacza domeny publicznej ani zgody.

## Automatyczna walidacja

Skrypt [`tools/Test-ContentProvenance.ps1`](https://github.com/haribo841/Abituria/blob/main/tools/Test-ContentProvenance.ps1):

1. odczytuje wszystkie `AvaloniaResource` oraz ikonę aplikacji z projektu;
2. rozwija wzorce do rzeczywistych plików;
3. wymaga kompletnych pól i istniejących dowodów;
4. odrzuca zasób brakujący w manifeście;
5. odrzuca zasób zadeklarowany więcej niż raz;
6. odrzuca wpis manifestu, który nie jest faktycznie paczkowany;
7. porównuje `releaseEligible` ze statusami grup;
8. z parametrem `-RequireReleaseEligible` kończy się błędem przy każdej grupie `blocked`.

Walidacja kompletności podczas pracy nad repozytorium:

```powershell
pwsh -NoProfile -File tools/Test-ContentProvenance.ps1
```

Brama publicznego wydania:

```powershell
pwsh -NoProfile -File tools/Test-ContentProvenance.ps1 `
  -RequireReleaseEligible
```

Nie wolno usuwać parametru bramy, ręcznie zmieniać `releaseEligible` ani wyłączać paczkowania problematycznego zasobu bez przeglądu wpływu na funkcje, testy i dokumentację.

## Aktualny stan

`releaseEligible` ma wartość `true`. Rozszerzenia [deklaracji praw do redystrybucji](ASSET_RIGHTS_DECLARATION.md) z 3 i 5 sierpnia 2026 r. obejmują matury CKE 2025 i 2026, zasady oceniania oraz dziewiętnaście autorskich implementacji wektorowych Avalonia. Grupy arkuszy 2025 i 2026 oraz `runtime-vector-diagrams` mają status `approved`. Zwykła walidacja kompletności oraz osobna brama `-RequireReleaseEligible` muszą przechodzić niezależnie.

### Materiały CKE

Grupa `cke-2021-correction-exam` obejmuje wyłącznie `Content/exam-2021-correction.json`. Autorem źródłowego arkusza i zasad oceniania `EMAP-P0-100-2108` pozostaje Centralna Komisja Egzaminacyjna, a adaptację i weryfikację wykonał Adam Kubiś. Diagramy używane przez zadania są osobnymi definicjami w katalogu `runtime-vector-diagrams`.

Grupa `cke-2026-main-basic-exam` obejmuje `Content/exam-2026-main-basic.json`. Transkrypcja korzysta z arkusza `MMAP-P0-100-A-2605` i zasad oceniania `MMAP-P0-100-2605`; oba adresy, sumy SHA-256 i data weryfikacji znajdują się w [macierzy matury 2026](MATURA_2026_COVERAGE.md). Grupa ma status `approved` na podstawie rozszerzenia deklaracji z 3 sierpnia 2026 r.

Grupa `cke-2026-main-extended-exam` obejmuje `Content/exam-2026-main-extended.json`. Transkrypcja korzysta z arkusza `MMAP-R0-100-A-2605` i zasad oceniania `MMAP-R0-100-2605`; oba adresy, sumy SHA-256 i data weryfikacji znajdują się w [macierzy matury 2026 PR](MATURA_2026_EXTENDED_COVERAGE.md). Grupa ma status `approved` na podstawie rozszerzenia deklaracji z 3 sierpnia 2026 r.

Grupa `cke-2025-main-basic-exam` obejmuje `Content/exam-2025-main-basic.json`. Transkrypcja korzysta z arkusza `MMAP-P0-100-A-2505` i zasad oceniania `MMAP-P0-100-2505`; oba adresy, sumy SHA-256 i data weryfikacji znajdują się w [macierzy matury 2025 PP](MATURA_2025_COVERAGE.md). Grupa ma status `approved` na podstawie rozszerzenia deklaracji z 5 sierpnia 2026 r.

Grupa `cke-2025-main-extended-exam` obejmuje `Content/exam-2025-main-extended.json`. Transkrypcja korzysta z arkusza `MMAP-R0-100-A-2505` i zasad oceniania `MMAP-R0-100-2505`; oba adresy, sumy SHA-256 i data weryfikacji znajdują się w [macierzy matury 2025 PR](MATURA_2025_EXTENDED_COVERAGE.md). Grupa ma status `approved` na podstawie rozszerzenia deklaracji z 5 sierpnia 2026 r.

Grupa `cke-formula-2023-transcription` obejmuje `Content/formulas.json`. Dokument źródłowy CKE jest wskazany dokładnym adresem i sumą SHA-256 w katalogu treści oraz w [macierzy pokrycia tablic](FORMULA_2023_COVERAGE.md). Transkrypcję i weryfikację wykonał Adam Kubiś, natomiast autorem dokumentu źródłowego pozostaje Centralna Komisja Egzaminacyjna. Status `approved` wynika z rozszerzenia deklaracji z 27 lipca 2026 r.

### Kurs matematyki Formuły 2023

Grupa `mathematics-course-formula-2023` obejmuje `Content/chapters.json` i `Content/course-exercises.json`. Dokładne wymagania pochodzą z podstawy programowej ogłoszonej w Dz.U. 2024 poz. 1019. Katalog przypina adres aktu, dwa informatory CKE, ich sumy SHA-256 i datę weryfikacji. Informatory służą do określenia kontekstu egzaminu; ich zadania, rozwiązania ani ilustracje nie zostały skopiowane.

Przykłady, ćwiczenia, podpowiedzi, pełne rozwiązania i cztery nowe definicje diagramów kursu są materiałami autorskimi Adama Kubisia. Osiem zachowanych diagramów wektorowych nadal ma historyczne przypisanie. Maszynowa macierz [MATH_COURSE_2023_COVERAGE.md](MATH_COURSE_2023_COVERAGE.md) rozdziela te warstwy oraz dokumentuje kontrakt `119/238/357`.

### Diagramy i archiwum historyczne

Grupa `runtime-vector-diagrams` obejmuje `Content/diagrams.json` z dokładnie 76 definicjami wektorowymi: 36 dla tablic wzorów, 9 dla arkusza poprawkowego 2021, 9 dla matury podstawowej 2025, 7 dla matury podstawowej 2026, 3 dla matury rozszerzonej 2026 i 12 dla kursu. Każda definicja ma stabilny identyfikator, źródło i opis alternatywny, a dziewiętnaście nowych figur maturalnych ma również stronę arkusza. `DiagramView` materializuje je wyłącznie jako `Line`, `Polyline`, `Polygon`, `Ellipse`, łuki i `TextBlock` Avalonia, bez ładowania rastrów, `Image` lub `Bitmap`. Implementacje dla matur 2025 i 2026 są autorskim kodem Adama Kubisia na licencji MIT, a grupa ma status `approved` na podstawie rozszerzeń deklaracji z 3 i 5 sierpnia 2026 r.

Siedemdziesiąt pięć historycznych obrazów znajduje się w `docs/legacy/originals/images/`. Pliki zachowano bajt w bajt, udokumentowano mapowaniem `PATH-MAPPING.csv` i sumami `SHA256SUMS`, ale nie są paczkowane ani publikowane przez DocFX. Jedynym statycznym wyjątkiem jest `img/icon.ico` w grupie `application-icon`, używany wyłącznie jako `ApplicationIcon`.

## Grupy i statusy

Manifest oznacza jako `approved`:

- aktualne treści techniczne i inwentarz autorstwa Adama Kubisia na licencji MIT;
- autorskie przykłady, ćwiczenia, rozwiązania i diagramy kursu Formuły 2023 wraz z przypisanym urzędowym źródłem dokładnego brzmienia wymagań;
- zmigrowane treści matematyczne, dla których dowodem jest zachowana historyczna licencja MIT i inwentarz migracji;
- arkusze CKE 2021, 2025 i 2026, transkrypcję tablic CKE dla Formuły 2023, katalog wektorów Avalonia oraz ikonę aplikacji objęte deklaracją praw lub upoważnienia do publicznej redystrybucji wyłącznie jako części Abiturii;
- font Mulish na licencji SIL Open Font License 1.1, potwierdzonej przez `fonts/OFL.txt` i `fonts/README.txt`.

Manifest nie zawiera obecnie grup `blocked`. Każda zmiana źródeł, zakresu redystrybucji lub sposobu renderowania wymaga ponownej oceny, uruchomienia zwykłego walidatora i osobnej bramy `-RequireReleaseEligible`.

Status zatwierdzony należy ponownie ocenić po każdej zmianie źródła, zakresu plików albo sposobu pakowania.

## Dodawanie lub zmiana zasobu

1. Dodaj plik oraz źródłowy dowód autorstwa lub licencji.
2. Zaktualizuj `Content/provenance.json`.
3. Dopilnuj, aby wzorzec obejmował dokładnie zasoby paczkowane i nie nakładał się na inną grupę.
4. Uruchom walidator bez parametru wydawniczego.
5. Sprawdź testy inwentarza i rendering treści.
6. Jeżeli podstawa dystrybucji została niezależnie potwierdzona, ustaw `approved`; w przeciwnym razie pozostaw `blocked` i opisz przyczynę.
7. Przed wydaniem uruchom walidator z `-RequireReleaseEligible`.

Usunięcie problematycznego pliku jest dopuszczalne tylko wtedy, gdy projekt przestaje go paczkować i zależne treści, UI oraz testy zostają poprawione razem. Nie wolno pozostawić niedziałających odwołań.

## Zależności programistyczne

Pakiety NuGet nie są wpisywane do manifestu zasobów. Ich dokładnie rozwiązane wersje i licencje generują [DEPENDENCIES.md](DEPENDENCIES.md) oraz [THIRD-PARTY-NOTICES.md](../THIRD-PARTY-NOTICES.md). Fonty, ikona, aktywne diagramy i treści pozostają w `Content/provenance.json`, ponieważ nie są rozwiązywane przez NuGet. Niepublikowane oryginały historyczne są opisane we własnym archiwum i nie należą do zbioru paczkowanych zasobów.

## Kryterium zgody na publikację

Publiczny prerelease można opublikować dopiero wtedy, gdy jednocześnie:

- `releaseEligible` wynosi `true` i wynika wyłącznie ze statusów grup;
- walidator z `-RequireReleaseEligible` przechodzi bez błędu;
- każdy zasób rzeczywiście znajdujący się w archiwach jest objęty manifestem;
- `LICENSE`, wymagane teksty licencji i `THIRD-PARTY-NOTICES.md` są dołączone do paczki;
- wynik jest potwierdzony w checkliście wydania i zachowany w logu workflow.

Ten dokument nie jest poradą prawną. W razie wątpliwości zasób pozostaje `blocked` do czasu uzyskania wiarygodnego potwierdzenia.
