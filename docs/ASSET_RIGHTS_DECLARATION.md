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

## Rozszerzenie deklaracji z 3 sierpnia 2026 r.

Adam Kubiś rozszerza deklarację na publiczną redystrybucję w ramach Abiturii następujących grup:

- `cke-2026-main-basic-exam`: strukturalna transkrypcja arkusza `MMAP-P0-100-A-2605` i zasad oceniania `MMAP-P0-100-2605` w `Content/exam-2026-main-basic.json`;
- `cke-2026-main-extended-exam`: strukturalna transkrypcja arkusza `MMAP-R0-100-A-2605` i zasad oceniania `MMAP-R0-100-2605` w `Content/exam-2026-main-extended.json`;
- `runtime-vector-diagrams`: katalog `Content/diagrams.json` rozszerzony do 67 deterministycznych definicji, w tym o dziesięć autorskich implementacji wektorowych Avalonia dla zadań maturalnych 2026.

Źródła CKE objęte rozszerzeniem:

- arkusz podstawowy: `https://cke.gov.pl/images/_EGZAMIN_MATURALNY_OD_2023/Arkusze_egzaminacyjne/2026/Matematyka/poziom_podstawowy/MMAP-P0-100-A-2605-arkusz.pdf`, SHA-256 `B7BD89434CA5CCFA33824B0CF063FF7CDDFF47B353059ECF225418E29BEEE71D`;
- zasady oceniania poziomu podstawowego: `https://cke.gov.pl/images/_EGZAMIN_MATURALNY_OD_2023/Arkusze_egzaminacyjne/2026/Matematyka/poziom_podstawowy/MMAP-P0-100-2605-zasady.pdf`, SHA-256 `A982890CF5EA17206266E4A64B7BFDF96F46FAB08C7435B022CCE5B3908A65AC`;
- arkusz rozszerzony: `https://cke.gov.pl/images/_EGZAMIN_MATURALNY_OD_2023/Arkusze_egzaminacyjne/2026/Matematyka/poziom_rozszerzony/MMAP-R0-100-A-2605-arkusz.pdf`, SHA-256 `DEC5F06020C35DCDABAB5747942BEDFC49CF7307B27F0AD105FAA93741D03964`;
- zasady oceniania poziomu rozszerzonego: `https://cke.gov.pl/images/_EGZAMIN_MATURALNY_OD_2023/Arkusze_egzaminacyjne/2026/Matematyka/poziom_rozszerzony/MMAP-R0-100-2605-zasady.pdf`, SHA-256 `D7C014240AF16885DBDD1711D923AFF24951B2F514B7C5659E0B6F16508878BD`.

Dziesięć nowych diagramów nie jest kopiami ani osadzonymi rastrami arkuszy. Adam Kubiś zaimplementował ich matematyczną treść jako własne, deterministyczne zestawy kontrolek i kształtów Avalonia: `Line`, `Polyline`, `Polygon`, `Ellipse`, łuki aproksymowane polilinią oraz `TextBlock`. Definicje są objęte licencją MIT kodu Abiturii, zachowują przypisanie kontekstu i stron źródłowych CKE oraz nie przypisują Adamowi Kubisiowi autorstwa arkuszy.

Rozszerzenie zachowuje wszystkie ograniczenia pierwotnej deklaracji. Nie udziela samodzielnej licencji na źródłowe pliki PDF ani na wyodrębnianie transkrypcji poza Abiturią.

## Rozszerzenie deklaracji z 5 sierpnia 2026 r.

Na podstawie bezpośredniego polecenia wdrożenia i publikacji wydania Adam Kubiś rozszerza deklarację na publiczną redystrybucję w ramach Abiturii następujących grup:

- `cke-2025-main-basic-exam`: strukturalna transkrypcja arkusza `MMAP-P0-100-A-2505` i zasad oceniania `MMAP-P0-100-2505` w `Content/exam-2025-main-basic.json`;
- `cke-2025-main-extended-exam`: strukturalna transkrypcja arkusza `MMAP-R0-100-A-2505` i zasad oceniania `MMAP-R0-100-2505` w `Content/exam-2025-main-extended.json`;
- `runtime-vector-diagrams`: katalog `Content/diagrams.json` rozszerzony do 76 deterministycznych definicji, w tym o dziewięć autorskich implementacji wektorowych Avalonia dla zadań matury podstawowej 2025.

Źródła CKE objęte rozszerzeniem:

