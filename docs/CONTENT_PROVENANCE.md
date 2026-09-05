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

`releaseEligible` ma obecnie wartość `false`. Rozszerzenie [deklaracji praw do redystrybucji](ASSET_RIGHTS_DECLARATION.md) z 1 września 2026 r. obejmuje wcześniej zatwierdzone grupy arkuszy CKE i 195 ówczesnych definicji `runtime-vector-diagrams`, lecz nie obejmuje dwudziestu grup arkuszy Formuły 2015 z 2015-2017 oraz lat równoległych 2023-2026 ani 54 pochodnych diagramów. Zwykła walidacja kompletności przechodzi lokalnie, natomiast brama `-RequireReleaseEligible` celowo blokuje wydanie do czasu odrębnego rozszerzenia deklaracji.

### Materiały CKE

Grupa `cke-2018-main-basic-exam` obejmuje `Content/exam-2018-main-basic.json`. Transkrypcja korzysta z arkusza `MMA-P1_1P-182` i zasad oceniania o tym samym kodzie; oba adresy, sumy SHA-256 i data weryfikacji znajdują się w [macierzy matury 2018 PP](MATURA_2018_BASIC_COVERAGE.md). Grupa ma status `approved` na podstawie rozszerzenia deklaracji właściciela z 1 września 2026 r.

Grupa `cke-2018-main-extended-exam` obejmuje `Content/exam-2018-main-extended.json`. Transkrypcja korzysta z arkusza `MMA-R1_1P-182` i zasad oceniania o tym samym kodzie; oba adresy, sumy SHA-256 i data weryfikacji znajdują się w [macierzy matury 2018 PR](MATURA_2018_EXTENDED_COVERAGE.md). Grupa ma status `approved` na podstawie rozszerzenia deklaracji właściciela z 1 września 2026 r.

Grupa `cke-2018-correction-basic-exam` obejmuje `Content/exam-2018-correction-basic.json`. Transkrypcja korzysta z arkusza `MMA-P1_1P-184` i zasad oceniania dla terminu poprawkowego, utrwalonych w publicznym archiwum PDF; adresy, sumy SHA-256 i data weryfikacji znajdują się w [macierzy matury poprawkowej 2018 PP](MATURA_2018_CORRECTION_BASIC_COVERAGE.md). Grupa ma status `approved` na podstawie rozszerzenia deklaracji właściciela z 1 września 2026 r.

Grupa `cke-2019-main-basic-exam` obejmuje `Content/exam-2019-main-basic.json`. Transkrypcja korzysta z arkusza `MMA-P1_1P-192` i zasad oceniania o tym samym kodzie; oba adresy, sumy SHA-256 i data weryfikacji znajdują się w [macierzy matury 2019 PP](MATURA_2019_BASIC_COVERAGE.md). Grupa ma status `approved` na podstawie rozszerzenia deklaracji właściciela z 1 września 2026 r.

Grupa `cke-2019-main-extended-exam` obejmuje `Content/exam-2019-main-extended.json`. Transkrypcja korzysta z arkusza `MMA-R1_1P-192` i zasad oceniania o tym samym kodzie; oba adresy, sumy SHA-256 i data weryfikacji znajdują się w [macierzy matury 2019 PR](MATURA_2019_EXTENDED_COVERAGE.md). Grupa ma status `approved` na podstawie rozszerzenia deklaracji właściciela z 1 września 2026 r.

Grupa `cke-2019-correction-basic-exam` obejmuje `Content/exam-2019-correction-basic.json`. Transkrypcja korzysta z arkusza `MMA-P1_1P-194` i zasad oceniania dla terminu poprawkowego, utrwalonych w publicznym archiwum PDF; adresy, sumy SHA-256 i data weryfikacji znajdują się w [macierzy matury poprawkowej 2019 PP](MATURA_2019_CORRECTION_BASIC_COVERAGE.md). Grupa ma status `approved` na podstawie rozszerzenia deklaracji właściciela z 1 września 2026 r.

Grupa `cke-2020-main-basic-exam` obejmuje `Content/exam-2020-main-basic.json`. Transkrypcja korzysta z arkusza `MMA-P1_1P-202` i zasad `MMA-PP-202`; oba adresy, sumy SHA-256 i data weryfikacji znajdują się w [macierzy matury 2020 PP](MATURA_2020_BASIC_COVERAGE.md). Grupa ma status `approved` na podstawie rozszerzenia deklaracji właściciela z 1 września 2026 r.

Grupa `cke-2020-main-extended-exam` obejmuje `Content/exam-2020-main-extended.json`. Transkrypcja korzysta z arkusza `MMA-R1_1P-202` i zasad `MMA-PR-202`; oba adresy, sumy SHA-256 i data weryfikacji znajdują się w [macierzy matury 2020 PR](MATURA_2020_EXTENDED_COVERAGE.md). Grupa ma status `approved` na podstawie rozszerzenia deklaracji właściciela z 1 września 2026 r.

