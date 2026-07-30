param(
    [string]$ContentRoot = (Join-Path $PSScriptRoot '..\Content'),
    [string]$DocumentationPath = (Join-Path $PSScriptRoot '..\docs\MATH_COURSE_2023_COVERAGE.md'),
    [string]$LegacyCatalogPath = ''
)

Set-StrictMode -Version Latest
$ErrorActionPreference = 'Stop'

$seedRoot = Join-Path $PSScriptRoot 'seeds\math-course'
$catalogSeedPath = Join-Path $seedRoot 'catalog.json'
$issue35SeedPath = Join-Path $PSScriptRoot 'seeds\issue-35-content.json'
$canonicalCatalogPath = Join-Path $PSScriptRoot '..\Content\chapters.json'
$chaptersPath = Join-Path $ContentRoot 'chapters.json'
$exercisesPath = Join-Path $ContentRoot 'course-exercises.json'
$author = 'Adam Kubiś'
$script:learningScenariosById = @{}

function Read-Json([string]$Path) {
    if (-not (Test-Path -LiteralPath $Path -PathType Leaf)) {
        throw "Nie znaleziono pliku źródłowego: $Path"
    }

    return Get-Content -Raw -Encoding UTF8 -LiteralPath $Path | ConvertFrom-Json
}

function Write-Json([string]$Path, [object]$Value) {
    $parent = Split-Path -Parent $Path
    New-Item -ItemType Directory -Path $parent -Force | Out-Null
    $json = $Value | ConvertTo-Json -Depth 100
    [System.IO.File]::WriteAllText(
        [System.IO.Path]::GetFullPath($Path),
        $json + [Environment]::NewLine,
        [System.Text.UTF8Encoding]::new($false))
}

function New-LearningScenario([object]$Requirement) {
    $requirementId = [string]$Requirement.id
    if (-not $script:learningScenariosById.ContainsKey($requirementId)) {
        throw "Brak autorskiego scenariusza dla wymagania $requirementId."
    }

    return $script:learningScenariosById[$requirementId]
}

function Get-TheoryText([string]$AreaId) {
    switch ($AreaId) {
        'area-i' { return 'Liczby rzeczywiste łączą rachunek, porządek i interpretację na osi. Przed obliczeniem sprawdź dziedzinę, a wynik dokładny przybliżaj dopiero na końcu.' }
        'area-ii' { return 'Przekształcenia algebraiczne są poprawne wtedy, gdy zachowują wartość wyrażenia w jego dziedzinie. Rozpoznawaj wspólny czynnik, wzór i strukturę wielomianu.' }
        'area-iii' { return 'Rozwiązaniem równania lub nierówności jest każda liczba z dziedziny spełniająca warunek. Zapisuj operacje równoważne i kontroluj wartości wykluczone.' }
        'area-iv' { return 'Układ równań opisuje warunki spełniane jednocześnie. Metody podstawiania i eliminacji prowadzą do tej samej pary, jeśli układ jest oznaczony.' }
        'area-v' { return 'Funkcję analizuj przez dziedzinę, wartości, miejsca zerowe i monotoniczność. Wybór wzoru, tabeli lub wykresu zależy od informacji potrzebnej w zadaniu.' }
        'area-vi' { return 'Ciąg jest funkcją określoną na liczbach naturalnych. Zanim użyjesz wzoru na wyraz lub sumę, rozpoznaj sposób określenia i typ ciągu.' }
        'area-vii' { return 'Funkcje trygonometryczne wiążą miary kątów z proporcjami boków. Rysunek pomocniczy pozwala dobrać definicję, twierdzenie sinusów lub cosinusów.' }
        'area-viii' { return 'W geometrii płaskiej zapisuj dane na własnym rysunku i uzasadniaj każdą zależność twierdzeniem albo definicją. Sama zgodność z rysunkiem nie jest dowodem.' }
        'area-ix' { return 'Geometria analityczna tłumaczy punkty, proste, okręgi i wektory na równania. Wynik algebraiczny warto sprawdzić przez jego znaczenie geometryczne.' }
        'area-x' { return 'W stereometrii kluczowe jest wskazanie właściwego przekroju i trójkąta pomocniczego. Nie zakładaj prostopadłości, jeśli nie wynika ona z danych lub twierdzenia.' }
        'area-xi' { return 'W zliczaniu rozdziel sytuacje rozłączne i kolejne etapy wyboru. Reguła dodawania łączy przypadki, a reguła mnożenia kolejne decyzje.' }
        'area-xii' { return 'Prawdopodobieństwo wymaga jawnej przestrzeni zdarzeń, a statystyka uporządkowanych danych i poprawnie dobranej miary położenia.' }
        'area-xiii' { return 'Optymalizacja zaczyna się od modelu i dziedziny. Ekstremum funkcji kwadratowej wyznacza wierzchołek, a w zakresie rozszerzonym znak pochodnej.' }
        default { throw "Brak teorii dla obszaru: $AreaId" }
    }
}

