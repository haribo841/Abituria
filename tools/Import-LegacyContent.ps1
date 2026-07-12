param(
    [string]$SourceRoot = '',
    [string]$OutputRoot = (Join-Path $PSScriptRoot '..\Content'),
    [switch]$CuratedChaptersOnly,
    [string]$VectorChapterPath = ''
)

Set-StrictMode -Version Latest
$ErrorActionPreference = 'Stop'

if (-not $CuratedChaptersOnly -and
    ([string]::IsNullOrWhiteSpace($SourceRoot) -or -not (Test-Path -LiteralPath $SourceRoot -PathType Container))) {
    throw "Nie znaleziono katalogu źródłowego: $SourceRoot"
}

$issue35SeedPath = Join-Path $PSScriptRoot 'seeds\issue-35-content.json'
$issue35Seed = Get-Content -Raw -Encoding UTF8 $issue35SeedPath | ConvertFrom-Json

$formulaTitles = @(
    'Wartość bezwzględna liczby', 'Potęgi i pierwiastki', 'Logarytmy',
    'Silnia i współczynnik dwumianowy', 'Wzór dwumianowy Newtona',
    'Wzory skróconego mnożenia', 'Ciągi', 'Funkcja kwadratowa',
    'Geometria analityczna', 'Planimetria', 'Stereometria', 'Trygonometria',
    'Kombinatoryka', 'Rachunek prawdopodobieństwa',
    'Parametry danych statystycznych', 'Granica ciągu', 'Pochodna funkcji',
    'Tablica wartości funkcji trygonometrycznych'
)

$questionPaperUrl = 'https://arkusze.pl/maturalne/matematyka-2021-sierpien-poprawkowa-podstawowa.pdf'
$answerKeyUrl = 'https://arkusze.pl/maturalne/matematyka-2021-sierpien-poprawkowa-podstawowa-odpowiedzi.pdf'
$verificationSource = 'CKE EMAP-P0-100-2108'
$answerKey = @(4, 3, 1, 1, 4, 3, 2, 3, 1, 3, 4, 4, 3, 2, 3, 2, 4, 1, 4, 2, 4, 1, 3, 3, 1, 2, 3, 2)
$topicDefinitions = @(
    [ordered]@{ id = 'powers'; title = 'Potęgi'; exerciseNumbers = @(1) }
    [ordered]@{ id = 'logarithms'; title = 'Logarytmy'; exerciseNumbers = @(2) }
    [ordered]@{ id = 'percentages'; title = 'Procenty'; exerciseNumbers = @(3) }
    [ordered]@{ id = 'identities'; title = 'Wzory skróconego mnożenia'; exerciseNumbers = @(4) }
    [ordered]@{ id = 'equations'; title = 'Równania'; exerciseNumbers = @(5, 30) }
    [ordered]@{ id = 'inequalities'; title = 'Nierówności'; exerciseNumbers = @(6, 29) }
    [ordered]@{ id = 'linear-function'; title = 'Funkcja liniowa'; exerciseNumbers = @(7, 8, 9) }
    [ordered]@{ id = 'quadratic-function'; title = 'Funkcja kwadratowa'; exerciseNumbers = @(10, 11, 12) }
    [ordered]@{ id = 'sequences'; title = 'Ciągi'; exerciseNumbers = @(13, 14, 15, 35) }
    [ordered]@{ id = 'trigonometry'; title = 'Trygonometria'; exerciseNumbers = @(16) }
    [ordered]@{ id = 'plane-geometry'; title = 'Planimetria'; exerciseNumbers = @(17, 18, 19, 20, 21, 23, 32, 33) }
    [ordered]@{ id = 'lines-and-segments'; title = 'Proste i odcinki'; exerciseNumbers = @(22) }
    [ordered]@{ id = 'solid-geometry'; title = 'Stereometria'; exerciseNumbers = @(24, 25) }
    [ordered]@{ id = 'combinatorics'; title = 'Kombinatoryka'; exerciseNumbers = @(26) }
    [ordered]@{ id = 'probability'; title = 'Prawdopodobieństwo'; exerciseNumbers = @(27, 34) }
    [ordered]@{ id = 'statistics'; title = 'Statystyka'; exerciseNumbers = @(28) }
    [ordered]@{ id = 'proofs'; title = 'Zadania dowodowe'; exerciseNumbers = @(31) }
)
$topicByNumber = @{}
foreach ($topic in $topicDefinitions) {
    foreach ($number in $topic.exerciseNumbers) { $topicByNumber[$number] = $topic.id }
}
$sourcePageByNumber = @{}
foreach ($number in 1..5) { $sourcePageByNumber[$number] = 2 }
foreach ($number in 6..9) { $sourcePageByNumber[$number] = 4 }
foreach ($number in 10..13) { $sourcePageByNumber[$number] = 6 }
foreach ($number in 14..17) { $sourcePageByNumber[$number] = 8 }
foreach ($number in 18..19) { $sourcePageByNumber[$number] = 10 }
foreach ($number in 20..23) { $sourcePageByNumber[$number] = 12 }
foreach ($number in 24..28) { $sourcePageByNumber[$number] = 14 }
foreach ($number in 29..35) { $sourcePageByNumber[$number] = $number - 13 }

