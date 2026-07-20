# Zależności Abiturii

Ten dokument jest generowany przez `tools/Generate-DependencyDocumentation.ps1` z przypiętych plików `packages.lock.json`, manifestu narzędzi i workflow. Nie należy edytować tabel ręcznie.

## Zależności bezpośrednie

| Pakiet | Wersja | Zakres | Zastosowanie | Licencja |
| --- | --- | --- | --- | --- |
| `Avalonia` | `12.0.4` | produkcyjna i testowa | Podstawowy framework interfejsu desktopowego. | MIT |
| `Avalonia.Desktop` | `12.0.4` | produkcyjna i testowa | Klasyczny cykl życia aplikacji desktopowej i backendy systemowe. | MIT |
| `Avalonia.Headless.XUnit` | `12.0.4` | testowa | Testy interfejsu bez ekranu. | MIT |
| `Avalonia.Themes.Fluent` | `12.0.4` | produkcyjna i testowa | Motyw i kontrolki Fluent. | MIT |
| `CommunityToolkit.Mvvm` | `8.4.2` | produkcyjna i testowa | Elementy modelu MVVM. | MIT |
| `coverlet.collector` | `6.0.4` | testowa | Pomiar pokrycia testów. | MIT |
| `Microsoft.EntityFrameworkCore.Sqlite` | `10.0.10` | produkcyjna i testowa | Trwałe profile i postęp w lokalnej bazie SQLite. | MIT |
| `Microsoft.Extensions.DependencyInjection` | `10.0.10` | produkcyjna i testowa | Kontener usług aplikacji. | MIT |
| `Microsoft.NET.Test.Sdk` | `17.14.1` | testowa | Host i integracja uruchamiania testów .NET. | MIT |
| `SQLitePCLRaw.bundle_e_sqlite3` | `2.1.12` | produkcyjna i testowa | Przypięta, poprawiona biblioteka natywna SQLite. | Apache-2.0 |
| `Sylinko.CSharpMath.Avalonia` | `12.0.0` | produkcyjna i testowa | Renderowanie wzorów matematycznych. | MIT |
| `xunit.runner.visualstudio` | `3.1.5` | testowa | Adapter testów dla dotnet test i Visual Studio. | Apache-2.0 |
| `xunit.v3` | `3.2.2` | testowa | Framework testów automatycznych. | Apache-2.0 |

## Narzędzia kompilacji, analizy i publikacji

Narzędzia w tej tabeli nie są częścią grafu runtime ani paczek aplikacji. Są przypiętymi pakietami .NET używanymi do tworzenia dokumentacji, SBOM i analizy jakości.

| Pakiet | Wersja | Zastosowanie | Licencja | Źródło wersji |
| --- | --- | --- | --- | --- |
| `docfx` | `2.78.5` | Budowanie kanonicznej strony dokumentacji i kontrola ostrzeżeń. | MIT | `.config/dotnet-tools.json` |
| `Microsoft.Sbom.DotNetTool` | `4.1.5` | Generowanie osobnego manifestu SPDX dla każdej paczki. | MIT | `.config/dotnet-tools.json` |
| `dotnet-sonarscanner` | `11.2.1` | Analiza C# i oczekiwanie na wynik SonarQube Quality Gate. | LGPL-3.0 | workflow SonarQube i wydania |

## Pełne rozwiązanie zależności

Tabela obejmuje również zależności przechodnie. Dokładne grafy dla każdego targetu pozostają w lockfile, a każde wydanie otrzymuje osobny SBOM SPDX.

`Avalonia.BuildServices 11.3.2` nie jest bezpośrednią zależnością Abiturii. Pozostaje wyłącznie przechodnim narzędziem czasu kompilacji wymaganym przez metapakiet `Avalonia 12.0.4` i nie należy do grafu runtime publikowanej aplikacji.

`Avalonia.Fonts.Inter 12.0.4` występuje wyłącznie jako przechodnia zależność `Avalonia.Headless` w projekcie testowym. Nie jest bezpośrednią zależnością, nie należy do grafu produkcyjnego i nie jest fontem interfejsu Abiturii; aplikacja używa paczkowanego fontu Mulish.

