# Historyczne dokumenty projektu

Dokumenty w tym katalogu pochodzą ze starych wersji Abiturii. Nie opisują bieżącej architektury ani gotowych funkcji. Zostały zachowane jako źródło decyzji produktowych i pozycji planu rozwoju.

Katalog `originals` zawiera niezmienione bajtowo kopie pięciu pierwotnych plików `.txt` oraz historycznego pliku licencji ze snapshotu `Projekt-Inzynierski-master`. Ich sumy kontrolne znajdują się w `originals/SHA256SUMS`. Plik licencji służy wyłącznie zachowaniu proweniencji; aktywną licencją repozytorium pozostaje główny `LICENSE`.

| Dokument | Znaczenie dla obecnego projektu |
| --- | --- |
| `analiza-biznesowa.md` | Cele, wymagania, harmonogram i model licencyjny |
| `analiza-biznesowa-pelna.md` | Pełna historyczna transkrypcja analizy przekazanej przy Issue #9; nie jest źródłem prawdy dla aktualnej aplikacji AvaloniaUI |
| `definicja-projektu.md` | Wizja narzędzia edukacyjnego i kalkulatorów |
| `implementacja.md` | Dystrybucja, dokumentacja i informacje o wydaniu |
| `opis-struktury-systemu.md` | Pierwotna architektura WPF i lista planowanych funkcji |
| `tresc-dzialow-matematyki.md` | Szkic przyszłych materiałów działowych |

Wersje Markdown są uporządkowanymi streszczeniami. Przy sprawdzaniu dokładnego brzmienia należy używać kopii z `originals`.

Plik `analiza-biznesowa-pelna.md` jest odrębną historyczną transkrypcją przekazaną przy bieżącym Issue #9, a nie kopią bajtową pliku z dawnych snapshotów. Jest jedynym dokumentem z tego katalogu celowo dostępnym przez DocFX, ponieważ aktywna analiza prowadzi do niego odnośnikiem i wyraźnie rozdziela stan historyczny od bieżącej implementacji. Pozostały katalog `legacy`, w tym `originals`, pozostaje wyłączony z publikacji DocFX.

Aktywny stan prac opisują [plan rozwoju](../ROADMAP.md) i [inwentarz migracji](../MIGRATION_INVENTORY.md).