- arkusz podstawowy: `https://cke.gov.pl/images/_EGZAMIN_MATURALNY_OD_2023/Arkusze_egzaminacyjne/2025/Matematyka/poziom_podstawowy/MMAP-P0-100-A-2505-arkusz.pdf`, SHA-256 `C5F8AFDE91393BEA3E5980560ADA103389679473DBD0C11A7485040F06631C85`;
- zasady oceniania poziomu podstawowego: `https://cke.gov.pl/images/_EGZAMIN_MATURALNY_OD_2023/Arkusze_egzaminacyjne/2025/zasady_oceniania/MMAP-P0-100-2505-zasady.pdf`, SHA-256 `D272201B35AD7829315C6897500F036A8619BBEE42B38291037DF952F9F150E5`;
- arkusz rozszerzony: `https://cke.gov.pl/images/_EGZAMIN_MATURALNY_OD_2023/Arkusze_egzaminacyjne/2025/Matematyka/poziom_rozszerzony/MMAP-R0-100-A-2505-arkusz.pdf`, SHA-256 `457B057602D81CF93A9688E7F4CB74103F4579B37C2B1A2A9AACE28C891CD4AD`;
- zasady oceniania poziomu rozszerzonego: `https://cke.gov.pl/images/_EGZAMIN_MATURALNY_OD_2023/Arkusze_egzaminacyjne/2025/zasady_oceniania/MMAP-R0-100-2505-zasady.pdf`, SHA-256 `B196084F2B9505D14C66E3CBE0064BBA7E4BA0F3FFA613500ED701A97724E523`.

Dziewięć nowych diagramów nie jest kopiami ani osadzonymi rastrami arkusza. Adam Kubiś zaimplementował ich matematyczną treść jako własne, deterministyczne zestawy kontrolek i kształtów Avalonia. Definicje są objęte licencją MIT kodu Abiturii, zachowują przypisanie kontekstu i stron źródłowych CKE oraz nie przypisują Adamowi Kubisiowi autorstwa arkusza.

Rozszerzenie zachowuje wszystkie ograniczenia pierwotnej deklaracji. Nie udziela samodzielnej licencji na źródłowe pliki PDF ani na wyodrębnianie transkrypcji poza Abiturią.

## Rozszerzenie deklaracji z 10 sierpnia 2026 r.

Na podstawie bezpośredniej decyzji właściciela repozytorium Adam Kubiś rozszerza deklarację na publiczną redystrybucję wyłącznie jako integralnej części Abiturii następujących grup:

- `cke-formula-2023-guide-examples`: `Content/official-course-examples.json`, zawierający 97 transkrybowanych przykładów, ich rozwiązania, kryteria, odwołania do wymagań i opisy informacji wizualnej;
- `cke-2024-main-basic-exam`: `Content/exam-2024-main-basic.json`, obejmujący arkusz `MMAP-P0-100-A-2405` oraz zasady oceniania `MMAP-P0-100-2405`;
- `cke-2024-main-extended-exam`: `Content/exam-2024-main-extended.json`, obejmujący arkusz `MMAP-R0-100-A-2405` oraz zasady oceniania `MMAP-R0-100-2405`;
- `runtime-vector-diagrams`: rozszerzenie katalogu `Content/diagrams.json` do 88 deterministycznych definicji, w tym jedenastu własnych implementacji wektorowych figur matury 2024 PP i jednej figury matury 2024 PR.

Źródła CKE objęte rozszerzeniem:

- informator podstawowy Formuły 2023: `https://bip.cke.gov.pl/attachments/download/10085`, SHA-256 `88A0EA8E2EE444506CCA5E89C860178E33B04F181650A36D9C9B4DC9BBE625B2`;
- informator rozszerzony Formuły 2023: `https://bip.cke.gov.pl/attachments/download/10088`, SHA-256 `BD408CDC8877E04EC79AAC3177FAB304E6F66C6B5FA152D8D3436D4ACFB2BC6F`;
- arkusz podstawowy 2024: `https://cke.gov.pl/images/_EGZAMIN_MATURALNY_OD_2023/Arkusze_egzaminacyjne/2024/Matematyka/poziom_podstawowy/MMAP-P0-100-A-2405-arkusz.pdf`, SHA-256 `37BDABE139A83CDD128E35C5A37A6E17DE5C4423F7DB2C6B4887EC8ADD96B7A0`;
- zasady oceniania poziomu podstawowego 2024: `https://cke.gov.pl/images/_EGZAMIN_MATURALNY_OD_2023/Arkusze_egzaminacyjne/2024/Matematyka/poziom_podstawowy/MMAP-P0-100-2405-zasady.pdf`, SHA-256 `28D7232FBD3EB77CCF17AAEADE8F541564FBC4E9B59E4547BE4E67DB386D5202`;
- arkusz rozszerzony 2024: `https://cke.gov.pl/images/_EGZAMIN_MATURALNY_OD_2023/Arkusze_egzaminacyjne/2024/Matematyka/poziom_rozszerzony/MMAP-R0-100-A-2405-arkusz.pdf`, SHA-256 `873691F2E3740126D969AAC957CBC5666FCAD7D7FCF8499442781E18F6AD53D6`;
- zasady oceniania poziomu rozszerzonego 2024: `https://cke.gov.pl/images/_EGZAMIN_MATURALNY_OD_2023/Arkusze_egzaminacyjne/2024/Matematyka/poziom_rozszerzony/MMAP-R0-100-2405-zasady.pdf`, SHA-256 `6535405993D0A9F3360759A2B2335BF7523E177144B207B4D8A6E3D8A3A8AB92`.

