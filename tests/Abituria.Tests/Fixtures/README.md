# Historyczna baza zgodności .NET 9

Plik `net9-efcore-9.0.17.db` jest niezmiennym fixture testowym utworzonym przez rzeczywisty kod z commita `92b75dc` - ostatniego commita aplikacji na `net9.0` przed migracją wydaniową do .NET 10.

Generator był osobnym, tymczasowym programem `net9.0`, który referował historyczny `Abituria.csproj` i wykonał migrację `InitialLocalAccounts` przez Entity Framework Core `9.0.17`. Następnie historyczne encje oraz `PasswordHasher` utworzyły konto `Uczeń z .NET 9` z domyślnymi `600000` iteracji PBKDF2 i postępem `mp21-z7`. Generator uruchomiono przez SDK `9.0.315` i runtime `9.0.17`.

Fixture nie zawiera danych rzeczywistego użytkownika. Identyfikator profilu, hasło testowe i czasy są sztuczne. Plik nie jest zasobem aplikacji i nie trafia do paczek wydania.

SHA-256:

```text
AA006B46A3E7384EB87A92DD4164B9364D50CF78C873322A870CC5582923E1ED  net9-efcore-9.0.17.db
```

Test przed każdym użyciem sprawdza sumę, wpis migracji EF `9.0.17`, historyczny koszt hasła, logowanie i trwałość postępu po otwarciu przez bieżący kod .NET 10 oraz ponownym uruchomieniu usług.