function Get-DiagramBlock([string]$LessonId) {
    switch ($LessonId) {
        'trigonometry' {
            return [ordered]@{
                type = 'diagram'
                diagramId = 'course-right-triangle'
            }
        }
        'planimetry' {
            return [ordered]@{
                type = 'diagram'
                diagramId = 'course-circle-angles'
            }
        }
        'analytic-geometry' {
            return [ordered]@{
                type = 'diagram'
                diagramId = 'course-coordinate-vector'
            }
        }
        'stereometry' {
            return [ordered]@{
                type = 'diagram'
                diagramId = 'course-cube-section'
            }
        }
        default { return $null }
    }
}

function Get-ExistingBlocks([string]$Path) {
    $result = @{}
    if (-not (Test-Path -LiteralPath $Path -PathType Leaf)) {
        return $result
    }

    $catalog = Read-Json $Path
    $items = if ([int]$catalog.schemaVersion -ge 3) { @($catalog.lessons) } else { @($catalog.chapters) }
    foreach ($item in $items) {
        $result[[string]$item.id] = @($item.blocks)
    }

    return $result
}

function New-LessonBlocks([object]$Lesson, [hashtable]$ExistingBlocks) {
    if ($Lesson.PSObject.Properties.Name -contains 'blocks' -and @($Lesson.blocks).Count -gt 0) {
        return @($Lesson.blocks)
    }

    if ($ExistingBlocks.ContainsKey([string]$Lesson.id) -and @($ExistingBlocks[[string]$Lesson.id]).Count -gt 0) {
        return @($ExistingBlocks[[string]$Lesson.id])
    }

    $blocks = @(
        [ordered]@{
            type = 'richText'
            text = "Podstawa teorii`n`n$(Get-TheoryText ([string]$Lesson.areaId))`n`nKażde wymaganie w tej lekcji ma dwa rozwiązane przykłady i trzy ćwiczenia z pełnym rozwiązaniem."
        }
    )
    $diagram = Get-DiagramBlock ([string]$Lesson.id)
    if ($null -ne $diagram) {
        $blocks += $diagram
    }

    return $blocks
}

