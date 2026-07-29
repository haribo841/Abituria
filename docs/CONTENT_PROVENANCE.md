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

`releaseEligible` ma wartość `true`. Wszystkie zasoby paczkowane przez projekt mają status `approved`. Podstawę dla trzech grup, które wcześniej były zablokowane, stanowi [deklaracja praw do redystrybucji](ASSET_RIGHTS_DECLARATION.md) z 19 lipca 2026 r. Deklarację rozszerzono 27 lipca 2026 r. o transkrypcję tablic CKE dla Formuły 2023. Kurs Formuły 2023 rozdziela urzędowe brzmienie wymagań, autorskie materiały dydaktyczne i zachowane materiały historyczne.

### Materiały CKE

Grupa `cke-2021-correction-exam` obejmuje `Content/exam-2021-correction.json` i `img/mp21z*.png`. Autorem źródłowego arkusza i zasad oceniania `EMAP-P0-100-2108` pozostaje Centralna Komisja Egzaminacyjna, a adaptację i weryfikację wykonał Adam Kubiś. Status `approved` wynika z deklaracji posiadania praw lub skutecznego upoważnienia do publicznej redystrybucji tych zasobów jako części Abiturii. Deklaracja nie przenosi autorstwa i nie udziela samodzielnej licencji na te materiały poza Abiturią.

Grupa `cke-formula-2023-transcription` obejmuje `Content/formulas.json`. Dokument źródłowy CKE jest wskazany dokładnym adresem i sumą SHA-256 w katalogu treści oraz w [macierzy pokrycia tablic](FORMULA_2023_COVERAGE.md). Transkrypcję i weryfikację wykonał Adam Kubiś, natomiast autorem dokumentu źródłowego pozostaje Centralna Komisja Egzaminacyjna. Status `approved` wynika z rozszerzenia deklaracji z 27 lipca 2026 r.

### Kurs matematyki Formuły 2023

Grupa `mathematics-course-formula-2023` obejmuje `Content/chapters.json`, `Content/course-exercises.json` i `img/course/*.png`. Dokładne wymagania pochodzą z podstawy programowej ogłoszonej w Dz.U. 2024 poz. 1019. Katalog przypina adres aktu, dwa informatory CKE, ich sumy SHA-256 i datę weryfikacji. Informatory służą do określenia kontekstu egzaminu; ich zadania, rozwiązania ani ilustracje nie zostały skopiowane.

Przykłady, ćwiczenia, podpowiedzi, pełne rozwiązania i cztery diagramy kursu są materiałami autorskimi Adama Kubisia. Zachowane fragmenty issue #35 nadal mają historyczne przypisanie i identyfikatory. Maszynowa macierz [MATH_COURSE_2023_COVERAGE.md](MATH_COURSE_2023_COVERAGE.md) rozdziela te warstwy oraz dokumentuje kontrakt `119/238/357`.

### Odziedziczone grafiki matematyczne

Grupa `inherited-mathematics-images` obejmuje `img/?.png` i `img/w*.png`. Autorzy pozostają nieustaleni, a źródłem są historyczne snapshoty projektu. Status `approved` wynika z deklaracji posiadania praw lub skutecznego upoważnienia do publicznej redystrybucji tych grafik jako części Abiturii. Nie jest to przypisanie autorstwa ani samodzielna licencja na grafiki.

### Grafiki i ikony aplikacji

Grupa `inherited-application-images` obejmuje grafiki nawigacji, `img/icon.png` oraz `img/icon.ico`, z którego wyprowadzane są metadane graficzne paczek Windows i macOS. Autorzy pozostają nieustaleni, a źródłem są historyczne snapshoty projektu. Status `approved` wynika z deklaracji posiadania praw lub skutecznego upoważnienia do publicznej redystrybucji tych grafik jako części Abiturii. Nie jest to przypisanie autorstwa ani samodzielna licencja na grafiki.

## Grupy zatwierdzone

Manifest oznacza obecnie jako `approved`:

- aktualne treści techniczne i inwentarz autorstwa Adama Kubisia na licencji MIT;
- autorskie przykłady, ćwiczenia, rozwiązania i diagramy kursu Formuły 2023 wraz z przypisanym urzędowym źródłem dokładnego brzmienia wymagań;
- zmigrowane treści matematyczne, dla których dowodem jest zachowana historyczna licencja MIT i inwentarz migracji;
- arkusz CKE 2021, transkrypcję tablic CKE dla Formuły 2023 oraz odziedziczone grafiki objęte deklaracją praw lub upoważnienia do publicznej redystrybucji wyłącznie jako części Abiturii;
- font Mulish na licencji SIL Open Font License 1.1, potwierdzonej przez `fonts/OFL.txt` i `fonts/README.txt`.

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

Pakiety NuGet nie są wpisywane do manifestu zasobów. Ich dokładnie rozwiązane wersje i licencje generują [DEPENDENCIES.md](DEPENDENCIES.md) oraz [THIRD-PARTY-NOTICES.md](../THIRD-PARTY-NOTICES.md). Fonty, obrazy i treści pozostają w `Content/provenance.json`, ponieważ nie są rozwiązywane przez NuGet.

## Kryterium zgody na publikację

Publiczny prerelease można opublikować dopiero wtedy, gdy jednocześnie:

- `releaseEligible` wynosi `true` i wynika wyłącznie ze statusów grup;
- walidator z `-RequireReleaseEligible` przechodzi bez błędu;
- każdy zasób rzeczywiście znajdujący się w archiwach jest objęty manifestem;
- `LICENSE`, wymagane teksty licencji i `THIRD-PARTY-NOTICES.md` są dołączone do paczki;
- wynik jest potwierdzony w checkliście wydania i zachowany w logu workflow.

Ten dokument nie jest poradą prawną. W razie wątpliwości zasób pozostaje `blocked` do czasu uzyskania wiarygodnego potwierdzenia.