function Read-XamlDocument([string]$Path) {
    $document = [System.Xml.XmlDocument]::new()
    $document.PreserveWhitespace = $true
    $document.Load($Path)
    return $document
}

function Get-AttributeValue([System.Xml.XmlNode]$Node, [string]$LocalName) {
    foreach ($attribute in $Node.Attributes) {
        if ($attribute.LocalName -eq $LocalName -or $attribute.Name -eq $LocalName) {
            return $attribute.Value
        }
    }
    return $null
}

function Repair-LegacyText([string]$Value) {
    if ([string]::IsNullOrWhiteSpace($Value)) { return $Value }
    $value = $Value.Replace('funkcji kwadratowe ', 'funkcji kwadratowej ')
    $value = $value.Replace('tójkąt', 'trójkąt').Replace('tójkąta', 'trójkąta')
    $value = $value.Replace('będzia', 'będzie').Replace('na lewa stronę', 'na lewą stronę')
    $value = $value.Replace('równiania', 'równania').Replace('B=(x_1,y_1)', 'B=(x_2,y_2)')
    $value = $value.Replace('Prosta Prosta', 'Prosta').Replace('rosnących ciągiem', 'rosnącym ciągiem')
    $value = $value.Replace('podsatwie', 'podstawie').Replace('wspołrzędna', 'współrzędna')
    $value = $value.Replace('pamietając', 'pamiętając').Replace('szcześcianu', 'sześcianu')
    $value = $value.Replace('kawadratowej', 'kwadratowej').Replace('wobec czegokąty', 'wobec czego kąty')
    $value = $value.Replace('Stosunek ku białych', 'Stosunek kul białych')
    $value = $value.Replace('wydarzenie wylosowania', 'zdarzenie wylosowania')
    $value = $value.Replace('współczynnik kierunkowy a jest dodatni', 'współczynnik wiodący a jest dodatni')
    $value = $value.Replace('co znalazła się na i nad osią', 'co znajduje się na osi i nad nią')
    return $value.Replace('Współczynnik prostej k jest zatem równy', 'Współczynnik prostej prostopadłej do k jest zatem równy')
}