Diagramy są autorską implementacją matematycznej treści w obsługiwanych prymitywach Avalonia i nie są kopiami rastrów ani samodzielnymi reprodukcjami plików PDF. Rozszerzenie nie przypisuje Adamowi Kubisiowi autorstwa materiałów CKE, nie przenosi praw na odbiorców i nie udziela samodzielnej licencji na źródłowe PDF-y lub wyodrębnione transkrypcje. Zachowuje wszystkie ograniczenia pierwotnej deklaracji, a jednocześnie stanowi podstawę statusu `approved` wymienionych grup w `Content/provenance.json`.

## Rozszerzenie deklaracji z 12 sierpnia 2026 r.

Na podstawie bezpośredniej decyzji właściciela repozytorium Adam Kubiś rozszerza deklarację na publiczną redystrybucję wyłącznie jako integralnej części Abiturii następujących grup:

- `cke-2023-main-extended-exam`: `Content/exam-2023-main-extended.json`, obejmujący arkusz `MMAP-R0-100-2305` oraz zasady oceniania `MMAP-R0-100-2305`;
- `runtime-vector-diagrams`: rozszerzenie katalogu `Content/diagrams.json` do 92 deterministycznych definicji, w tym czterech własnych implementacji wektorowych figur matury rozszerzonej 2023.

Źródła CKE objęte rozszerzeniem:

- arkusz rozszerzony 2023: `https://cke.gov.pl/images/_EGZAMIN_MATURALNY_OD_2023/Arkusze_egzaminacyjne/2023/Matematyka/poziom_rozszerzony/MMAP-R0-100-2305.pdf`, SHA-256 `24EC13FEA77323841A8538E85B816C7EE36199E64F5F756E30340489864EC207`;
- zasady oceniania poziomu rozszerzonego 2023: `https://cke.gov.pl/images/_EGZAMIN_MATURALNY_OD_2023/Arkusze_egzaminacyjne/2023/Matematyka/poziom_rozszerzony/MMAP-R0-100-2305-zasady.pdf`, SHA-256 `B8FECD4D23811033E0DFF6C532A405F04ECE0CCC469A6D60412E353F1BBDBD2B`.

Nowe diagramy są autorską implementacją matematycznej treści w obsługiwanych prymitywach Avalonia i nie są kopiami rastrów ani samodzielnymi reprodukcjami plików PDF. Rozszerzenie nie przypisuje Adamowi Kubisiowi autorstwa materiałów CKE, nie przenosi praw na odbiorców i nie udziela samodzielnej licencji na źródłowe PDF-y lub wyodrębnione transkrypcje. Zachowuje wszystkie ograniczenia pierwotnej deklaracji, a jednocześnie stanowi podstawę statusu `approved` wymienionych grup w `Content/provenance.json`.
## Rozszerzenie deklaracji z 1 września 2026 r.

Adam Kubiś, właściciel repozytorium i opiekun bieżącej implementacji Abiturii, potwierdza posiadanie praw lub skutecznego upoważnienia do publicznej redystrybucji niżej wskazanych materiałów CKE wyłącznie jako integralnej części Abiturii:

- `cke-2018-main-basic-exam`, `cke-2018-main-extended-exam` i `cke-2018-correction-basic-exam`;
- `cke-2019-main-basic-exam`, `cke-2019-main-extended-exam` i `cke-2019-correction-basic-exam`;
- `cke-2020-main-basic-exam`, `cke-2020-main-extended-exam` i `cke-2020-correction-basic-exam`;
- `cke-2021-main-basic-exam` i `cke-2021-main-extended-exam`;
- `cke-2022-main-basic-exam`, `cke-2022-main-extended-exam` i `cke-2022-correction-basic-exam`;
- `cke-2023-main-basic-exam` i `cke-2023-correction-basic-exam`;
- `cke-2024-correction-basic-exam` i `cke-2025-correction-basic-exam`;
- `runtime-vector-diagrams`, rozszerzony do 195 deterministycznych definicji, w tym 103 figur pochodnych z wymienionych arkuszy.

Zakres obejmuje strukturalne transkrypcje zadań, odpowiedzi, rozwiązań i kryteriów oceniania oraz własne implementacje wektorowe Avalonia powiązane z tymi arkuszami. Dokładne adresy źródeł, sumy SHA-256, daty weryfikacji i przypisanie stron są zapisane w `Content/provenance.json` oraz macierzach `MATURA_2018_*_COVERAGE.md` do `MATURA_2025_CORRECTION_BASIC_COVERAGE.md`.

Rozszerzenie stanowi podstawę ustawienia statusu `approved` dla wymienionych grup i wartości `releaseEligible=true` w manifeście. Nie przypisuje Adamowi Kubisiowi autorstwa materiałów CKE, nie przenosi praw na odbiorców i nie udziela samodzielnej licencji na źródłowe pliki PDF ani na wyodrębnione transkrypcje poza Abiturią.