function New-CoverageDocument([object]$Catalog, [object[]]$Requirements, [object[]]$Exercises) {
    $exampleCount = 0
    foreach ($lesson in $Catalog.lessons) {
        if ($null -ne $lesson.workedExamples) {
            $exampleCount += @($lesson.workedExamples).Count
        }
    }
    $builder = [System.Text.StringBuilder]::new()
    [void]$builder.AppendLine('# Pokrycie kursu matematyki - Formuła 2023')
    [void]$builder.AppendLine()
    [void]$builder.AppendLine('Katalog obejmuje dokładne wymagania szczegółowe podstawy programowej z 2024 r., stosowanej na maturze 2026. Przykłady, ćwiczenia, rozwiązania i diagramy są autorskie; informatory CKE służą wyłącznie do kalibracji stylu egzaminu.')
    [void]$builder.AppendLine('Każdy identyfikator wymagania ma osobny scenariusz dydaktyczny w jednym z czterech plików `learning-stage-*.json`; generator odrzuca brak, nadmiar, duplikat lub powtórzone polecenie w pakiecie.')
    [void]$builder.AppendLine()
    [void]$builder.AppendLine('## Przypięte źródła')
    [void]$builder.AppendLine()
    foreach ($source in $Catalog.sources) {
        [void]$builder.AppendLine(('- [{0}]({1})' -f [string]$source.title, [string]$source.documentUrl))
        [void]$builder.AppendLine(('  - wydawca: {0}' -f [string]$source.publisher))
        [void]$builder.AppendLine(('  - SHA-256: `{0}`' -f [string]$source.documentSha256))
        [void]$builder.AppendLine(('  - weryfikacja: {0}' -f [string]$source.verifiedOn))
    }
    [void]$builder.AppendLine()
    [void]$builder.AppendLine('## Audyt liczbowy')
    [void]$builder.AppendLine()
    [void]$builder.AppendLine("- grupy: $(@($Catalog.groups).Count) / 4;")
    [void]$builder.AppendLine("- obszary: $(@($Catalog.areas).Count) / 13;")
    [void]$builder.AppendLine("- wymagania podstawowe: $(@($Requirements | Where-Object level -eq 'basic').Count) / 73;")
    [void]$builder.AppendLine("- wymagania rozszerzone: $(@($Requirements | Where-Object level -eq 'extended').Count) / 46;")
    [void]$builder.AppendLine("- autorskie scenariusze wymagań: $($script:learningScenariosById.Count) / 119;")
    [void]$builder.AppendLine("- rozwiązane przykłady: $exampleCount / 238;")
    [void]$builder.AppendLine("- ćwiczenia: $($Exercises.Count) / 357.")
    [void]$builder.AppendLine()
    [void]$builder.AppendLine('## Macierz obszarów')
    [void]$builder.AppendLine()
    [void]$builder.AppendLine('| Obszar | Podstawowe | Rozszerzone | Przykłady | Ćwiczenia |')
    [void]$builder.AppendLine('| --- | ---: | ---: | ---: | ---: |')
    foreach ($area in @($Catalog.areas | Sort-Object order)) {
        $areaRequirements = @($Requirements | Where-Object areaId -eq $area.id)
        $basic = @($areaRequirements | Where-Object level -eq 'basic').Count
        $extended = @($areaRequirements | Where-Object level -eq 'extended').Count
        [void]$builder.AppendLine("| $($area.officialNumber). $($area.title) | $basic | $extended | $($areaRequirements.Count * 2) | $($areaRequirements.Count * 3) |")
    }
    [void]$builder.AppendLine()
    [void]$builder.AppendLine('## Macierz wymagań')
    [void]$builder.AppendLine()
    [void]$builder.AppendLine('| Id | Poziom | Lekcja | Przykłady | Ćwiczenia |')
    [void]$builder.AppendLine('| --- | --- | --- | --- | --- |')
    foreach ($requirement in $Requirements) {
        [void]$builder.AppendLine("| $($requirement.id) | $($requirement.level) | `$($requirement.lessonId)` | $($requirement.workedExampleIds -join ', ') | $($requirement.exerciseIds -join ', ') |")
    }
    [void]$builder.AppendLine()
    [void]$builder.AppendLine('Pełne brzmienie każdego wymagania oraz jego mapowanie jest zapisane maszynowo w `Content/chapters.json`. Dane ćwiczeń znajdują się w `Content/course-exercises.json`.')
    return $builder.ToString()
}

$catalogSeed = Read-Json $catalogSeedPath
$stageFiles = @(Get-ChildItem -LiteralPath $seedRoot -Filter 'stage-*.json' | Sort-Object Name)
if ($stageFiles.Count -ne 4) {
    throw "Oczekiwano 4 plików etapów, znaleziono $($stageFiles.Count)."
}

$expectedStageCounts = @(35, 33, 43, 8)
$rawRequirements = @()
for ($stageIndex = 0; $stageIndex -lt $stageFiles.Count; $stageIndex++) {
    $stage = Read-Json $stageFiles[$stageIndex].FullName
    $stageRequirements = @($stage.requirements)
    if ($stageRequirements.Count -ne $expectedStageCounts[$stageIndex]) {
        throw "Etap $($stage.stage) ma $($stageRequirements.Count) wymagań zamiast $($expectedStageCounts[$stageIndex])."
    }
    $rawRequirements += $stageRequirements
}

if ($rawRequirements.Count -ne 119 -or
    @($rawRequirements | Where-Object level -eq 'basic').Count -ne 73 -or
    @($rawRequirements | Where-Object level -eq 'extended').Count -ne 46 -or
    @($rawRequirements.id | Sort-Object -Unique).Count -ne 119) {
    throw 'Macierz źródłowa nie spełnia kontraktu 119/73/46 albo zawiera powtórzone identyfikatory.'
}