function Repair-LegacyLatex([string]$Value) {
    $value = Repair-LegacyText $Value
    $value = $value.Replace('/cdot', '\cdot').Replace('/text', '\text')
    $value = $value.Replace('\tg', '\operatorname{tg}')
    $value = $value.Replace('\gt', '>').Replace('\lt', '<')
    $value = $value.Replace('\left\[', '\left[').Replace('\right\]', '\right]')
    $value = $value.Replace('1=-\frac{\Delta}{4a}', 'q=-\frac{\Delta}{4a}')
    $value = $value.Replace('x_1+x_1=', 'x_1+x_2=').Replace('x_1\cdot{x_1}=', 'x_1\cdot{x_2}=')
    $value = $value.Replace('(x^2+2)(x+)=0', '(x^2+2)(x+2)=0').Replace('(a-2b^)2', '(a-2b)^2')
    $cases = [regex]::Match($value, '\\cases\{(?<body>.*)\}$')
    if ($cases.Success) {
        $body = $cases.Groups['body'].Value.Replace('\cr', '\\')
        $value = $value.Substring(0, $cases.Index) + '\begin{cases}' + $body + '\end{cases}'
    }
    $value = [regex]::Replace($value, '\\([A-Za-z]+)(?==)', '\$1{}')
    $value = [regex]::Replace($value, '(\d+)\^\s*\\cdot', '$1^{\circ}')
    return $value.Trim()
}

function ConvertTo-SingleLineMath([string]$Value) {
    $singleLine = [regex]::Replace($Value, '[ \t]*(?:\r\n|\r|\n)[ \t]*', ' ')
    return "\($singleLine\)"
}

