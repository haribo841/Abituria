param(
    [string]$SourceRoot = 'C:\Users\Adam\source\repos\Abituria\Projekt-Inzynierski-master',
    [string]$OutputRoot = (Join-Path $PSScriptRoot '..\Content')
)

Set-StrictMode -Version Latest
$ErrorActionPreference = 'Stop'

$formulaTitles = @(
    'Wartość bezwzględna liczby', 'Potęgi i pierwiastki', 'Logarytmy',
    'Silnia i współczynnik dwumianowy', 'Wzór dwumianowy Newtona',
    'Wzory skróconego mnożenia', 'Ciągi', 'Funkcja kwadratowa',
    'Geometria analityczna', 'Planimetria', 'Stereometria', 'Trygonometria',
    'Kombinatoryka', 'Rachunek prawdopodobieństwa',
    'Parametry danych statystycznych', 'Granica ciągu', 'Pochodna funkcji',
    'Tablica wartości funkcji trygonometrycznych'
)

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
    return $value.Replace('równiania', 'równania').Replace('B=(x_1,y_1)', 'B=(x_2,y_2)')
}

function Repair-LegacyLatex([string]$Value) {
    $value = Repair-LegacyText $Value
    $value = $value.Replace('/cdot', '\cdot').Replace('/text', '\text')
    $value = $value.Replace('\tg', '\operatorname{tg}')
    $value = $value.Replace('\gt', '>').Replace('\lt', '<')
    $value = $value.Replace('\left\[', '\left[').Replace('\right\]', '\right]')
    $value = $value.Replace('1=-\frac{\Delta}{4a}', 'q=-\frac{\Delta}{4a}')
    $value = $value.Replace('x_1+x_1=', 'x_1+x_2=').Replace('x_1\cdot{x_1}=', 'x_1\cdot{x_2}=')
    $cases = [regex]::Match($value, '\\cases\{(?<body>.*)\}$')
    if ($cases.Success) {
        $body = $cases.Groups['body'].Value.Replace('\cr', '\\')
        $value = $value.Substring(0, $cases.Index) + '\begin{cases}' + $body + '\end{cases}'
    }
    $value = [regex]::Replace($value, '\\([A-Za-z]+)(?==)', '\$1{}')
    $value = [regex]::Replace($value, '(\d+)\^\s*\\cdot', '$1^{\circ}')
    return $value.Trim()
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
    return $result.Trim()
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
    $matches = [regex]::Matches($arrayMatch.Groups['body'].Value, '@"(?<value>(?:""|[^"])*)"', [System.Text.RegularExpressions.RegexOptions]::Singleline)
    foreach ($match in $matches) {
        $value = $match.Groups['value'].Value.Replace('""', '"')
        $hints.Add("\($(Repair-LegacyLatex $value)\)")
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
            return "\($value\)"
        }
        return $value
    }
    return $null
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
        $promptTokens = [System.Collections.Generic.List[object]]::new()
        $optionTokens = [System.Collections.Generic.List[object]]::new()
        $options = [System.Collections.Generic.List[string]]::new()
        $assets = [System.Collections.Generic.List[string]]::new()
        $insideOption = $false
        foreach ($token in $tokens) {
            if ($token.Kind -eq 'Image') {
                if (-not $assets.Contains($token.Value)) { $assets.Add($token.Value) }
                continue
            }
            if ($token.Kind -eq 'Option') {
                if ($insideOption) {
                    $optionText = Convert-TokensToText $optionTokens.ToArray()
                    $options.Add(([regex]::Replace($optionText, '^\s*[A-D]\.\s*', '')).Trim())
                    $optionTokens.Clear()
                }
                $insideOption = $true
                continue
            }
            if ($insideOption) { $optionTokens.Add($token) } else { $promptTokens.Add($token) }
        }
        if ($insideOption) {
            $optionText = Convert-TokensToText $optionTokens.ToArray()
            $options.Add(([regex]::Replace($optionText, '^\s*[A-D]\.\s*', '')).Trim())
        }

        $correctMatch = [regex]::Match($code, 'readonly\s+int\s+correctAnsw\s*=\s*(?<answer>\d+)')
        $answer = if ($correctMatch.Success) { [int]$correctMatch.Groups['answer'].Value } else { $null }
        $exercises.Add([ordered]@{
            id = "mp21-z$number"; examId = 'matura-poprawkowa-2021'; number = $number
            title = "Zadanie $number"; mode = $(if ($number -le 28) { 'multipleChoice' } else { 'revealOnly' })
            prompt = Convert-TokensToText $promptTokens.ToArray(); options = $options.ToArray()
            correctOption = $answer; hints = @(Get-Hints $code)
            revealedAnswer = Get-RevealedAnswer $code; assets = $assets.ToArray()
        })
    }
    return $exercises.ToArray()
}

function Write-Json([string]$Name, [object]$Value) {
    $path = Join-Path $OutputRoot $Name
    $json = $Value | ConvertTo-Json -Depth 100
    Set-Content -LiteralPath $path -Value $json -Encoding UTF8
}

New-Item -ItemType Directory -Path $OutputRoot -Force | Out-Null
$formulas = @(Get-FormulaArticles)
$chapters = @(
    Get-VectorChapter
    [ordered]@{ id = 'chapter-1'; title = 'Dział I'; status = 'placeholder'; message = 'Treść ogólna działu I'; blocks = @() }
    [ordered]@{ id = 'chapter-2'; title = 'Dział II'; status = 'placeholder'; message = 'Treść ogólna działu II'; blocks = @() }
    [ordered]@{ id = 'chapter-3'; title = 'Dział III'; status = 'placeholder'; message = 'Treść ogólna działu III'; blocks = @() }
)
$exam = [ordered]@{
    id = 'matura-poprawkowa-2021'; title = 'Matura poprawkowa 2021'; year = 2021
    session = 'poprawkowa'; exercises = @(Get-Exercises)
}
$placeholders = @(
    [ordered]@{ id = 'matura-2019'; title = 'Matura 2019'; category = 'exam'; message = 'Arkusz maturalny 2019 nie został jeszcze uzupełniony.' }
    [ordered]@{ id = 'matura-2020'; title = 'Matura 2020'; category = 'exam'; message = 'Arkusz maturalny 2020 nie został jeszcze uzupełniony.' }
    [ordered]@{ id = 'matura-2021'; title = 'Matura 2021'; category = 'exam'; message = 'Podstawowy arkusz maturalny 2021 nie został jeszcze uzupełniony.' }
    [ordered]@{ id = 'general-calculator'; title = 'Kalkulator ogólny'; category = 'calculator'; message = 'Kalkulator ogólny jest w przygotowaniu.' }
    [ordered]@{ id = 'exercise-set-e'; title = 'Zestaw E1-E35'; category = 'exercise'; message = 'Ekrany E1-E35 w starej wersji były pustymi szablonami i pozostają oznaczone jako treść w przygotowaniu.' }
)

Write-Json 'formulas.json' ([ordered]@{ schemaVersion = 1; articles = $formulas })
Write-Json 'chapters.json' ([ordered]@{ schemaVersion = 1; chapters = $chapters })
Write-Json 'exam-2021-correction.json' ([ordered]@{ schemaVersion = 1; exam = $exam })
Write-Json 'placeholders.json' ([ordered]@{ schemaVersion = 1; items = $placeholders })
Write-Host "Wygenerowano: $($formulas.Count) tablic, $($chapters.Count) działy i $($exam.exercises.Count) zadań."
