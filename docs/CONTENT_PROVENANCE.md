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

`releaseEligible` ma wartość `false`. Publiczne wydanie `0.9.0-beta.1` jest zablokowane przez trzy grupy:

### Materiały CKE

Grupa `cke-2021-correction-exam` obejmuje `Content/exam-2021-correction.json` i `img/mp21z*.png`. Zidentyfikowano arkusz oraz zasady oceniania CKE `EMAP-P0-100-2108`, ale nie ma udokumentowanej zgody lub licencji pozwalającej redystrybuować treść i ilustracje w paczce aplikacji.

Do odblokowania potrzebny jest jednoznaczny dokument CKE obejmujący tę formę redystrybucji albo pisemna zgoda. Sam publiczny dostęp do arkusza nie jest wystarczającym dowodem.

### Odziedziczone grafiki matematyczne

Grupa `inherited-mathematics-images` obejmuje `img/?.png` i `img/w*.png`. Pliki pochodzą z historycznych snapshotów, ale autorstwo i źródło każdego obrazu nie zostały potwierdzone. Zachowana licencja repozytorium historycznego nie dowodzi jeszcze, że wszystkie dołączone grafiki zostały przez jego autorów stworzone lub legalnie sublicencjonowane.

Każdy plik należy przypisać do źródła i licencji albo zastąpić nowym, własnym zasobem z udokumentowanym procesem utworzenia.

### Grafiki i ikony aplikacji

Grupa `inherited-application-images` obejmuje grafiki nawigacji, `img/icon.png` oraz `img/icon.ico`, z którego wyprowadzane są metadane graficzne paczek Windows i macOS. Autorzy i dokładne źródła nie są potwierdzeni.

Najbezpieczniejszą ścieżką jest zastąpienie ich nowym zestawem zaprojektowanym dla aktualnej implementacji i zapisanie autora, plików źródłowych oraz licencji w repozytorium.

## Grupy zatwierdzone

Manifest oznacza obecnie jako `approved`:

- aktualne treści techniczne i inwentarz autorstwa Adama Kubisia na licencji MIT;
- zmigrowane treści matematyczne, dla których dowodem jest zachowana historyczna licencja MIT i inwentarz migracji;
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