function Add-XamlTokens([System.Xml.XmlNode]$Node, [System.Collections.Generic.List[object]]$Tokens) {
    if ($Node.NodeType -in @([System.Xml.XmlNodeType]::Text, [System.Xml.XmlNodeType]::CDATA)) {
        $value = [regex]::Replace($Node.Value, '\s+', ' ')
        if (-not [string]::IsNullOrWhiteSpace($value)) {
            $Tokens.Add([pscustomobject]@{ Kind = 'Text'; Value = (Repair-LegacyText $value) })
        }
        return
    }
    if ($Node.NodeType -ne [System.Xml.XmlNodeType]::Element) { return }

    switch ($Node.LocalName) {
        'FormulaControl' {
            $formula = Get-AttributeValue $Node 'Formula'
            if (-not [string]::IsNullOrWhiteSpace($formula)) {
                $Tokens.Add([pscustomobject]@{ Kind = 'Math'; Value = (Repair-LegacyLatex $formula) })
            }
            return
        }
        'LineBreak' { $Tokens.Add([pscustomobject]@{ Kind = 'Break'; Value = '' }); return }
        'CheckBox' {
            $Tokens.Add([pscustomobject]@{ Kind = 'Option'; Value = (Get-AttributeValue $Node 'Name') })
            return
        }
        'Image' {
            $source = Get-AttributeValue $Node 'Source'
            if (-not [string]::IsNullOrWhiteSpace($source)) {
                $Tokens.Add([pscustomobject]@{ Kind = 'Image'; Value = $source.Replace('\', '/').TrimStart('/') })
            }
            return
        }
        'Run' {
            $text = Get-AttributeValue $Node 'Text'
            if (-not [string]::IsNullOrWhiteSpace($text)) {
                $Tokens.Add([pscustomobject]@{ Kind = 'Text'; Value = (Repair-LegacyText $text) })
            }
        }
    }
    foreach ($child in $Node.ChildNodes) { Add-XamlTokens $child $Tokens }
}

function Convert-TokensToText([object[]]$Tokens) {
    $builder = [System.Text.StringBuilder]::new()
    foreach ($token in $Tokens) {
        switch ($token.Kind) {
            'Text' { [void]$builder.Append($token.Value) }
            'Math' { [void]$builder.Append("\($($token.Value)\)") }
            'Break' { [void]$builder.AppendLine() }
        }
    }
    $lines = $builder.ToString() -split '\r?\n' | ForEach-Object { $_.Trim() }
    $result = ($lines -join "`n")
    $result = [regex]::Replace($result, '[ ]{2,}', ' ')
    $result = [regex]::Replace($result, '(\r?\n){3,}', "`n`n")
    return (Repair-LegacyText $result.Trim())
}

function Convert-TokensToBlocks([object[]]$Tokens) {
    $blocks = [System.Collections.Generic.List[object]]::new()
    $buffer = [System.Collections.Generic.List[object]]::new()
    foreach ($token in $Tokens) {
        if ($token.Kind -eq 'Image') {
            $text = Convert-TokensToText $buffer.ToArray()
            if (-not [string]::IsNullOrWhiteSpace($text)) {
                $blocks.Add([ordered]@{ type = 'richText'; text = $text })
            }
            $buffer.Clear()
            $blocks.Add([ordered]@{ type = 'image'; asset = $token.Value })
        } elseif ($token.Kind -ne 'Option') {
            $buffer.Add($token)
        }
    }
    $remaining = Convert-TokensToText $buffer.ToArray()
    if (-not [string]::IsNullOrWhiteSpace($remaining)) {
        $blocks.Add([ordered]@{ type = 'richText'; text = $remaining })
    }
    return $blocks.ToArray()
}

function Get-FormulaArticles {
    $articles = [System.Collections.Generic.List[object]]::new()
    for ($index = 1; $index -le 18; $index++) {
        $fileName = if ($index -eq 1) { 'WPage.xaml' } else { "W${index}Page.xaml" }
        $path = Join-Path $SourceRoot "pages\equations\$fileName"
        $document = Read-XamlDocument $path
        $roots = $document.SelectNodes("//*[local-name()='Border' and @Grid.Column='2' and @Grid.Row='1']")
        if ($roots.Count -eq 0) { throw "Nie znaleziono treści tablicy w $path" }
        $tokens = [System.Collections.Generic.List[object]]::new()
        Add-XamlTokens $roots[$roots.Count - 1] $tokens
        $articles.Add([ordered]@{
            id = "formula-$index"; order = $index; title = $formulaTitles[$index - 1]
            blocks = @(Convert-TokensToBlocks $tokens.ToArray())
        })
    }
    return $articles.ToArray()
}

function Get-VectorChapter {
    $path = Join-Path $SourceRoot 'pages\chapters\WektoryPage.xaml'
    $document = Read-XamlDocument $path
    $roots = $document.SelectNodes("//*[local-name()='TextBlock' and @Grid.Row='1' and @Grid.Column='1']")
    if ($roots.Count -eq 0) { throw "Nie znaleziono treści działu Wektory w $path" }
    $root = $roots[$roots.Count - 1]
    $tokens = [System.Collections.Generic.List[object]]::new()
    Add-XamlTokens $root $tokens
    return [ordered]@{
        id = 'vectors'; title = 'Wektory'; status = 'available'
        blocks = @(Convert-TokensToBlocks $tokens.ToArray())
    }
}

function Get-Hints([string]$Code) {
    $arrayMatch = [regex]::Match($Code, 'string\[\]\s+hintsArray\s*=\s*\{(?<body>.*?)\};', [System.Text.RegularExpressions.RegexOptions]::Singleline)
    if (-not $arrayMatch.Success) { return @() }
    $hints = [System.Collections.Generic.List[string]]::new()
    $hintMatches = [regex]::Matches($arrayMatch.Groups['body'].Value, '@"(?<value>(?:""|[^"])*)"', [System.Text.RegularExpressions.RegexOptions]::Singleline)
    foreach ($match in $hintMatches) {
        $value = $match.Groups['value'].Value.Replace('""', '"')
        $hints.Add((ConvertTo-SingleLineMath (Repair-LegacyLatex $value)))
    }
    return $hints.ToArray()
}

function Get-RevealedAnswer([string]$Code) {
    $pattern = 'this\.hint(?:Field|Answer)\.(?:Formula|Text)\s*=\s*@?"(?<value>(?:""|[^"])*)"'
    foreach ($match in [regex]::Matches($Code, $pattern, [System.Text.RegularExpressions.RegexOptions]::Singleline)) {
        $value = $match.Groups['value'].Value.Replace('""', '"').Trim()
        if ([string]::IsNullOrWhiteSpace($value)) { continue }
        $value = Repair-LegacyLatex $value
        if ($value.Contains('\')) {
            return ConvertTo-SingleLineMath $value
        }
        return $value
    }
    return $null
}

function Add-ExerciseOption([object]$OptionTokens, [object]$Options) {
    $optionText = Convert-TokensToText $OptionTokens.ToArray()
    $Options.Add(([regex]::Replace($optionText, '^\s*[A-D]\.\s*', '')).Trim())
    $OptionTokens.Clear()
}

function Get-ExerciseTokenParts([object[]]$Tokens) {
    $promptTokens = [System.Collections.Generic.List[object]]::new()
    $optionTokens = [System.Collections.Generic.List[object]]::new()
    $options = [System.Collections.Generic.List[string]]::new()
    $assets = [System.Collections.Generic.List[string]]::new()
    $insideOption = $false

    foreach ($token in $Tokens) {
        switch ($token.Kind) {
            'Image' {
                if (-not $assets.Contains($token.Value)) { $assets.Add($token.Value) }
            }
            'Option' {
                if ($insideOption) { Add-ExerciseOption $optionTokens $options }
                $insideOption = $true
            }
            default {
                if ($insideOption) { $optionTokens.Add($token) } else { $promptTokens.Add($token) }
            }
        }
    }
    if ($insideOption) { Add-ExerciseOption $optionTokens $options }

    return [pscustomobject]@{
        PromptTokens = $promptTokens.ToArray()
        Options = $options.ToArray()
        Assets = $assets.ToArray()
    }
}

function Get-Exercises {
    $exercises = [System.Collections.Generic.List[object]]::new()
    for ($number = 1; $number -le 35; $number++) {
        $xamlPath = Join-Path $SourceRoot "pages\finalexams\Z\Z${number}Page.xaml"
        $code = Get-Content -LiteralPath "$xamlPath.cs" -Raw -Encoding UTF8
        $document = Read-XamlDocument $xamlPath
        $root = $document.SelectSingleNode("//*[local-name()='Border' and @Grid.Row='1' and @Grid.Column='2']//*[local-name()='Viewbox' and @Grid.Column='1' and @Grid.Row='0']")
        if ($null -eq $root) { throw "Nie znaleziono treści zadania w $xamlPath" }

        $tokens = [System.Collections.Generic.List[object]]::new()
        Add-XamlTokens $root $tokens
        $parts = Get-ExerciseTokenParts $tokens.ToArray()

        $answer = if ($number -le 28) { $answerKey[$number - 1] } else { $null }
        $exercise = [ordered]@{
            id = "mp21-z$number"; examId = 'matura-poprawkowa-2021'; number = $number
            title = "Zadanie $number"; topicId = $topicByNumber[$number]
            sourcePage = $sourcePageByNumber[$number]; verificationSource = $verificationSource
            mode = $(if ($number -le 28) { 'multipleChoice' } else { 'revealOnly' })
            prompt = Convert-TokensToText $parts.PromptTokens; options = $parts.Options
            correctOption = $answer; hints = @(Get-Hints $code)
            revealedAnswer = Get-RevealedAnswer $code; assets = $parts.Assets
        }

        # The legacy XAML contains copied answers and hints; keep verified CKE corrections deterministic.
        switch ($number) {
            6 {
                $exercise.hints[0] = '\(\text{Rozwiąż nierówność. Zacznij od pomnożenia obu jej stron przez } 4.\)'
                $exercise.hints[6] = '\(\text{Rozwiązaniem są wszystkie liczby mniejsze lub równe } 7, \text{ czyli } x\in(-\infty,7].\)'
            }
            7 {
                $exercise.options = @(
                    '\(g(x)=-2x+2\)', '\(g(x)=-2x\)', '\(g(x)=-2x+6\)', '\(g(x)=-2x+8\)'
                )
            }
            13 {
                $exercise.hints[0] = '\(\text{Skorzystaj z własności ciągu arytmetycznego.}\)'
            }
            14 {
                $exercise.hints[0] = '\(\text{Dodatnie liczby parzyste mniejsze od 1001 tworzą ciąg: } 2,4,6,\ldots,998,1000.\)'
                $exercise.hints[3] = '\(S_n=\frac{a_1+a_n}{2}\cdot n\)'
            }
            17 {
                $exercise.prompt = 'Czworokąt \(ABCD\) jest wpisany w okrąg o środku \(S\). Bok \(AD\) jest średnicą tego okręgu, a miara kąta \(BDC\) jest równa \(20^{\circ}\) (zobacz rysunek). Wtedy miara kąta \(BSC\) jest równa:'
                $exercise.options = @('\(10^{\circ}\)', '\(20^{\circ}\)', '\(30^{\circ}\)', '\(40^{\circ}\)')
                $exercise.hints = @(
                    '\(\text{Kąt } BDC \text{ jest kątem wpisanym opartym na łuku } BC.\)',
                    '\(\text{Kąt } BSC \text{ jest kątem środkowym opartym na tym samym łuku.}\)',
                    '\(\text{Kąt środkowy ma miarę dwa razy większą od kąta wpisanego opartego na tym samym łuku.}\)',
                    '\(\angle BSC=2\cdot20^{\circ}=40^{\circ}\)'
                )
                $exercise.assets = @('img/mp21z17.png')
            }
            18 {
                $exercise.prompt = 'Okrąg o środku w punkcie \(O\) jest wpisany w trójkąt \(ABC\). Wiadomo, że \(|AB|=|AC|\) i \(\angle BOC=100^{\circ}\) (zobacz rysunek). Miara kąta \(BAC\) jest równa:'
            }
            19 {
                $exercise.prompt = 'Punkty \(A, B, C\) i \(D\) leżą na okręgu o środku w punkcie \(O\). Cięciwy \(DB\) i \(AC\) przecinają się w punkcie \(E\), \(\angle ACB=55^{\circ}\) oraz \(\angle AEB=140^{\circ}\) (zobacz rysunek). Miara kąta \(DAC\) jest równa:'
            }
            22 {
                $exercise.hints[3] = '\(\text{Współczynnik prostej prostopadłej do } k \text{ jest zatem równy } \frac{7}{4}.\)'
            }
            24 {
                $exercise.hints[1] = '\(\text{Następnie oblicz pole całkowite: dwie podstawy oraz sześć kwadratowych ścian bocznych.} \\ P_c=2\cdot6\sqrt{3}+6\cdot2\cdot2=12\sqrt{3}+24\)'
            }
            26 {
                $exercise.hints[2] = '\(\text{Na drugiej, trzeciej i czwartej pozycji można użyć każdej cyfry od 0 do 9, czyli po 10 możliwości.}\)'
            }
            28 {
                $exercise.hints = @(
                    '\(\text{Zapisz równanie wynikające z definicji średniej arytmetycznej pięciu liczb.}\)',
                    '\(\frac{(5x+6)+(6x+7)+(7x+8)+(8x+9)+(9x+10)}{5}=8\)',
                    '\(\frac{35x+40}{5}=8 \quad\Longrightarrow\quad 7x+8=8\)',
                    '\(7x=0 \quad\Longrightarrow\quad x=0\)'
                )
            }
            29 {
                $exercise.hints[4] = '\(\text{Ponieważ współczynnik wiodący } a=1>0, \text{ ramiona paraboli są skierowane w górę. Zaznacz miejsca zerowe } -1 \text{ i } 5.\)'
                $exercise.hints[5] = '\(\text{Nierówność jest spełniona na osi i nad osią } Ox, \text{ więc } x\in(-\infty,-1]\cup[5,+\infty).\)'
                $exercise.revealedAnswer = '\(\text{Odpowiedź: } x\in(-\infty,-1]\cup[5,+\infty)\)'
            }
            31 {
                $exercise.hints[2] = '\(a^2-4ab+4b^2+b^2=(a-2b)^2+b^2\)'
                $exercise.revealedAnswer = '\(b(5b-4a)+a^2=(a-2b)^2+b^2\geq0\), ponieważ suma kwadratów liczb rzeczywistych jest nieujemna.'
            }
            32 {
                $exercise.hints = @(
                    '\(\text{W trójkącie } ADC \text{ kąty mają miary } 90^{\circ},60^{\circ},30^{\circ}.\)',
                    '\(\text{Bok } AD=6 \text{ leży naprzeciw kąta } 30^{\circ}, \text{ więc przeciwprostokątna } CD=12.\)',
                    '\(\angle DCB=\angle ABC=30^{\circ}\), zatem trójkąt \(DBC\) jest równoramienny.',
                    '\(|BD|=|CD|=12\)'
                )
                $exercise.revealedAnswer = '\(|BD|=12\)'
            }
        }

        $exercises.Add($exercise)
    }
    return $exercises.ToArray()
}

function Write-Json([string]$Name, [object]$Value) {
    $path = Join-Path $OutputRoot $Name
    $json = $Value | ConvertTo-Json -Depth 100
    Set-Content -LiteralPath $path -Value $json -Encoding UTF8
}

New-Item -ItemType Directory -Path $OutputRoot -Force | Out-Null
if ($CuratedChaptersOnly) {
    $directorySeparators = [char[]]"\/"
    $defaultOutputRoot = [System.IO.Path]::GetFullPath((Join-Path $PSScriptRoot '..\Content')).TrimEnd($directorySeparators)
    $requestedOutputRoot = [System.IO.Path]::GetFullPath($OutputRoot).TrimEnd($directorySeparators)
    if ([string]::Equals($requestedOutputRoot, $defaultOutputRoot, [StringComparison]::OrdinalIgnoreCase)) {
        throw 'Tryb CuratedChaptersOnly wymaga jawnego katalogu OutputRoot poza aktywnym katalogiem Content.'
    }
    if ([string]::IsNullOrWhiteSpace($VectorChapterPath) -or
        -not (Test-Path -LiteralPath $VectorChapterPath -PathType Leaf)) {
        throw 'Tryb CuratedChaptersOnly wymaga pliku VectorChapterPath z rozdziałem Wektory.'
    }

    $vectorChapter = Get-Content -Raw -Encoding UTF8 $VectorChapterPath | ConvertFrom-Json
    if ($vectorChapter.id -ne 'vectors' -or $vectorChapter.status -ne 'available') {
        throw 'Plik VectorChapterPath nie zawiera dostępnego rozdziału vectors.'
    }
    Write-Json 'chapters.json' ([ordered]@{
            schemaVersion = 2
            introduction = @($issue35Seed.chapterIntroduction)
            chapters = @($vectorChapter) + @($issue35Seed.chapters)
        })
    Write-Json 'roadmap.json' ([ordered]@{
            schemaVersion = 1
            introduction = @($issue35Seed.roadmapIntroduction)
            items = @($issue35Seed.roadmapItems)
        })
    Write-Host "Wygenerowano niezależny katalog 9 działów, w tym treści issue #35."
    return
}

$formulas = @(Get-FormulaArticles)
$chapters = @(
    Get-VectorChapter
    @($issue35Seed.chapters)
)
$exam = [ordered]@{
    id = 'matura-poprawkowa-2021'; title = 'Matura poprawkowa 2021'; year = 2021
    session = 'poprawkowa'
    introduction = @(
        [ordered]@{ type = 'richText'; text = 'Matura poprawkowa przysługuje osobom, które przystąpiły do wszystkich egzaminów obowiązkowych i nie zdały dokładnie jednego z nich. Ten historyczny arkusz zawiera 28 zadań zamkniętych i 7 otwartych. Można w nim zdobyć 45 punktów, czas pracy wynosił 170 minut, a próg zdania egzaminu obowiązkowego wynosił 30%.' }
        [ordered]@{ type = 'richText'; text = 'Dane historyczne z 2021 roku: maturę zdało 74,5% zdających, 25,5% jej nie zdało, a 7,8% zdających nie spełniało warunków przystąpienia do poprawki.' }
    )
    topicIntroduction = @(
        [ordered]@{ type = 'richText'; text = 'W tym segmencie możesz rozwiązywać zadania maturalne podzielone ze względu na dziedzinę, a nie arkusz. Wybierz zagadnienie, które właśnie powtarzasz, aby sprawdzić opanowanie materiału.' }
    )
    source = [ordered]@{
        publisher = 'Centralna Komisja Egzaminacyjna'; documentCode = 'EMAP-P0-100-2108'
        examDate = '2021-08-24'; questionPaperUrl = $questionPaperUrl; answerKeyUrl = $answerKeyUrl
        verifiedOn = '2026-06-30'
    }
    topics = $topicDefinitions
    exercises = @(Get-Exercises)
}
$placeholders = @(
    [ordered]@{ id = 'matura-2019'; title = 'Matura 2019'; category = 'exam'; message = 'Arkusz maturalny 2019 nie został jeszcze uzupełniony.'; roadmapId = 'exam-archive'; blocks = @() }
    [ordered]@{ id = 'matura-2020'; title = 'Matura 2020'; category = 'exam'; message = 'Arkusz maturalny 2020 nie został jeszcze uzupełniony.'; roadmapId = 'exam-archive'; blocks = @() }
    [ordered]@{ id = 'matura-2021'; title = 'Matura 2021'; category = 'exam'; message = 'Podstawowy arkusz maturalny 2021 nie został jeszcze uzupełniony.'; roadmapId = 'exam-archive'; blocks = @(
        [ordered]@{ type = 'richText'; text = 'Dane historyczne zapisane w starej wersji aplikacji: w 2021 roku maturę z matematyki zdało 79% absolwentów - 84% absolwentów liceów i 71% absolwentów techników. Średni wynik egzaminu na poziomie podstawowym wyniósł 56% możliwych punktów: 62% w liceach i 47% w technikach. Pomimo zmian związanych z pandemią ogólny wynik był zbliżony do poprzednich lat. Pełny arkusz i zadania nie były zaimplementowane.' }
    ) }
    [ordered]@{ id = 'graph-generator'; title = 'Generator wykresów'; category = 'calculator'; message = 'Generator wykresów był nieaktywnym placeholderem w starszej wersji.'; roadmapId = 'graph-generator'; blocks = @() }
    [ordered]@{ id = 'trigonometric-calculator'; title = 'Kalkulator funkcji trygonometrycznych'; category = 'calculator'; message = 'Kalkulator funkcji trygonometrycznych był nieaktywnym placeholderem w starszej wersji.'; roadmapId = 'trigonometric-calculator'; blocks = @() }
    [ordered]@{ id = 'exercise-set-e'; title = 'Zestaw E1-E35'; category = 'exercise'; message = 'Ekrany E1-E35 w starej wersji były pustymi szablonami i zostały zastąpione wspólnym planem rozwoju zadań.'; roadmapId = 'legacy-empty-exercises'; blocks = @() }
)

$roadmap = @($issue35Seed.roadmapItems)

Write-Json 'formulas.json' ([ordered]@{ schemaVersion = 2; introduction = @(
    [ordered]@{ type = 'richText'; text = 'W czasie matury z matematyki na obu poziomach możesz korzystać z tablic matematycznych przygotowanych przez CKE. Znajdziesz w nich wiele wzorów i informacji pomocnych podczas egzaminu. Przejrzyj wcześniej całe tablice, aby wiedzieć, gdzie szukać potrzebnych treści i ograniczyć stres. Jeżeli zdajesz tylko poziom podstawowy, pamiętaj, że część tablic dotyczy poziomu rozszerzonego; warto rozpoznawać te strony i pomijać je podczas egzaminu.' }
); articles = $formulas })
Write-Json 'chapters.json' ([ordered]@{ schemaVersion = 2; introduction = @($issue35Seed.chapterIntroduction); chapters = $chapters })
Write-Json 'exam-2021-correction.json' ([ordered]@{ schemaVersion = 2; exam = $exam })
Write-Json 'placeholders.json' ([ordered]@{ schemaVersion = 2; items = $placeholders })
Write-Json 'roadmap.json' ([ordered]@{ schemaVersion = 1; introduction = @($issue35Seed.roadmapIntroduction); items = $roadmap })
Write-Host "Wygenerowano: $($formulas.Count) tablic, $($chapters.Count) działów i $($exam.exercises.Count) zadań."