| Pakiet | Wersja | Zakres | Typ | Licencja |
| --- | --- | --- | --- | --- |
| `Avalonia` | `12.0.4` | produkcyjna i testowa | bezpośrednia | MIT |
| `Avalonia.Angle.Windows.Natives` | `2.1.27548.20260419` | produkcyjna i testowa | przechodnia | BSD-3-Clause (LICENSE w pakiecie) |
| `Avalonia.BuildServices` | `11.3.2` | produkcyjna i testowa | przechodnia | MIT |
| `Avalonia.Desktop` | `12.0.4` | produkcyjna i testowa | bezpośrednia | MIT |
| `Avalonia.Fonts.Inter` | `12.0.4` | testowa | przechodnia | MIT |
| `Avalonia.FreeDesktop` | `12.0.4` | produkcyjna i testowa | przechodnia | MIT |
| `Avalonia.FreeDesktop.AtSpi` | `12.0.4` | produkcyjna i testowa | przechodnia | MIT |
| `Avalonia.HarfBuzz` | `12.0.4` | produkcyjna i testowa | przechodnia | MIT |
| `Avalonia.Headless` | `12.0.4` | testowa | przechodnia | MIT |
| `Avalonia.Headless.XUnit` | `12.0.4` | testowa | bezpośrednia | MIT |
| `Avalonia.Native` | `12.0.4` | produkcyjna i testowa | przechodnia | MIT |
| `Avalonia.Remote.Protocol` | `12.0.4` | produkcyjna i testowa | przechodnia | MIT |
| `Avalonia.Skia` | `12.0.4` | produkcyjna i testowa | przechodnia | MIT |
| `Avalonia.Themes.Fluent` | `12.0.4` | produkcyjna i testowa | bezpośrednia | MIT |
| `Avalonia.Win32` | `12.0.4` | produkcyjna i testowa | przechodnia | MIT |
| `Avalonia.X11` | `12.0.4` | produkcyjna i testowa | przechodnia | MIT |
| `CommunityToolkit.Mvvm` | `8.4.2` | produkcyjna i testowa | bezpośrednia | MIT |
| `coverlet.collector` | `6.0.4` | testowa | bezpośrednia | MIT |
| `HarfBuzzSharp` | `8.3.1.3` | produkcyjna i testowa | przechodnia | MIT |
| `HarfBuzzSharp.NativeAssets.Linux` | `8.3.1.3` | produkcyjna i testowa | przechodnia | MIT |
| `HarfBuzzSharp.NativeAssets.macOS` | `8.3.1.3` | produkcyjna i testowa | przechodnia | MIT |
| `HarfBuzzSharp.NativeAssets.WebAssembly` | `8.3.1.3` | produkcyjna i testowa | przechodnia | MIT |
| `HarfBuzzSharp.NativeAssets.Win32` | `8.3.1.3` | produkcyjna i testowa | przechodnia | MIT |
| `MicroCom.Runtime` | `0.11.4` | produkcyjna i testowa | przechodnia | MIT |
| `Microsoft.ApplicationInsights` | `2.23.0` | testowa | przechodnia | MIT |
| `Microsoft.Bcl.AsyncInterfaces` | `6.0.0` | testowa | przechodnia | MIT |
| `Microsoft.CodeCoverage` | `17.14.1` | testowa | przechodnia | MIT |
| `Microsoft.Data.Sqlite.Core` | `10.0.10` | produkcyjna i testowa | przechodnia | MIT |
| `Microsoft.EntityFrameworkCore` | `10.0.10` | produkcyjna i testowa | przechodnia | MIT |
| `Microsoft.EntityFrameworkCore.Abstractions` | `10.0.10` | produkcyjna i testowa | przechodnia | MIT |
| `Microsoft.EntityFrameworkCore.Analyzers` | `10.0.10` | produkcyjna i testowa | przechodnia | MIT |
| `Microsoft.EntityFrameworkCore.Relational` | `10.0.10` | produkcyjna i testowa | przechodnia | MIT |
| `Microsoft.EntityFrameworkCore.Sqlite` | `10.0.10` | produkcyjna i testowa | bezpośrednia | MIT |
| `Microsoft.EntityFrameworkCore.Sqlite.Core` | `10.0.10` | produkcyjna i testowa | przechodnia | MIT |
| `Microsoft.Extensions.Caching.Abstractions` | `10.0.10` | produkcyjna i testowa | przechodnia | MIT |
| `Microsoft.Extensions.Caching.Memory` | `10.0.10` | produkcyjna i testowa | przechodnia | MIT |
| `Microsoft.Extensions.Configuration.Abstractions` | `10.0.10` | produkcyjna i testowa | przechodnia | MIT |
| `Microsoft.Extensions.DependencyInjection` | `10.0.10` | produkcyjna i testowa | bezpośrednia | MIT |
| `Microsoft.Extensions.DependencyInjection.Abstractions` | `10.0.10` | produkcyjna i testowa | przechodnia | MIT |
| `Microsoft.Extensions.DependencyModel` | `10.0.10` | produkcyjna i testowa | przechodnia | MIT |
| `Microsoft.Extensions.Logging` | `10.0.10` | produkcyjna i testowa | przechodnia | MIT |
| `Microsoft.Extensions.Logging.Abstractions` | `10.0.10` | produkcyjna i testowa | przechodnia | MIT |
| `Microsoft.Extensions.Options` | `10.0.10` | produkcyjna i testowa | przechodnia | MIT |
| `Microsoft.Extensions.Primitives` | `10.0.10` | produkcyjna i testowa | przechodnia | MIT |
| `Microsoft.NET.Test.Sdk` | `17.14.1` | testowa | bezpośrednia | MIT |
| `Microsoft.Testing.Extensions.Telemetry` | `1.9.1` | testowa | przechodnia | MIT |
| `Microsoft.Testing.Extensions.TrxReport.Abstractions` | `1.9.1` | testowa | przechodnia | MIT |
| `Microsoft.Testing.Platform` | `1.9.1` | testowa | przechodnia | MIT |
| `Microsoft.Testing.Platform.MSBuild` | `1.9.1` | testowa | przechodnia | MIT |
| `Microsoft.TestPlatform.ObjectModel` | `17.14.1` | testowa | przechodnia | MIT |
| `Microsoft.TestPlatform.TestHost` | `17.14.1` | testowa | przechodnia | MIT |
| `Microsoft.Win32.Registry` | `5.0.0` | testowa | przechodnia | MIT |
| `Newtonsoft.Json` | `13.0.3` | testowa | przechodnia | MIT |
| `SkiaSharp` | `3.119.4` | produkcyjna i testowa | przechodnia | MIT |
| `SkiaSharp.NativeAssets.Linux` | `3.119.4` | produkcyjna i testowa | przechodnia | MIT |
| `SkiaSharp.NativeAssets.macOS` | `3.119.4` | produkcyjna i testowa | przechodnia | MIT |
| `SkiaSharp.NativeAssets.WebAssembly` | `3.119.4` | produkcyjna i testowa | przechodnia | MIT |
| `SkiaSharp.NativeAssets.Win32` | `3.119.4` | produkcyjna i testowa | przechodnia | MIT |
| `SQLitePCLRaw.bundle_e_sqlite3` | `2.1.12` | produkcyjna i testowa | bezpośrednia | Apache-2.0 |
| `SQLitePCLRaw.core` | `2.1.12` | produkcyjna i testowa | przechodnia | Apache-2.0 |
| `SQLitePCLRaw.lib.e_sqlite3` | `2.1.12` | produkcyjna i testowa | przechodnia | Apache-2.0 |
| `SQLitePCLRaw.provider.e_sqlite3` | `2.1.12` | produkcyjna i testowa | przechodnia | Apache-2.0 |
| `Sylinko.CSharpMath.Avalonia` | `12.0.0` | produkcyjna i testowa | bezpośrednia | MIT |
| `Tmds.DBus.Protocol` | `0.92.0` | produkcyjna i testowa | przechodnia | MIT |
| `xunit.analyzers` | `1.27.0` | testowa | przechodnia | Apache-2.0 |
| `xunit.runner.visualstudio` | `3.1.5` | testowa | bezpośrednia | Apache-2.0 |
| `xunit.v3` | `3.2.2` | testowa | bezpośrednia | Apache-2.0 |
| `xunit.v3.assert` | `3.2.2` | testowa | przechodnia | Apache-2.0 |
| `xunit.v3.common` | `3.2.2` | testowa | przechodnia | Apache-2.0 |
| `xunit.v3.core.mtp-v1` | `3.2.2` | testowa | przechodnia | Apache-2.0 |
| `xunit.v3.extensibility.core` | `3.2.2` | testowa | przechodnia | Apache-2.0 |
| `xunit.v3.mtp-v1` | `3.2.2` | testowa | przechodnia | Apache-2.0 |
| `xunit.v3.runner.common` | `3.2.2` | testowa | przechodnia | Apache-2.0 |
| `xunit.v3.runner.inproc.console` | `3.2.2` | testowa | przechodnia | Apache-2.0 |

## Środowisko dołączane do paczek self-contained

Każda paczka produkcyjna zawiera `Microsoft.NETCore.App.Runtime.<rid>` oraz kod apphost z `Microsoft.NETCore.App.Host.<rid>` w wersji `10.0.10`. Konkretny RID i dokładny zbiór komponentów są wyprowadzane z opublikowanego `Abituria.deps.json` i rejestrowane w osobnym SBOM SPDX 2.2.

## Weryfikacja

```powershell
dotnet restore Abituria.sln --configfile NuGet.Config --locked-mode
dotnet list Abituria.sln package --vulnerable --include-transitive
pwsh -NoProfile -File tools/Generate-DependencyDocumentation.ps1 -Verify
```