$learningFiles = @(Get-ChildItem -LiteralPath $seedRoot -Filter 'learning-stage-*.json' | Sort-Object Name)
if ($learningFiles.Count -ne 4) {
    throw "Oczekiwano 4 plików autorskich scenariuszy, znaleziono $($learningFiles.Count)."
}

foreach ($learningFile in $learningFiles) {
    $learningStage = Read-Json $learningFile.FullName
    foreach ($scenario in @($learningStage.scenarios)) {
        $scenarioId = [string]$scenario.requirementId
        if ([string]::IsNullOrWhiteSpace($scenarioId) -or $script:learningScenariosById.ContainsKey($scenarioId)) {
            throw "Pusty albo powtórzony identyfikator scenariusza w $($learningFile.Name): $scenarioId"
        }

        foreach ($field in @(
            'foundationPrompt',
            'foundationSolution',
            'examPrompt',
            'examSolution',
            'numericPrompt',
            'numericSolution',
            'reasoningPrompt',
            'reasoningSolution',
            'method')) {
            if (-not ($scenario.PSObject.Properties.Name -contains $field) -or
                [string]::IsNullOrWhiteSpace([string]$scenario.$field)) {
                throw "Scenariusz $scenarioId nie ma pola $field."
            }
        }

        if (-not ($scenario.PSObject.Properties.Name -contains 'numericValue') -or
            [double]::IsNaN([double]$scenario.numericValue) -or
            [double]::IsInfinity([double]$scenario.numericValue)) {
            throw "Scenariusz $scenarioId nie ma skończonej wartości numericValue."
        }

        $promptCount = @(
            [string]$scenario.foundationPrompt,
            [string]$scenario.examPrompt,
            [string]$scenario.numericPrompt,
            [string]$scenario.reasoningPrompt | Sort-Object -Unique).Count
        if ($promptCount -ne 4) {
            throw "Scenariusz $scenarioId zawiera powtórzone polecenie."
        }

        $script:learningScenariosById[$scenarioId] = $scenario
    }
}

$rawRequirementIds = @($rawRequirements.id | Sort-Object)
$scenarioIds = @($script:learningScenariosById.Keys | Sort-Object)
if ($scenarioIds.Count -ne 119 -or (Compare-Object $rawRequirementIds $scenarioIds)) {
    throw 'Autorskie scenariusze nie odwzorowują dokładnie wszystkich 119 wymagań.'
}

$lessonIds = @($catalogSeed.lessons.id)
foreach ($requirement in $rawRequirements) {
    if ($requirement.lessonId -notin $lessonIds) {
        throw "Wymaganie $($requirement.id) wskazuje nieznaną lekcję $($requirement.lessonId)."
    }
}

if ([string]::IsNullOrWhiteSpace($LegacyCatalogPath)) {
    $LegacyCatalogPath = if (Test-Path -LiteralPath $chaptersPath -PathType Leaf) {
        $chaptersPath
    } else {
        $canonicalCatalogPath
    }
}
$existingBlocks = Get-ExistingBlocks $LegacyCatalogPath
$issue35Seed = Read-Json $issue35SeedPath
foreach ($legacyChapter in @($issue35Seed.chapters | Where-Object { @($_.blocks).Count -gt 0 })) {
    $existingBlocks[[string]$legacyChapter.id] = @($legacyChapter.blocks)
}

