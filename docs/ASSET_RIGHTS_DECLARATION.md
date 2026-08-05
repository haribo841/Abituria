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
