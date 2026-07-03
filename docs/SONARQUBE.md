# SonarQube Cloud i SonarQube for IDE

SonarQube Cloud jest dodatkową bramą jakości. Nie zastępuje kompilacji, testów jednostkowych ani testów wizualnych Avalonia Headless.

## Aktywacja SonarQube Cloud

1. Zaimportuj publiczne repozytorium `haribo841/Abituria` do SonarQube Cloud.
2. W ustawieniach repozytorium GitHub dodaj zmienne Actions:
   - `SONAR_PROJECT_KEY` - klucz utworzonego projektu,
   - `SONAR_ORGANIZATION` - klucz organizacji SonarQube Cloud.
3. Dodaj sekret Actions `SONAR_TOKEN` wygenerowany dla projektu lub organizacji.
4. Wyłącz automatyczną analizę projektu w SonarQube Cloud. Repozytorium używa analizy CI przez SonarScanner for .NET.
5. Ustaw check jakości SonarQube Cloud jako wymagany w ochronie gałęzi `main`.

Workflow `.github/workflows/sonarcloud.yml` uruchamia skaner wokół kompilacji Release, wykonuje testy z pokryciem OpenCover i czeka na wynik quality gate. Dopóki zmienne projektu nie istnieją, zadanie jest bezpiecznie pomijane. Pull requesty z forków nie otrzymują sekretu i również pomijają skan.

## SonarQube for Visual Studio

Należy używać aktualnego rozszerzenia SonarQube for Visual Studio, dawniej SonarLint. Po utworzeniu projektu:

1. Otwórz `Extensions > SonarQube > Connected Mode`.
2. Połącz rozszerzenie z SonarQube Cloud za pomocą własnego tokenu użytkownika.
3. Powiąż rozwiązanie `Abituria.sln` z projektem.
4. Użyj `Share Configuration` i zatwierdź wygenerowany plik `.sonarlint/Abituria.slconfig`.

Pliku powiązania nie należy tworzyć ręcznie przed utworzeniem projektu. Nie zawiera on sekretu, ale musi wskazywać rzeczywisty klucz projektu i organizacji.