Grupa `cke-2020-correction-basic-exam` obejmuje `Content/exam-2020-correction-basic.json`. Transkrypcja korzysta z arkusza `MMA-P1_1P-204` i zasad oceniania dla terminu poprawkowego, utrwalonych w publicznym archiwum PDF; adresy, sumy SHA-256 i data weryfikacji znajdują się w [macierzy matury poprawkowej 2020 PP](MATURA_2020_CORRECTION_BASIC_COVERAGE.md). Grupa ma status `approved` na podstawie rozszerzenia deklaracji właściciela z 1 września 2026 r.

Grupa `cke-2021-correction-exam` obejmuje wyłącznie `Content/exam-2021-correction.json`. Autorem źródłowego arkusza i zasad oceniania `EMAP-P0-100-2108` pozostaje Centralna Komisja Egzaminacyjna, a adaptację i weryfikację wykonał Adam Kubiś. Diagramy używane przez zadania są osobnymi definicjami w katalogu `runtime-vector-diagrams`.

Grupa `cke-2026-main-basic-exam` obejmuje `Content/exam-2026-main-basic.json`. Transkrypcja korzysta z arkusza `MMAP-P0-100-A-2605` i zasad oceniania `MMAP-P0-100-2605`; oba adresy, sumy SHA-256 i data weryfikacji znajdują się w [macierzy matury 2026](MATURA_2026_COVERAGE.md). Grupa ma status `approved` na podstawie rozszerzenia deklaracji z 3 sierpnia 2026 r.

Grupa `cke-2026-main-extended-exam` obejmuje `Content/exam-2026-main-extended.json`. Transkrypcja korzysta z arkusza `MMAP-R0-100-A-2605` i zasad oceniania `MMAP-R0-100-2605`; oba adresy, sumy SHA-256 i data weryfikacji znajdują się w [macierzy matury 2026 PR](MATURA_2026_EXTENDED_COVERAGE.md). Grupa ma status `approved` na podstawie rozszerzenia deklaracji z 3 sierpnia 2026 r.

Grupa `cke-2025-main-basic-exam` obejmuje `Content/exam-2025-main-basic.json`. Transkrypcja korzysta z arkusza `MMAP-P0-100-A-2505` i zasad oceniania `MMAP-P0-100-2505`; oba adresy, sumy SHA-256 i data weryfikacji znajdują się w [macierzy matury 2025 PP](MATURA_2025_COVERAGE.md). Grupa ma status `approved` na podstawie rozszerzenia deklaracji z 5 sierpnia 2026 r.

Grupa `cke-2025-main-extended-exam` obejmuje `Content/exam-2025-main-extended.json`. Transkrypcja korzysta z arkusza `MMAP-R0-100-A-2505` i zasad oceniania `MMAP-R0-100-2505`; oba adresy, sumy SHA-256 i data weryfikacji znajdują się w [macierzy matury 2025 PR](MATURA_2025_EXTENDED_COVERAGE.md). Grupa ma status `approved` na podstawie rozszerzenia deklaracji z 5 sierpnia 2026 r.

Grupa `cke-2024-main-basic-exam` obejmuje `Content/exam-2024-main-basic.json`. Transkrypcja korzysta z arkusza `MMAP-P0-100-A-2405` i zasad oceniania `MMAP-P0-100-2405`; oba adresy, sumy SHA-256 i data weryfikacji znajdują się w [macierzy matury 2024 PP](MATURA_2024_BASIC_COVERAGE.md). Grupa ma status `approved` na podstawie rozszerzenia deklaracji właściciela z 10 sierpnia 2026 r., wyłącznie dla redystrybucji jako części Abiturii.

Grupa `cke-2024-main-extended-exam` obejmuje `Content/exam-2024-main-extended.json`. Transkrypcja korzysta z arkusza `MMAP-R0-100-A-2405` i zasad oceniania `MMAP-R0-100-2405`; oba adresy, sumy SHA-256 i data weryfikacji znajdują się w [macierzy matury 2024 PR](MATURA_2024_EXTENDED_COVERAGE.md). Grupa ma status `approved` na podstawie rozszerzenia deklaracji właściciela z 10 sierpnia 2026 r., wyłącznie dla redystrybucji jako części Abiturii.

Grupa `cke-2023-main-extended-exam` obejmuje `Content/exam-2023-main-extended.json`. Transkrypcja korzysta z arkusza `MMAP-R0-100-2305` i zasad oceniania `MMAP-R0-100-2305`; oba adresy, sumy SHA-256 i data weryfikacji znajdują się w [macierzy matury 2023 PR](MATURA_2023_EXTENDED_COVERAGE.md). Grupa ma status `approved` na podstawie rozszerzenia deklaracji właściciela z 12 sierpnia 2026 r., wyłącznie dla redystrybucji jako części Abiturii.

