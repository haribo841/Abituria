# Protokół kryteriów akceptacji i oceny - Issue #45

Wersja dokumentacji: `0.9.0-beta.1`.

Data sporządzenia protokołu retrospektywnego: 19 lipca 2026 r.

## Cel i zakres protokołu

Protokół formalizuje kryteria opisane w [Issue #45 - Kryteria akceptacji](https://github.com/Projekt-Inzynierski-AK-AS-FD/Projekt-Inzynierski/issues/45) oraz rozdziela dwa różne przedmioty oceny:

- `HISTORYCZNY 2022` - projekt WPF przedstawiony komisji 17 stycznia 2022 r., historycznie przetestowany, wdrożony i oceniony;
- `BIEŻĄCA MIGRACJA` - implementacja AvaloniaUI `0.9.0-beta.1`, rozwijana i weryfikowana w repozytorium `haribo841/Abituria`, ale jeszcze nieopublikowana w GitHub Releases.

Pozytywny wynik historycznej obrony nie jest przedstawiany jako ocena bieżącej migracji. Brak publicznego wydania bieżącej migracji nie unieważnia historycznej decyzji komisji.

## Statusy

- `PASS` - istnieje bezpośredni, zachowany dowód spełnienia;
- `PASS RETROSPEKTYWNY` - wynik został jednoznacznie poświadczony, ale pełny materiał pierwotny nie został zachowany;
- `PARTIAL` - zachowano tylko część wymaganego dowodu albo kryterium ma charakter jakościowy;
- `PENDING` - wymagane działanie dotyczące bieżącej migracji nie zostało jeszcze wykonane;
- `NOT APPLICABLE` - kryterium historycznej komisji nie jest ponownie stosowane do bieżącej rekonstrukcji.

## Źródła dowodowe

| Źródło | Zakres dowodu | Ograniczenie |
| --- | --- | --- |
| Issue #45 | oryginalne kryteria prezentacji, produktu, testów, wdrożenia i pracy zespołu | zgłoszenie określa kryteria, ale nie zapisuje wyniku oceny |
| `DEFENSE_PROTOCOL.md` | data, przebieg, skład komisji, demonstracja, Q&A i poświadczony wynik | wynik i pełny skład nie są ogłoszone w zachowanej części transmisji |
| nagranie `Projekt Inz final` | prezentacja, demonstracja, pytania, odpowiedzi i zapowiedź narady komisji | nagranie ma status `unlisted` i kończy się przed ogłoszeniem formalnego wyniku |
| [prerelease `v1.0.0`](https://github.com/Projekt-Inzynierski-AK-AS-FD/Projekt-Inzynierski/releases/tag/v1.0.0) | paczka wdrożeniowa dostępna przed obroną | brak zachowanego protokołu wskazującego ją jako formalną kopię zapasową |
| [release `v1.0.1`](https://github.com/Projekt-Inzynierski-AK-AS-FD/Projekt-Inzynierski/releases/tag/v1.0.1) | publiczne archiwum aplikacji z 18 stycznia 2022 r. | aplikacja zależy od Windows Desktop Runtime 3.1; historyczna instrukcja instalacji i raport testów nie zachowały się |
| poświadczenie właściciela z 19 lipca 2026 r. | pozytywna decyzja komisji, wynik bardzo dobry, historyczne testy uczestników i późniejszy odbiór prowadzącego | brak podpisanej karty oceny, indywidualnych ocen i osobnej opinii prowadzącego |
| aktywny kod, dokumentacja i testy | stan techniczny bieżącej migracji | lokalne zmiany nie są jeszcze wersjonowanym wydaniem ani wynikiem komisji |

### Zachowane artefakty wdrożeniowe

| Wydanie | Data publikacji | Artefakt | Rozmiar | SHA-256 |
| --- | --- | --- | ---: | --- |
| `v1.0.0` prerelease | 12 stycznia 2022 r. | `Pre-release.zip` z `setup.exe`, manifestem ClickOnce i plikami aplikacji | 3 178 921 B | `837C38D0EB8A53E56B51598B07FE8087C6E0B7E85A398394AFFF04A8353A0DC1` |
| `v1.0.1` release | 18 stycznia 2022 r. | `Abituria.zip` z `setup.exe`, `Abituria.exe`, manifestem i zależnościami | 12 737 712 B | `E88F327B172B1CD78A0F7E9F646378D6032267DA16464218051D0F030E87D6DE` |

Artefakty potwierdzają przygotowanie i publiczne udostępnienie wersji wdrożeniowych. Nie zawierają zachowanej instrukcji instalacji ani wymagań systemowych. `v1.0.1` jest aplikacją WPF zależną od `Microsoft.WindowsDesktop.App` 3.1.0 oraz lokalnego SQL Server LocalDB, którego silnika nie ma w archiwum. Pliki wykonywalne nie stanowią obecnie zaufanego, podpisanego instalatora. Nie zachowano też protokołu instalacji na niezależnym komputerze.

Tagi `v1.0.0` i `v1.0.1` nie mają wspólnego przodka według porównania GitHub, dlatego protokół nie przedstawia `v1.0.1` jako dowiedzionego przyrostu `v1.0.0`. Są to dwa odrębne, zachowane artefakty. Dowód historycznego wdrożenia tworzą łącznie paczki, demonstracja, pozytywna ocena i poświadczenie właściciela, a nie sam fakt istnienia archiwów.

## Dowód prezentacji i demonstracji

Nagranie jest dostępne pod adresem [youtube.com/watch?v=Min_5DBHHnQ](https://www.youtube.com/watch?v=Min_5DBHHnQ). Metadane wskazują zakończoną transmisję na żywo z 17 stycznia 2022 r., czas trwania 2:10:03 oraz widoczność `unlisted`, czyli dostęp przez link bez publicznego wyszukiwania.

| Moment | Zdarzenie |
| --- | --- |
| [1:35:30](https://www.youtube.com/watch?v=Min_5DBHHnQ&t=5730s) | plansza tytułowa Abiturii |
| [1:36:38](https://www.youtube.com/watch?v=Min_5DBHHnQ&t=5798s) | rozpoczęcie właściwego omówienia projektu |
| [1:57:27](https://www.youtube.com/watch?v=Min_5DBHHnQ&t=7047s) | rozpoczęcie demonstracji działającej aplikacji |
| [2:02:41](https://www.youtube.com/watch?v=Min_5DBHHnQ&t=7361s) | zakończenie demonstracji |
| [2:02:49](https://www.youtube.com/watch?v=Min_5DBHHnQ&t=7369s) | rozpoczęcie pytań dotyczących odpowiedzi otwartych i dydaktyki |
| [2:06:00](https://www.youtube.com/watch?v=Min_5DBHHnQ&t=7560s) | pytanie o dalszy rozwój programu |
| [2:07:32](https://www.youtube.com/watch?v=Min_5DBHHnQ&t=7652s) | pozytywne podsumowanie zakresu pracy i gratulacje |
| [2:09:18](https://www.youtube.com/watch?v=Min_5DBHHnQ&t=7758s) | zapowiedź narady komisji |

Nagranie bezpośrednio potwierdza prezentację, demonstrację oraz sesję pytań i odpowiedzi. Nie zawiera ogłoszenia formalnej decyzji ani oceny bardzo dobrej. Te dwa fakty są zapisane jako poświadczenie właściciela projektu.

## Ocena siedmiu obszarów

| Obszar Issue #45 | HISTORYCZNY 2022 | BIEŻĄCA MIGRACJA | Uzasadnienie |
| --- | --- | --- | --- |
| 1. Prezentacja projektu i produktu | PASS | NOT APPLICABLE | nagranie potwierdza omówienie projektu, demonstrację i Q&A; bieżąca migracja nie była przedmiotem komisji z 2022 r. |
| 2. Uzyskany produkt | PASS RETROSPEKTYWNY | PASS techniczny | komisja zaakceptowała historyczny produkt; bieżący zakres obejmuje konta, SQLite, 7 dostępnych działów, 35 zadań, podpowiedzi, losowanie i dwa kalkulatory |
| 2a. Innowacyjność | PARTIAL | PARTIAL | integracja nauki, zadań, podpowiedzi i narzędzi jest udokumentowana, ale nie zachowano osobnej punktacji ani porównawczej analizy innowacyjności |
| 3. Dokumentacja projektu | PARTIAL historyczny | PASS lokalny | historyczne wydanie nie zachowało pełnego zestawu według obecnego standardu; bieżący pakiet obejmuje wymagania, architekturę, instalację, testy, licencje, autorstwo, wersje i ograniczenia |
| 4. Testowanie produktu | PASS RETROSPEKTYWNY | PASS | właściciel potwierdził pomyślne testy historyczne; bieżący zestaw obejmuje testy jednostkowe, integracyjne, funkcjonalne, regresyjne, UI, instalacyjne, wydajnościowe, pamięciowe i obciążeniowe |
| 5. Wdrożenie | PASS | PENDING publicznego wydania | `v1.0.0` było dostępne przed obroną, a `v1.0.1` opublikowano dzień po niej; obecna migracja ma zielone smoke testy trzech systemów, ale nie ma tagu ani GitHub Release |
| 6. Przebieg pracy zespołu | PARTIAL | PASS dla pracy jednoosobowej | stare repozytorium zachowuje Issues, Project, przepływ kolumn i PR-y, lecz brak osobnej pisemnej opinii prowadzącego; bieżącą implementację rozwija Adam Kubiś |
| 7. Ocena zespołowa i indywidualny wkład | PARTIAL | PASS dla autorstwa bieżącego | poświadczono wynik projektu i obrony, ale nie zachowano rozdziału ocen indywidualnych; `AUTHORS.md` prawidłowo przypisuje bieżącą implementację Adamowi Kubisiowi |

Zakres i jakość bieżącej migracji są oceniane względem zatwierdzonego, jawnego zakresu, a nie względem niewdrożonych obietnic historycznych. Dwa placeholdery i funkcje poza zakresem beta są opisane jako ograniczenia, a nie jako istniejące możliwości.

## Warunki uznania projektu za zaakceptowany

| Nr | Warunek | HISTORYCZNY 2022 | BIEŻĄCA MIGRACJA | Dowód lub ograniczenie |
| --- | --- | --- | --- | --- |
| 1 | działa zgodnie z zatwierdzonym zakresem | PASS RETROSPEKTYWNY | PASS | demonstracja oraz kryteria w `REQUIREMENTS.md` |
| 2 | realizuje główne wymagania funkcjonalne | PASS RETROSPEKTYWNY | PASS | demonstracja, testy funkcjonalne i macierz wymagań |
| 3 | nie zawiera błędów krytycznych | PASS RETROSPEKTYWNY | PASS lokalny | pozytywna obrona; bieżący build, testy i SonarCloud dla commita bazowego |
| 4 | został przetestowany | PASS RETROSPEKTYWNY | PASS | poświadczenie historyczne oraz pełny raport `TESTING.md` |
| 5 | posiada aktualną dokumentację | PARTIAL historyczny | PASS lokalny | obecna dokumentacja jest kompletna; historyczny pakiet nie zachował wszystkich dzisiejszych elementów |
| 6 | można go uruchomić poza środowiskiem autorów | PASS RETROSPEKTYWNY | PASS techniczny | zachowane paczki, demonstracja i pozytywny odbiór historyczny; brak osobnego protokołu niezależnej instalacji; natywne runnery Windows, Ubuntu i macOS dla bieżącej migracji |
| 7 | został zaprezentowany przed komisją | PASS | NOT APPLICABLE | nagranie obrony 17 stycznia 2022 r. |
| 8 | decyzje projektowe i techniczne zostały uzasadnione | PASS | PASS dokumentacyjny | Q&A na nagraniu oraz aktualna dokumentacja architektury i ograniczeń |
| 9 | ukończono obowiązkowe elementy wdrożenia | PASS | PENDING | historyczne wydania istnieją; bieżące `0.9.0-beta.1` nie zostało opublikowane |
| 10 | prowadzący potwierdził realizację przyrostów | PASS RETROSPEKTYWNY | NOT APPLICABLE | poświadczony odbiór na początku lutego 2022 r.; szczegółowe decyzje I-III nie zostały zachowane |

Wynik historyczny: `ACCEPTED`.

Wynik bieżącej migracji: `TECHNICALLY ACCEPTED - PUBLIC RELEASE PENDING`.

## Warunki uzyskania oceny bardzo dobrej

| Warunek | HISTORYCZNY 2022 | BIEŻĄCA MIGRACJA |
| --- | --- | --- |
| pełne testowanie | PASS RETROSPEKTYWNY | PASS |
| skuteczne wdrożenie | PASS | PENDING |
| zatwierdzone wymagania | PASS RETROSPEKTYWNY | PASS w uzgodnionym zakresie |
| wysoka jakość techniczna | PASS RETROSPEKTYWNY | PASS lokalny; zdalna baza jakości dotyczy `e0afeea` |
| kompletna i spójna dokumentacja | PARTIAL historyczny | PASS lokalny |
| stabilność i użyteczność | PASS RETROSPEKTYWNY | PASS techniczny i PASS RETROSPEKTYWNY testów uczestników |
| rzeczowa prezentacja | PASS | NOT APPLICABLE |
| systematyczna praca | PARTIAL bez osobnej opinii | PASS na podstawie historii bieżącego repozytorium |
| właściwe narzędzia | PASS | PASS |
| zdolność wyjaśnienia architektury, implementacji, testów i ograniczeń | PASS | PASS dokumentacyjny |

Komisja przyznała historycznemu projektowi wynik bardzo dobry zgodnie z poświadczeniem właściciela z 19 lipca 2026 r. Protokół nie rekonstruuje nieistniejących ocen cząstkowych i nie twierdzi, że bieżąca migracja otrzymała ocenę komisji.

## Praca zespołu i autorstwo

Historyczne repozytorium potwierdza korzystanie z GitHub Issues, klasycznego GitHub Project, kolumn `To do`, `In progress`, `Review in progress`, `Reviewer approved` i `Done` oraz pull requestów. Jest to dowód organizacji procesu, ale nie samodzielny dowód autorstwa obecnego kodu ani podstawy do przypisywania dawnym osobom konkretnych ról.

Opisy obu historycznych wydań wskazują, że projekt był realizowany przez grupę dwuosobową. Jest to informacja o organizacji projektu WPF z 2022 r., a nie dowód konkretnego zakresu prac każdej osoby ani autorstwa bieżącej implementacji.

Autorem i opiekunem bieżącej implementacji jest Adam Kubiś. Aliasowe warianty autora w historii Git odnoszą się do tej samej osoby. Innych aktywnych autorów lub ról nie dopisuje się na podstawie historycznych przydziałów, komentarzy albo pojedynczych commitów do usuniętej lub przepisanej wersji WPF.

Nie zachowano osobnej pisemnej opinii prowadzącego o systematyczności i zaangażowaniu ani dowodu, czy oceny członków historycznego zespołu były jednakowe. Poprawny zapis brzmi: `projekt i obrona uzyskały wynik bardzo dobry`. Nie należy twierdzić, że każda historyczna osoba otrzymała indywidualnie ocenę bardzo dobrą.

## Decyzja formalna dla Issue #45

Historyczne kryteria zostały zastosowane podczas obrony i zakończyły się akceptacją projektu oraz wynikiem bardzo dobrym. Projekt posiadał publiczną paczkę przed obroną i wydanie bezpośrednio po niej. Ograniczenia zachowanych testów, dokumentacji oraz opinii prowadzącego są jawne.

Decyzja: `ACCEPTED - HISTORICAL CRITERIA SATISFIED - READY TO CLOSE AS COMPLETED`.

Bieżące publiczne wydanie `0.9.0-beta.1` pozostaje osobnym zadaniem i nie jest warunkiem retrospektywnego zamknięcia Issue #45.

## Checklista formalnego zamknięcia

- [x] zapisano wszystkie siedem obszarów oceny;
- [x] oceniono dziesięć warunków akceptacji;
- [x] oceniono warunki wyniku bardzo dobrego;
- [x] wskazano nagranie z indeksami czasu;
- [x] rozdzielono treść nagrania od poświadczenia wyniku;
- [x] wskazano historyczne paczki wdrożeniowe;
- [x] ujawniono brak podpisanej karty oceny i osobnej opinii prowadzącego;
- [x] rozdzielono autorstwo bieżącej implementacji od historycznej pracy zespołu;
- [x] rozdzielono historyczne wdrożenie od nieopublikowanej migracji;
- [x] przygotowano treść komentarza zamykającego.

## Treść komentarza zamykającego

```markdown
Issue #45 zostało spełnione dla historycznego projektu Abituria.

- Publiczna obrona odbyła się 17 stycznia 2022 r. Pokazano działającą aplikację i przeprowadzono Q&A: https://www.youtube.com/watch?v=Min_5DBHHnQ&t=5730s
- Prerelease v1.0.0 z paczką aplikacji opublikowano 12 stycznia 2022 r.: https://github.com/Projekt-Inzynierski-AK-AS-FD/Projekt-Inzynierski/releases/tag/v1.0.0
- Wydanie v1.0.1 opublikowano 18 stycznia 2022 r.: https://github.com/Projekt-Inzynierski-AK-AS-FD/Projekt-Inzynierski/releases/tag/v1.0.1
- Komisja zaakceptowała projekt, a wynik projektu i obrony był bardzo dobry. Wynik jest poświadczeniem właściciela; zachowane nagranie kończy się przed jego formalnym ogłoszeniem.
- Historyczne testy uczestników zakończyły się powodzeniem według poświadczenia właściciela. Brakujące karty sesji, oceny cząstkowe i osobna opinia prowadzącego nie są rekonstruowane.
- Bieżąca migracja AvaloniaUI 0.9.0-beta.1 oraz jej przyszłe publiczne wydanie są odrębnym zakresem i nie zmieniają historycznej decyzji komisji.
- Pełna macierz i ograniczenia dowodowe znajdują się w `docs/EVALUATION_PROTOCOL.md`.

Decyzja: completed.
```