$requirements = @()
$workedExamplesByLesson = @{}
$exercisesByLesson = @{}
$exercises = @()
$globalRequirementIndex = 0
foreach ($raw in $rawRequirements) {
    $globalRequirementIndex++
    $slug = "$(([string]$raw.id.Split('.')[0]).ToLowerInvariant())-$(([string]$raw.level)[0])$('{0:D2}' -f [int]$raw.number)"
    $exampleIds = @("example-$slug-foundation", "example-$slug-exam")
    $exerciseIds = @(1..3 | ForEach-Object { "course-$slug-$_" })
    $scenario = New-LearningScenario $raw

    $requirement = [ordered]@{
        id = [string]$raw.id
        areaId = [string]$raw.areaId
        level = [string]$raw.level
        number = [int]$raw.number
        text = [string]$raw.text
        sourceId = 'legal-basis-2024'
        lessonId = [string]$raw.lessonId
        workedExampleIds = $exampleIds
        exerciseIds = $exerciseIds
    }
    $requirements += $requirement

    $lessonId = [string]$raw.lessonId
    if (-not $workedExamplesByLesson.ContainsKey($lessonId)) {
        $workedExamplesByLesson[$lessonId] = @()
        $exercisesByLesson[$lessonId] = @()
    }
    $workedExamplesByLesson[$lessonId] += @(
        [ordered]@{
            id = $exampleIds[0]
            requirementId = [string]$raw.id
            kind = 'foundation'
            title = 'Przykład podstawowy'
            author = $author
            prompt = "$($scenario.foundationPrompt) Wymaganie: $($raw.id)."
            solution = [string]$scenario.foundationSolution
        },
        [ordered]@{
            id = $exampleIds[1]
            requirementId = [string]$raw.id
            kind = 'exam'
            title = 'Przykład egzaminacyjny'
            author = $author
            prompt = "$($scenario.examPrompt) Zapisz wszystkie istotne kroki."
            solution = [string]$scenario.examSolution
        }
    )

    $verificationSource = "Rozporządzenie Dz.U. 2024 poz. 1019, wymaganie $($raw.id)"
    $choice = [ordered]@{
        id = $exerciseIds[0]
        examId = 'math-course-2023'
        number = ($globalRequirementIndex - 1) * 3 + 1
        title = "$($raw.id) - szybkie sprawdzenie"
        author = $author
        topicId = [string]$raw.areaId
        sourcePage = 0
        verificationSource = $verificationSource
        mode = 'multipleChoice'
        prompt = "Który plan najlepiej rozpoczyna rozwiązanie zadania zgodnego z wymaganiem $($raw.id): $($raw.text)"
        options = @(
            [string]$scenario.method,
            'Pominąć dziedzinę i wybrać wynik wyłącznie na podstawie rysunku.',
            'Zaokrąglić wszystkie dane przed rozpoczęciem rozwiązania.',
            'Zastosować dowolny wzór bez sprawdzenia jego założeń.'
        )
        correctOption = 1
        hints = @(
            'Zwróć uwagę na czasownik opisujący oczekiwaną czynność w wymaganiu.',
            "Właściwy plan brzmi: $($scenario.method)"
        )
        revealedAnswer = "Poprawna jest odpowiedź A. $($scenario.method)"
        diagramIds = @()
        requirementId = [string]$raw.id
        level = [string]$raw.level
        absoluteTolerance = 0.000000001
        relativeTolerance = 0.000000001
    }
    $numeric = [ordered]@{
        id = $exerciseIds[1]
        examId = 'math-course-2023'
        number = ($globalRequirementIndex - 1) * 3 + 2
        title = "$($raw.id) - obliczenia"
        author = $author
        topicId = [string]$raw.areaId
        sourcePage = 0
        verificationSource = $verificationSource
        mode = 'numeric'
        prompt = [string]$scenario.numericPrompt
        options = @()
        hints = @(
            [string]$scenario.method,
            'Wpisz sam wynik jako liczbę albo proste wyrażenie; możesz użyć przecinka lub kropki dziesiętnej.'
        )
        revealedAnswer = [string]$scenario.numericSolution
        diagramIds = @()
        requirementId = [string]$raw.id
        level = [string]$raw.level
        expectedValue = [double]$scenario.numericValue
        absoluteTolerance = 0.000000001
        relativeTolerance = 0.000000001
    }
    $reasoning = [ordered]@{
        id = $exerciseIds[2]
        examId = 'math-course-2023'
        number = $globalRequirementIndex * 3
        title = "$($raw.id) - rozumowanie"
        author = $author
        topicId = [string]$raw.areaId
        sourcePage = 0
        verificationSource = $verificationSource
        mode = 'revealOnly'
        prompt = [string]$scenario.reasoningPrompt
        options = @()
        hints = @(
            [string]$scenario.method,
            'Rozdziel rozwiązanie na dane, metodę, obliczenia i kontrolę otrzymanego wyniku.'
        )
        revealedAnswer = [string]$scenario.reasoningSolution
        diagramIds = @()
        requirementId = [string]$raw.id
        level = [string]$raw.level
        absoluteTolerance = 0.000000001
        relativeTolerance = 0.000000001
    }
    $packageExercises = @($choice, $numeric, $reasoning)
    $exercises += $packageExercises
    $exercisesByLesson[$lessonId] += $packageExercises
}