Grupa `cke-2022-main-basic-exam` obejmuje `Content/exam-2022-main-basic.json`. Adaptacja korzysta z arkusza `EMAP-P0-100-2205` i zasad oceniania o tym samym kodzie, udostępnionych w archiwum OKE Warszawa. Adresy, sumy SHA-256 i data weryfikacji znajdują się w [macierzy matury 2022 PP](MATURA_2022_BASIC_COVERAGE.md). Grupa ma status `approved` na podstawie rozszerzenia deklaracji właściciela z 1 września 2026 r.

Grupa `cke-2022-main-extended-exam` obejmuje `Content/exam-2022-main-extended.json`. Adaptacja korzysta z arkusza `EMAP-R0-100-2205` i zasad oceniania o tym samym kodzie, udostępnionych w archiwum OKE Warszawa. Adresy, sumy SHA-256 i data weryfikacji znajdują się w [macierzy matury 2022 PR](MATURA_2022_EXTENDED_COVERAGE.md). Grupa ma status `approved` na podstawie rozszerzenia deklaracji właściciela z 1 września 2026 r.

Grupa `cke-2022-correction-basic-exam` obejmuje `Content/exam-2022-correction-basic.json`. Transkrypcja korzysta z arkusza `EMAP-P0-100-2208` i zasad oceniania o tym samym kodzie. CKE jest wydawcą treści, a obecnie dostępne publiczne archiwum, jego sumy SHA-256 i data weryfikacji są jawnie wskazane w [macierzy matury poprawkowej 2022 PP](MATURA_2022_CORRECTION_BASIC_COVERAGE.md). Grupa ma status `approved` na podstawie rozszerzenia deklaracji właściciela z 1 września 2026 r.

Grupa `cke-2023-main-basic-exam` obejmuje `Content/exam-2023-main-basic.json`. Transkrypcja korzysta z arkusza `MMAP-P0-100-2305` i zasad oceniania `MMAP-P0-100-2305`; oba adresy, sumy SHA-256 i data weryfikacji znajdują się w [macierzy matury 2023 PP](MATURA_2023_BASIC_COVERAGE.md). Grupa ma status `approved` na podstawie rozszerzenia deklaracji właściciela z 1 września 2026 r.

Grupa `cke-2023-correction-basic-exam` obejmuje `Content/exam-2023-correction-basic.json`. Transkrypcja korzysta z arkusza `MMAP-P0-100-2308` i zasad oceniania o tym samym kodzie. CKE jest wydawcą treści, a obecnie dostępne archiwum publiczne, jego sumy SHA-256 i data weryfikacji są jawnie wskazane w [macierzy matury poprawkowej 2023 PP](MATURA_2023_CORRECTION_BASIC_COVERAGE.md). Grupa ma status `approved` na podstawie rozszerzenia deklaracji właściciela z 1 września 2026 r.

Grupa `cke-2024-correction-basic-exam` obejmuje `Content/exam-2024-correction-basic.json`. Transkrypcja korzysta z arkusza `MMAP-P0-100-2408` i zasad oceniania o tym samym kodzie. CKE jest wydawcą treści, a obecnie dostępne archiwum publiczne, jego sumy SHA-256 i data weryfikacji są jawnie wskazane w [macierzy matury poprawkowej 2024 PP](MATURA_2024_CORRECTION_BASIC_COVERAGE.md). Grupa ma status `approved` na podstawie rozszerzenia deklaracji właściciela z 1 września 2026 r.

Grupa `cke-2025-correction-basic-exam` obejmuje `Content/exam-2025-correction-basic.json`. Transkrypcja korzysta z arkusza `MMAP-P0-100-2508` i zasad oceniania o tym samym kodzie. CKE jest wydawcą treści, a obecnie dostępne archiwum publiczne, jego sumy SHA-256 i data weryfikacji są jawnie wskazane w [macierzy matury poprawkowej 2025 PP](MATURA_2025_CORRECTION_BASIC_COVERAGE.md). Grupa ma status `approved` na podstawie rozszerzenia deklaracji właściciela z 1 września 2026 r.

Grupa `cke-formula-2023-transcription` obejmuje `Content/formulas.json`. Dokument źródłowy CKE jest wskazany dokładnym adresem i sumą SHA-256 w katalogu treści oraz w [macierzy pokrycia tablic](FORMULA_2023_COVERAGE.md). Transkrypcję i weryfikację wykonał Adam Kubiś, natomiast autorem dokumentu źródłowego pozostaje Centralna Komisja Egzaminacyjna. Status `approved` wynika z rozszerzenia deklaracji z 27 lipca 2026 r.

### Kurs matematyki Formuły 2023

