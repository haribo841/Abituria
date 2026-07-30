# Deklaracja praw do redystrybucji zasobów

Data deklaracji: 19 lipca 2026 r.

Adam Kubiś, właściciel repozytorium i opiekun bieżącej implementacji Abiturii, oświadcza, że posiada prawa lub skuteczne upoważnienie pozwalające na publiczną redystrybucję niżej wskazanych zasobów jako integralnej części projektu i paczek aplikacji Abituria:

- `cke-2021-correction-exam`: `Content/exam-2021-correction.json`;
- `runtime-vector-diagrams`: `Content/diagrams.json` z 57 deterministycznymi definicjami diagramów;
- `application-icon`: `img/icon.ico`, używany wyłącznie jako ikona aplikacji.

Deklaracja obejmuje publiczną redystrybucję tych zasobów wyłącznie w ramach Abiturii. Stanowi podstawę ustawienia statusu `approved` dla wymienionych grup w `Content/provenance.json`.

Deklaracja:

- nie przypisuje Adamowi Kubisiowi autorstwa materiałów CKE ani zasobów historycznych;
- nie zmienia autorów i źródeł zapisanych w manifeście pochodzenia;
- nie przenosi praw autorskich ani innych praw na odbiorców projektu;
- nie udziela samodzielnej licencji na wyodrębnianie, ponowne licencjonowanie lub dystrybucję tych zasobów poza Abiturią;
- nie zastępuje informacji o autorstwie, źródle i licencjach pozostałych składników projektu.

Zakres plików jest maszynowo weryfikowany przez `tools/Test-ContentProvenance.ps1`. Każda zmiana zasobów, sposobu ich pakowania albo podstawy upoważnienia wymaga ponownego przeglądu manifestu.

## Rozszerzenie deklaracji z 27 lipca 2026 r.

Adam Kubiś rozszerza powyższą deklarację na grupę `cke-formula-2023-transcription`, obejmującą wierną transkrypcję treści dokumentu Centralnej Komisji Egzaminacyjnej „Wybrane wzory matematyczne na egzamin maturalny z matematyki” do pliku `Content/formulas.json` oraz jej powiązanie z diagramami wektorowymi w `Content/diagrams.json`.

Źródłem transkrypcji jest dokument:

- adres: `https://bip.cke.gov.pl/attachments/download/9944`;
- data publikacji wskazana przez CKE: 26 sierpnia 2024 r.;
- suma SHA-256 zweryfikowanego pliku: `57CFF1265A7E38C13ECB6A00F566A37CDFDA667ABF2D550BA65E19E166CC0D45`.

Rozszerzenie obejmuje publiczną redystrybucję transkrypcji wyłącznie jako integralnej części Abiturii, na tych samych zasadach i z tymi samymi ograniczeniami co pierwotna deklaracja. Nie przypisuje Adamowi Kubisiowi autorstwa dokumentu CKE i nie stanowi samodzielnej licencji na dokument źródłowy.

## Archiwizacja rastrów z 29 lipca 2026 r.

Siedemdziesiąt pięć historycznych obrazów przeniesiono bez zmiany bajtów do `docs/legacy/originals/images/`. Nie są one zasobami działania aplikacji, nie trafiają do paczek i są wyłączone z DocFX. `PATH-MAPPING.csv` zachowuje mapowanie dawnych ścieżek, a `SHA256SUMS` pozwala zweryfikować każdy oryginał. Katalog diagramów jest aktywną reprezentacją wizualną, natomiast archiwum pozostaje wyłącznie dowodem historycznym.