$lessons = @()
foreach ($rawLesson in @($catalogSeed.lessons)) {
    $lessonId = [string]$rawLesson.id
    $lessonRequirements = @($requirements | Where-Object lessonId -eq $lessonId)
    $lessonExamples = @()
    $lessonExercises = @()
    if ($workedExamplesByLesson.ContainsKey($lessonId)) {
        $lessonExamples = @($workedExamplesByLesson[$lessonId])
        $lessonExercises = @($exercisesByLesson[$lessonId])
    }
    $lessons += [ordered]@{
        id = $lessonId
        areaId = [string]$rawLesson.areaId
        order = [int]$rawLesson.order
        title = [string]$rawLesson.title
        level = [string]$rawLesson.level
        alwaysVisible = [bool]($rawLesson.PSObject.Properties.Name -contains 'alwaysVisible' -and $rawLesson.alwaysVisible)
        requirementIds = @($lessonRequirements | ForEach-Object { $_.id })
        blocks = @(New-LessonBlocks $rawLesson $existingBlocks)
        workedExamples = [object[]]$lessonExamples
        exerciseIds = @($lessonExercises | ForEach-Object { $_.id })
    }
}

$areas = @()
foreach ($rawArea in @($catalogSeed.areas)) {
    $areas += [ordered]@{
        id = [string]$rawArea.id
        order = [int]$rawArea.order
        officialNumber = [string]$rawArea.officialNumber
        groupId = [string]$rawArea.groupId
        title = [string]$rawArea.title
        lessonIds = @($lessons | Where-Object areaId -eq $rawArea.id | ForEach-Object { $_.id })
    }
}

$courseCatalog = [ordered]@{
    schemaVersion = 4
    author = $author
    sources = @($catalogSeed.sources)
    introduction = @(
        [ordered]@{
            type = 'richText'
            text = "Kurs matematyki - Formuła 2023`n`nZakres odpowiada podstawie programowej z 2024 r. stosowanej na maturze 2026. Poziom podstawowy obejmuje 73 wymagania i 219 ćwiczeń. Filtr rozszerzony dodaje 46 wymagań i 138 ćwiczeń. Dokładne brzmienie wymagań pochodzi z aktu urzędowego; przykłady, ćwiczenia, rozwiązania i diagramy opracował Adam Kubiś. Zadania z informatorów CKE nie zostały skopiowane."
        }
    )
    groups = @($catalogSeed.groups)
    areas = $areas
    requirements = $requirements
    lessons = $lessons
}
$exerciseCatalog = [ordered]@{
    schemaVersion = 1
    author = $author
    exercises = $exercises
}

$workedExampleCount = 0
foreach ($lesson in $lessons) {
    if ($null -ne $lesson.workedExamples) {
        $workedExampleCount += @($lesson.workedExamples).Count
    }
}
if (@($courseCatalog.groups).Count -ne 4 -or
    @($courseCatalog.areas).Count -ne 13 -or
    $workedExampleCount -ne 238 -or
    $exercises.Count -ne 357 -or
    @($exercises.id | Sort-Object -Unique).Count -ne 357 -or
    @($exercises | Where-Object { $_.id.Length -ge 80 }).Count -ne 0) {
    throw 'Wygenerowany katalog nie spełnia kontraktu 4/13/238/357 lub kontraktu identyfikatorów.'
}

Write-Json $chaptersPath $courseCatalog
Write-Json $exercisesPath $exerciseCatalog

$documentationParent = Split-Path -Parent $DocumentationPath
New-Item -ItemType Directory -Path $documentationParent -Force | Out-Null
$documentation = New-CoverageDocument $courseCatalog $requirements $exercises
[System.IO.File]::WriteAllText(
    [System.IO.Path]::GetFullPath($DocumentationPath),
    $documentation,
    [System.Text.UTF8Encoding]::new($false))

Write-Host "Wygenerowano 4 grupy, 13 obszarów, 119 wymagań, $workedExampleCount przykładów i $($exercises.Count) ćwiczeń."