Grupa `mathematics-course-formula-2023` obejmuje `Content/chapters.json` i `Content/course-exercises.json`. Dokładne wymagania pochodzą z podstawy programowej ogłoszonej w Dz.U. 2024 poz. 1019. Katalog przypina adres aktu, dwa informatory CKE, ich sumy SHA-256 i datę weryfikacji.

Warstwa autorska pozostaje niezmieniona: 238 przykładów, 357 ćwiczeń, podpowiedzi, pełne rozwiązania i cztery nowe definicje diagramów kursu są materiałami Adama Kubisia. Osiem zachowanych diagramów wektorowych nadal ma historyczne przypisanie. Osobna grupa `cke-formula-2023-guide-examples` obejmuje `Content/official-course-examples.json`: 66 przykładów podstawowych i 31 rozszerzonych wraz z oficjalnym brzmieniem wymagań wskazanych przy zadaniach, zasadami oceniania, rozwiązaniami, stronami PDF i 53 opisami informacji wizualnej. Autorem tych materiałów pozostaje CKE. Maszynowa macierz [MATH_COURSE_2023_COVERAGE.md](MATH_COURSE_2023_COVERAGE.md) rozdziela obie warstwy oraz dokumentuje kontrakty `119/238/357` i `66/31/97`.

### Matury Formuły 2015 z 2015-2017 oraz lat równoległych 2023-2026

Grupy `cke-2016-*`, `cke-2017-*`, `cke-2015-*` oraz `cke-2023-f2015-*` do `cke-2026-f2015-*` obejmują dwadzieścia lokalnych transkrypcji arkuszy i zasad oceniania. Każdy arkusz ma przypięte adresy źródłowe, sumy SHA-256 i datę weryfikacji w danych JSON. Pełną macierz czternastu nowo dodanych arkuszy przedstawia [MATURA_FORMULA_2015_ARCHIVE_COVERAGE.md](MATURA_FORMULA_2015_ARCHIVE_COVERAGE.md). Wszystkie dwadzieścia grup ma status `blocked`, ponieważ deklaracja z 1 września 2026 r. nie wskazuje tych konkretnych arkuszy. Techniczne przejście testów nie stanowi zgody na publiczną redystrybucję.

### Diagramy i archiwum historyczne

Grupa `runtime-vector-diagrams` obejmuje `Content/diagrams.json` z dokładnie 249 definicjami wektorowymi. Składa się z 195 wcześniej zatwierdzonych definicji, 16 figur pochodnych z arkuszy 2017, 15 figur pochodnych z arkuszy 2016 oraz 23 nowych figur pochodnych z Formuły 2015 z 2015 i lat równoległych 2023-2026. Każda definicja ma stabilny identyfikator, źródło i opis alternatywny. `DiagramView` materializuje je wyłącznie jako `Line`, `Polyline`, `Polygon`, `Ellipse`, łuki i `TextBlock` Avalonia, bez ładowania rastrów, `Image` lub `Bitmap`. Implementacje figur są autorskim kodem wektorowym Adama Kubisia, ale wspólna grupa ma status `blocked`, ponieważ zawiera 54 figury pochodne z niezatwierdzonych transkrypcji.

Siedemdziesiąt pięć historycznych obrazów znajduje się w `docs/legacy/originals/images/`. Pliki zachowano bajt w bajt, udokumentowano mapowaniem `PATH-MAPPING.csv` i sumami `SHA256SUMS`, ale nie są paczkowane ani publikowane przez DocFX. Jedynym statycznym wyjątkiem jest `img/icon.ico` w grupie `application-icon`, używany wyłącznie jako `ApplicationIcon`.

## Grupy i statusy

Manifest oznacza jako `approved`:

- aktualne treści techniczne i inwentarz autorstwa Adama Kubisia na licencji MIT;
- autorskie przykłady, ćwiczenia, rozwiązania i diagramy kursu Formuły 2023 wraz z przypisanym urzędowym źródłem dokładnego brzmienia wymagań;
- zmigrowane treści matematyczne, dla których dowodem jest zachowana historyczna licencja MIT i inwentarz migracji;
- zatwierdzone transkrypcje arkuszy CKE 2018-2026, objęte deklaracją praw lub upoważnienia do publicznej redystrybucji wyłącznie jako części Abiturii;
- font Mulish na licencji SIL Open Font License 1.1, potwierdzonej przez `fonts/OFL.txt` i `fonts/README.txt`.

Manifest zawiera dwadzieścia jeden grup `blocked`: dwadzieścia grup arkuszy Formuły 2015 z 2015-2017 oraz lat równoległych 2023-2026, a także wspólny katalog diagramów. Każda zmiana źródeł, zakresu redystrybucji lub sposobu renderowania wymaga ponownej oceny, uruchomienia zwykłego walidatora i osobnej bramy `-RequireReleaseEligible`.

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
