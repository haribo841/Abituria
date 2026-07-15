# Wsparcie

Abituria jest projektem beta utrzymywanym przez jedną osobę. Najskuteczniejszym kanałem kontaktu są zgłoszenia w repozytorium.

## Zanim zgłosisz problem

1. Sprawdź [znane ograniczenia](docs/KNOWN_LIMITATIONS.md).
2. Upewnij się, że używasz paczki przeznaczonej dla swojego systemu i architektury.
3. Zweryfikuj sumę SHA-256 oraz attestation zgodnie z [instrukcją instalacji](docs/INSTALLATION.md#sprawdzenie-integralności-i-pochodzenia).
4. Uruchom diagnostykę wydania z nowym, pustym katalogiem danych:

   ```text
   Abituria --release-smoke-test --data-directory <katalog-tymczasowy>
   ```

5. Sprawdź wersję i commit na ekranie „O programie”.

## Zgłaszanie błędu

Utwórz [nowe GitHub Issue](https://github.com/haribo841/Abituria/issues/new) i podaj:

- krótki, jednoznaczny tytuł;
- wersję aplikacji i commit z ekranu „O programie”;
- system operacyjny, jego wersję i architekturę;
- nazwę pobranej paczki;
- dokładne kroki prowadzące do problemu;
- oczekiwany oraz rzeczywisty rezultat;
- pełny komunikat błędu i kod wyjścia, jeśli problem dotyczy smoke testu;
- informację, czy problem występuje z pustym, tymczasowym katalogiem danych;
- zrzut ekranu, jeśli nie zawiera prywatnych danych.

Nie publikuj bazy `abituria.db`, haseł, kodów odzyskiwania, danych osobowych ani innych sekretów. Jeżeli błąd może być podatnością bezpieczeństwa, nie twórz publicznego issue i postępuj zgodnie z [SECURITY.md](SECURITY.md).

## Propozycje funkcji

Przed utworzeniem zgłoszenia sprawdź [roadmapę](docs/ROADMAP.md) i istniejące issues. Opisz problem użytkownika, oczekiwany efekt, przykładowy scenariusz i wpływ na obecne funkcje. Generator wykresów, kalkulator trygonometryczny, natywne instalatory i automatyczna aktualizacja są obecnie poza zakresem wydania `0.9.0-beta.1`.

## Pytania o materiały i licencje

Pytania dotyczące źródeł, autorstwa lub prawa do dystrybucji zasobów należy oznaczać jako problem pochodzenia treści i odnieść do [inwentarza pochodzenia](docs/CONTENT_PROVENANCE.md). Nie wolno obchodzić blokady publicznego wydania przez ręczne pominięcie kontroli zasobów.

## Czas odpowiedzi

Projekt nie gwarantuje indywidualnego SLA. Zgłoszenia kompletne, odtwarzalne i dotyczące wspieranego środowiska beta mają pierwszeństwo przed propozycjami funkcji.
