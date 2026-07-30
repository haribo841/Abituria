param(
    [string]$OutputPath = (Join-Path $PSScriptRoot '..\Content\diagrams.json')
)

Set-StrictMode -Version Latest
$ErrorActionPreference = 'Stop'

function New-Line {
    param(
        [double]$X, [double]$Y, [double]$X2, [double]$Y2,
        [string]$Stroke = 'primary', [double]$Thickness = 3,
        [bool]$Dashed = $false, [bool]$ArrowStart = $false, [bool]$ArrowEnd = $false
    )
    [ordered]@{
        type = 'line'; x = $X; y = $Y; x2 = $X2; y2 = $Y2
        stroke = $Stroke; strokeThickness = $Thickness
        dashed = $Dashed; arrowStart = $ArrowStart; arrowEnd = $ArrowEnd
    }
}

function New-Polyline {
    param(
        [double[]]$Points, [string]$Stroke = 'primary', [double]$Thickness = 3,
        [bool]$Dashed = $false, [bool]$ArrowEnd = $false
    )
    [ordered]@{
        type = 'polyline'; points = @($Points); stroke = $Stroke
        strokeThickness = $Thickness; dashed = $Dashed; arrowEnd = $ArrowEnd
    }
}

function New-Polygon {
    param(
        [double[]]$Points, [string]$Stroke = 'primary', [string]$Fill = 'none',
        [double]$Thickness = 3, [bool]$Dashed = $false
    )
    [ordered]@{
        type = 'polygon'; points = @($Points); stroke = $Stroke; fill = $Fill
        strokeThickness = $Thickness; dashed = $Dashed
    }
}

function New-Ellipse {
    param(
        [double]$X, [double]$Y, [double]$RadiusX, [double]$RadiusY,
        [string]$Stroke = 'primary', [string]$Fill = 'none', [double]$Thickness = 3,
        [bool]$Dashed = $false
    )
    [ordered]@{
        type = 'ellipse'; x = $X; y = $Y; radiusX = $RadiusX; radiusY = $RadiusY
        stroke = $Stroke; fill = $Fill; strokeThickness = $Thickness; dashed = $Dashed
    }
}

function New-Arc {
    param(
        [double]$X, [double]$Y, [double]$RadiusX, [double]$RadiusY,
        [double]$StartAngle, [double]$SweepAngle,
        [string]$Stroke = 'accent', [double]$Thickness = 3
    )
    [ordered]@{
        type = 'arc'; x = $X; y = $Y; radiusX = $RadiusX; radiusY = $RadiusY
        startAngle = $StartAngle; sweepAngle = $SweepAngle
        stroke = $Stroke; strokeThickness = $Thickness
    }
}

function New-Text {
    param(
        [double]$X, [double]$Y, [string]$Text,
        [string]$Stroke = 'primary', [double]$FontSize = 24
    )
    [ordered]@{
        type = 'text'; x = $X; y = $Y; text = $Text
        stroke = $Stroke; fontSize = $FontSize
    }
}

function Get-Axes {
    @(
        New-Line 110 520 900 520 'primary' 3 $false $false $true
        New-Line 200 590 200 70 'primary' 3 $false $false $true
        New-Text 905 515 'x' 'primary' 24
        New-Text 210 55 'y' 'primary' 24
        New-Text 170 525 '0' 'muted' 20
    )
}

function Get-Triangle {
    param([bool]$Altitude = $false, [bool]$Isosceles = $false)
    $items = @(
        New-Polygon @(150,500, 820,500, 650,130) 'primary' 'none' 4
        New-Text 125 505 'A'
        New-Text 825 505 'B'
        New-Text 650 95 'C'
        New-Text 470 510 'c' 'muted' 22
        New-Text 365 285 ($(if ($Isosceles) { 'a' } else { 'b' })) 'muted' 22
        New-Text 755 300 'a' 'muted' 22
        New-Arc 150 500 85 85 -37 37 'accent' 4
        New-Text 220 440 'α' 'accent' 24
    )
    if ($Altitude) {
        $items += New-Line 650 130 650 500 'muted' 3 $true
        $items += New-Polyline @(650,470, 680,470, 680,500) 'accent' 3
        $items += New-Text 620 315 'h' 'muted' 23
    }
    $items
}

function Get-TwoTriangles {
    @(
        New-Polygon @(90,480, 400,480, 330,150) 'primary' 'none' 4
        New-Polygon @(560,480, 890,480, 800,175) 'accent' 'none' 4
        New-Text 65 485 'A'; New-Text 405 485 'B'; New-Text 325 115 'C'
        New-Text 535 485 'D'; New-Text 895 485 'E'; New-Text 800 140 'F'
        New-Text 415 280 '≅' 'muted' 34
    )
}

function Get-Circle {
    param([string]$Variant)
    $items = @(
        New-Ellipse 500 325 225 225 'primary' 'none' 4
        New-Text 485 310 'O' 'muted' 24
    )
    switch ($Variant) {
        'radius' {
            $items += New-Line 500 325 665 175 'accent' 4
            $items += New-Text 590 225 'r' 'accent' 25
        }
        'sector' {
            $items += New-Line 500 325 690 220 'accent' 4
            $items += New-Line 500 325 700 415 'accent' 4
            $items += New-Arc 500 325 85 85 -29 53 'accent' 4
            $items += New-Text 575 315 'α' 'accent' 24
            $items += New-Text 700 195 'A'; $items += New-Text 710 415 'B'
        }
        'angles' {
            $items += New-Line 300 430 665 175 'accent' 3
            $items += New-Line 300 430 700 410 'accent' 3
            $items += New-Line 500 325 665 175 'primary' 3
            $items += New-Line 500 325 700 410 'primary' 3
            $items += New-Arc 500 325 72 72 -42 65 'primary' 3
            $items += New-Arc 300 430 64 64 -35 32 'accent' 3
            $items += New-Text 530 315 '2α'; $items += New-Text 350 390 'α' 'accent'
            $items += New-Text 650 140 'A'; $items += New-Text 710 410 'B'; $items += New-Text 270 430 'C'
        }
        'tangent' {
            $items += New-Line 250 545 835 90 'accent' 3
            $items += New-Line 250 545 850 545 'primary' 3
            $items += New-Text 220 545 'A'; $items += New-Text 690 180 'B'; $items += New-Text 850 545 'P'
        }
        'tangents' {
            $items += New-Line 275 465 875 325 'accent' 3
            $items += New-Line 340 135 875 325 'accent' 3
            $items += New-Text 250 470 'A'; $items += New-Text 325 100 'B'; $items += New-Text 885 310 'P'
        }
        'secant' {
            $items += New-Line 175 555 850 180 'accent' 3
            $items += New-Line 175 555 900 555 'primary' 3
            $items += New-Text 155 555 'P'; $items += New-Text 370 430 'A'; $items += New-Text 675 255 'B'; $items += New-Text 500 560 'C'
        }
        'cyclic' {
            $items += New-Polygon @(355,155, 690,190, 725,455, 300,475) 'accent' 'none' 3
            $items += New-Text 335 120 'A'; $items += New-Text 700 160 'B'; $items += New-Text 735 455 'C'; $items += New-Text 270 475 'D'
            $items += New-Text 470 490 'α + γ = 180°' 'accent' 23
        }
        'tangential' {
            $items += New-Polygon @(260,455, 385,120, 760,175, 820,470) 'accent' 'none' 3
            $items += New-Text 245 460 'A'; $items += New-Text 370 85 'D'; $items += New-Text 770 145 'C'; $items += New-Text 830 470 'B'
        }
    }
    $items
}

function Get-Solid {
    param([string]$Kind)
    switch ($Kind) {
        'cuboid' {
            @(
                New-Polygon @(260,210,650,210,650,500,260,500) 'primary' 'none' 4
                New-Polygon @(390,110,780,110,780,400,650,500,650,210,260,210) 'muted' 'none' 3
                New-Line 260 500 390 400 'muted' 3 $true
                New-Line 390 110 390 400 'muted' 3 $true
                New-Line 390 400 780 400 'muted' 3 $true
                New-Text 235 505 'A'; New-Text 660 505 'B'; New-Text 790 400 'C'; New-Text 370 405 'D'
            )
        }
        'prism' {
            @(
                New-Polygon @(260,500,650,500,730,390,660,180,420,90,270,210) 'primary' 'none' 4
                New-Polyline @(270,210,570,300,730,390) 'muted' 3 $true
                New-Line 420 90 570 300 'muted' 3 $true
                New-Line 570 300 570 520 'accent' 3 $false $false $true
                New-Text 585 395 'h' 'accent'
            )
        }
        'pyramid' {
            @(
                New-Polygon @(230,500,700,500,820,400,420,390) 'primary' 'none' 4
                New-Line 525 80 230 500 'primary' 4
                New-Line 525 80 700 500 'primary' 4
                New-Line 525 80 820 400 'muted' 3 $true
                New-Line 525 80 420 390 'muted' 3 $true
                New-Line 525 80 525 430 'accent' 3 $true $false $true
                New-Text 540 260 'h' 'accent'; New-Text 515 45 'S'
            )
        }
        'cylinder' {
            @(
                New-Ellipse 500 145 180 70 'primary' 'none' 4
                New-Ellipse 500 500 180 70 'primary' 'none' 4 $true
                New-Line 320 145 320 500 'primary' 4
                New-Line 680 145 680 500 'primary' 4
                New-Line 500 500 650 500 'accent' 3
                New-Text 565 470 'r' 'accent'; New-Text 700 310 'h'
            )
        }
        'cone' {
            @(
                New-Ellipse 500 500 210 70 'primary' 'none' 4 $true
                New-Line 500 70 290 500 'primary' 4
                New-Line 500 70 710 500 'primary' 4
                New-Line 500 70 500 500 'accent' 3 $true
                New-Line 500 500 685 500 'accent' 3
                New-Text 515 280 'h' 'accent'; New-Text 595 470 'r' 'accent'; New-Text 650 285 'l'
            )
        }
        'sphere' {
            @(
                New-Ellipse 500 325 225 225 'primary' 'none' 4
                New-Ellipse 500 325 225 75 'muted' 'none' 3 $true
                New-Line 500 325 690 325 'accent' 3
                New-Text 585 290 'r' 'accent'; New-Text 475 310 'O'
            )
        }
        'cube-section' {
            @(
                New-Polygon @(250,210,650,210,650,520,250,520) 'accent' 'none' 3
                New-Polygon @(390,90,790,90,790,400,650,520,650,210,250,210) 'muted' 'none' 3
                New-Line 250 520 390 400 'muted' 3
                New-Line 390 90 390 400 'muted' 3
                New-Polygon @(250,210,790,90,650,520) 'danger' 'surface' 4
                New-Text 470 290 'przekrój' 'danger' 25
            )
        }
    }
}

function Get-Graph {
    param([string]$Kind)
    $items = @(Get-Axes)
    switch ($Kind) {
        'parabola' { $items += New-Polyline @(280,550,340,350,410,210,480,140,550,125,620,170,690,280,760,455,800,580) 'accent' 5 }
        'sine' { $items += New-Polyline @(100,520,180,430,260,350,340,430,420,520,500,610,580,520,660,430,740,350,820,430,900,520) 'accent' 5 }
        'cosine' { $items += New-Polyline @(100,350,180,430,260,520,340,610,420,520,500,430,580,350,660,430,740,520,820,610,900,520) 'accent' 5 }
        'tangent' {
            $items += New-Line 330 90 330 590 'muted' 2 $true
            $items += New-Line 670 90 670 590 'muted' 2 $true
            $items += New-Polyline @(340,575,390,520,440,450,500,325,560,200,610,130,660,80) 'accent' 5
        }
        'line-angle' {
            $items += New-Line 280 590 790 90 'accent' 4
            $items += New-Arc 705 520 80 80 -45 45 'accent' 3
            $items += New-Text 745 445 '45°' 'accent' 24
            $items += New-Text 690 270 'k' 'primary' 24
        }
        'coordinate-segment' {
            $items += New-Line 310 455 700 140 'accent' 4 $false $false $true
            $items += New-Ellipse 310 455 6 6 'primary' 'primary' 2
            $items += New-Ellipse 700 140 6 6 'primary' 'primary' 2
            $items += New-Text 275 455 'A'; $items += New-Text 715 115 'B'
            $items += New-Line 310 455 700 455 'muted' 2 $true
            $items += New-Line 700 455 700 140 'muted' 2 $true
        }
    }
    $items
}

function Get-VectorScene {
    param([string]$Kind)
    switch ($Kind) {
        'anatomy' {
            @(
                New-Line 190 470 760 180 'accent' 5 $false $false $true
                New-Ellipse 190 470 7 7 'primary' 'primary' 2
                New-Text 155 480 'A'; New-Text 770 155 'B'; New-Text 430 295 'AB⃗' 'accent' 30
                New-Text 95 540 'początek wektora' 'muted' 19
                New-Text 715 105 'zwrot i koniec' 'muted' 19
            )
        }
        'separate' {
            @(
                New-Line 150 180 450 80 'danger' 4 $false $false $true
                New-Line 400 500 190 350 'success' 4 $false $false $true
                New-Line 620 300 880 260 'accent' 4 $false $true
                New-Text 290 105 'v⃗' 'danger' 28; New-Text 270 410 'w⃗' 'success' 28; New-Text 735 230 'u⃗' 'accent' 28
            )
        }
        'parallelogram' {
            @(
                New-Line 190 500 420 150 'danger' 4 $false $false $true
                New-Line 190 500 740 500 'accent' 4 $false $false $true
                New-Line 420 150 800 150 'accent' 4 $false $false $true
                New-Line 740 500 800 150 'danger' 4 $false $false $true
                New-Text 245 315 'AD⃗' 'danger' 27; New-Text 485 505 'AB⃗' 'accent' 27
            )
        }
        'components' {
            $items = @(Get-Axes)
            $items += New-Line 310 510 560 150 'danger' 4 $false $false $true
            $items += New-Line 310 510 560 510 'muted' 3 $true
            $items += New-Line 560 510 560 150 'muted' 3 $true
            $items += New-Text 415 520 '3'; $items += New-Text 575 330 '4'; $items += New-Text 440 295 'v⃗' 'danger' 28
            $items
        }
        'coordinate' {
            $items = @(Get-Axes)
            $items += New-Line 360 170 770 470 'danger' 4 $false $false $true
            $items += New-Line 360 170 360 470 'muted' 3 $true
            $items += New-Line 360 470 770 470 'muted' 3 $true
            $items += New-Text 475 285 'AB⃗ = [8,-6]' 'danger' 25
            $items
        }
        'equal' {
            @(
                New-Line 130 490 450 270 'danger' 4 $false $false $true
                New-Line 850 220 530 440 'accent' 4 $false $false $true
                New-Line 450 270 850 220 'muted' 2 $true
                New-Line 130 490 530 440 'muted' 2 $true
                New-Text 275 350 'v⃗' 'danger' 28; New-Text 665 310 'u⃗' 'accent' 28
            )
        }
        'sum' {
            @(
                New-Line 190 520 420 140 'primary' 4 $false $false $true
                New-Line 420 140 800 300 'primary' 4 $false $false $true
                New-Line 190 520 800 300 'danger' 5 $false $false $true
                New-Text 275 300 'v⃗'; New-Text 610 175 'w⃗'; New-Text 470 405 'v⃗ + w⃗' 'danger' 28
            )
        }
        'difference' {
            @(
                New-Line 390 500 620 180 'primary' 4 $false $false $true
                New-Line 620 180 890 300 'primary' 4 $false $false $true
                New-Line 190 100 390 500 'danger' 4 $false $false $true
                New-Line 190 100 620 180 'accent' 3 $true $false $true
                New-Text 235 280 'v⃗ - w⃗' 'danger' 27; New-Text 720 190 'w⃗'; New-Text 455 315 'v⃗'
            )
        }
    }
}

function Get-TemplatePrimitives {
    param([string]$Template)
    switch ($Template) {
        'coordinate-segment' { Get-Graph 'coordinate-segment' }
        'line-angle' { Get-Graph 'line-angle' }
        'two-triangles' { Get-TwoTriangles }
        'triangle' { Get-Triangle }
        'triangle-altitude' { Get-Triangle $true }
        'isosceles' { Get-Triangle $true $true }
        'parallel-lines' {
            @(
                New-Line 110 480 900 480 'primary' 4
                New-Line 200 300 930 300 'primary' 4
                New-Line 250 570 700 110 'accent' 4
                New-Text 135 500 'p'; New-Text 875 320 'q'; New-Text 585 205 'k'
            )
        }
        'thales' {
            @(
                New-Line 90 430 900 430 'primary' 3
                New-Line 120 530 800 100 'accent' 3
                New-Line 320 535 250 250 'primary' 3
                New-Line 650 520 580 140 'primary' 3
                New-Text 80 440 'P'; New-Text 300 450 'B'; New-Text 625 450 'D'
            )
        }
        'trapezoid' {
            @(
                New-Polygon @(170,500,830,500,690,180,310,180) 'primary' 'none' 4
                New-Line 690 180 690 500 'accent' 3 $true
                New-Text 500 510 'a'; New-Text 485 140 'b'; New-Text 710 330 'h' 'accent'
            )
        }
        'parallelogram' {
            @(
                New-Polygon @(170,500,720,500,850,180,300,180) 'primary' 'none' 4
                New-Line 170 500 850 180 'accent' 3
                New-Line 300 180 720 500 'accent' 3
                New-Text 120 505 'A'; New-Text 730 505 'B'; New-Text 860 150 'C'; New-Text 270 150 'D'
            )
        }
        'rhombus' {
            @(
                New-Polygon @(150,325,500,110,850,325,500,540) 'primary' 'none' 4
                New-Line 150 325 850 325 'accent' 3
                New-Line 500 110 500 540 'accent' 3
                New-Polyline @(500,325,530,325,530,355,500,355) 'muted' 3
            )
        }
        'circle-radius' { Get-Circle 'radius' }
        'circle-sector' { Get-Circle 'sector' }
        'circle-angles' { Get-Circle 'angles' }
        'circle-tangent' { Get-Circle 'tangent' }
        'circle-tangents' { Get-Circle 'tangents' }
        'circle-secant' { Get-Circle 'secant' }
        'circle-cyclic' { Get-Circle 'cyclic' }
        'circle-tangential' { Get-Circle 'tangential' }
        'plane' {
            @(
                New-Polygon @(170,470,760,470,900,210,310,210) 'muted' 'surface' 3
                New-Line 190 560 780 110 'accent' 4
                New-Line 350 380 850 300 'primary' 4
                New-Line 510 330 510 120 'danger' 3 $false $false $true
                New-Text 805 445 'l'; New-Text 835 270 'm'; New-Text 525 115 'k'; New-Text 485 340 'P'
            )
        }
        'cuboid' { Get-Solid 'cuboid' }
        'prism' { Get-Solid 'prism' }
        'pyramid' { Get-Solid 'pyramid' }
        'cylinder' { Get-Solid 'cylinder' }
        'cone' { Get-Solid 'cone' }
        'sphere' { Get-Solid 'sphere' }
        'cube-section' { Get-Solid 'cube-section' }
        'right-triangle' { Get-Triangle $false }
        'unit-circle' {
            $items = @(Get-Axes)
            $items += New-Line 200 520 550 170 'accent' 4
            $items += New-Line 550 170 550 520 'muted' 3 $true
            $items += New-Arc 200 520 110 110 -45 45 'accent' 3
            $items += New-Text 360 320 'r'; $items += New-Text 565 145 'M = (x,y)'; $items += New-Text 280 470 'α'
            $items
        }
        'sine' { Get-Graph 'sine' }
        'cosine' { Get-Graph 'cosine' }
        'tangent' { Get-Graph 'tangent' }
        'trig-table' {
            @(
                New-Polygon @(100,100,900,100,900,550,100,550) 'primary' 'surface' 3
                New-Line 100 190 900 190 'primary' 2; New-Line 100 280 900 280 'primary' 2
                New-Line 100 370 900 370 'primary' 2; New-Line 100 460 900 460 'primary' 2
                New-Line 280 100 280 550 'primary' 2; New-Line 435 100 435 550 'primary' 2
                New-Line 590 100 590 550 'primary' 2; New-Line 745 100 745 550 'primary' 2
                New-Text 130 125 'α'; New-Text 300 125 '0°'; New-Text 455 125 '30°'; New-Text 610 125 '45°'; New-Text 765 125 '60°'
                New-Text 125 215 'sin α'; New-Text 125 305 'cos α'; New-Text 125 395 'tg α'
                New-Text 315 215 '0'; New-Text 470 215 '1/2'; New-Text 625 215 '√2/2'; New-Text 780 215 '√3/2'
            )
        }
        'parabola' { Get-Graph 'parabola' }
        'incircle' {
            $items = @(Get-Triangle)
            $items += New-Ellipse 550 380 105 105 'accent' 'none' 3
            $items += New-Line 550 380 620 455 'accent' 3
            $items += New-Text 525 350 'O'; $items += New-Text 615 385 '100°' 'danger' 22
            $items
        }
        'intersecting-chords' {
            $items = @(Get-Circle 'radius')
            $items += New-Line 310 430 690 180 'accent' 3
            $items += New-Line 285 245 720 405 'accent' 3
            $items += New-Text 485 300 '140°' 'danger' 22; $items += New-Text 650 180 '55°' 'accent' 22
            $items
        }
        'square' {
            @(
                New-Polygon @(250,150,750,150,750,500,250,500) 'primary' 'none' 4
                New-Line 250 500 750 150 'accent' 3
                New-Line 250 300 750 300 'muted' 3
                New-Text 225 505 'A'; New-Text 760 505 'B'; New-Text 760 115 'C'; New-Text 225 115 'D'
            )
        }
        'exam-prism' {
            $items = @(Get-Solid 'prism')
            $items += New-Text 160 510 '2'; $items += New-Text 690 520 '2'; $items += New-Text 760 365 '2'
            $items
        }
        'exam-triangle' {
            $items = @(Get-Triangle $true)
            $items += New-Text 405 460 '6'; $items += New-Text 365 440 '60°' 'accent'; $items += New-Text 720 450 '30°' 'accent'
            $items
        }
        'trapezoid-diagonals' {
            $items = @(Get-TemplatePrimitives 'trapezoid')
            $items += New-Line 170 500 690 180 'accent' 3
            $items += New-Line 310 180 830 500 'accent' 3
            $items += New-Text 500 330 'S'
            $items
        }
        'vector-anatomy' { Get-VectorScene 'anatomy' }
        'vectors' { Get-VectorScene 'separate' }
        'vector-parallelogram' { Get-VectorScene 'parallelogram' }
        'vector-components' { Get-VectorScene 'components' }
        'vector-coordinate' { Get-VectorScene 'coordinate' }
        'vector-equal' { Get-VectorScene 'equal' }
        'vector-sum' { Get-VectorScene 'sum' }
        'vector-difference' { Get-VectorScene 'difference' }
        default { throw "Nieznany szablon diagramu: $Template" }
    }
}

function New-Definition {
    param([string]$Id, [string]$SourceId, [string]$AlternativeText, [string]$Template)
    [ordered]@{
        id = $Id
        sourceId = $SourceId
        alternativeText = $AlternativeText
        width = 1000
        height = 650
        primitives = @(Get-TemplatePrimitives $Template)
    }
}

$definitions = @(
    New-Definition 'formula-w9a' 'cke-formula-2023' 'Układ współrzędnych z odcinkiem łączącym punkty A i B.' 'coordinate-segment'
    New-Definition 'formula-w9b' 'cke-formula-2023' 'Prosta y równa się ax plus b oraz kąt nachylenia alfa.' 'line-angle'
    New-Definition 'formula-w10e' 'cke-formula-2023' 'Trójkąt z bokami a, b, c i wysokością h.' 'triangle-altitude'
    New-Definition 'formula-w10f' 'cke-formula-2023' 'Trójkąt równoramienny z opuszczoną wysokością.' 'isosceles'
    New-Definition 'formula-w10g' 'cke-formula-2023' 'Dwie proste równoległe przecięte sieczną.' 'parallel-lines'
    New-Definition 'formula-w10a' 'cke-formula-2023' 'Dwa trójkąty używane do porównania cech przystawania.' 'two-triangles'
    New-Definition 'formula-w10b' 'cke-formula-2023' 'Dwa odpowiadające sobie trójkąty.' 'two-triangles'
    New-Definition 'formula-w10c' 'cke-formula-2023' 'Para trójkątów o odpowiadających bokach i kątach.' 'two-triangles'
    New-Definition 'formula-w10d' 'cke-formula-2023' 'Trójkąt ABC z oznaczonymi bokami i kątami.' 'triangle'
    New-Definition 'formula-w10h' 'cke-formula-2023' 'Konfiguracja prostych do twierdzenia Talesa.' 'thales'
    New-Definition 'formula-w10i' 'cke-formula-2023' 'Trapez o podstawach a i b oraz wysokości h.' 'trapezoid'
    New-Definition 'formula-w10m' 'cke-formula-2023' 'Koło o środku O i promieniu r.' 'circle-radius'
    New-Definition 'formula-w10n' 'cke-formula-2023' 'Wycinek koła o kącie środkowym alfa.' 'circle-sector'
    New-Definition 'formula-w10o' 'cke-formula-2023' 'Kąt środkowy i kąt wpisany oparte na tym samym łuku.' 'circle-angles'
    New-Definition 'formula-w10p' 'cke-formula-2023' 'Kąt między styczną i cięciwą okręgu.' 'circle-tangent'
    New-Definition 'formula-w10r' 'cke-formula-2023' 'Styczna do okręgu i promień poprowadzony do punktu styczności.' 'circle-tangent'
    New-Definition 'formula-w10s' 'cke-formula-2023' 'Dwie styczne poprowadzone z punktu P do okręgu.' 'circle-tangents'
    New-Definition 'formula-w10t' 'cke-formula-2023' 'Sieczna i styczna poprowadzone z punktu zewnętrznego.' 'circle-secant'
    New-Definition 'formula-w10j' 'cke-formula-2023' 'Trapez z zaznaczoną wysokością.' 'trapezoid'
    New-Definition 'formula-w10k' 'cke-formula-2023' 'Równoległobok z przekątnymi.' 'parallelogram'
    New-Definition 'formula-w10l' 'cke-formula-2023' 'Romb z prostopadłymi przekątnymi.' 'rhombus'
    New-Definition 'formula-w10u' 'cke-formula-2023' 'Czworokąt wpisany w okrąg.' 'circle-cyclic'
    New-Definition 'formula-w10w' 'cke-formula-2023' 'Czworokąt opisany na okręgu.' 'circle-tangential'
    New-Definition 'formula-w11a' 'cke-formula-2023' 'Proste k, l i m względem płaszczyzny i punktu P.' 'plane'
    New-Definition 'formula-w11b' 'cke-formula-2023' 'Prostopadłościan z oznaczonymi krawędziami.' 'cuboid'
    New-Definition 'formula-w11c' 'cke-formula-2023' 'Graniastosłup prosty z wysokością h.' 'prism'
    New-Definition 'formula-w11d' 'cke-formula-2023' 'Ostrosłup z wysokością h.' 'pyramid'
    New-Definition 'formula-w11e' 'cke-formula-2023' 'Walec o promieniu r i wysokości h.' 'cylinder'
    New-Definition 'formula-w11f' 'cke-formula-2023' 'Stożek o promieniu r, wysokości h i tworzącej l.' 'cone'
    New-Definition 'formula-w11g' 'cke-formula-2023' 'Kula o środku O i promieniu r.' 'sphere'
    New-Definition 'formula-w12a' 'cke-formula-2023' 'Trójkąt prostokątny z bokami a, b, c i kątem alfa.' 'right-triangle'
    New-Definition 'formula-w12b' 'cke-formula-2023' 'Punkt M w układzie współrzędnych i kąt alfa.' 'unit-circle'
    New-Definition 'formula-w12c' 'cke-formula-2023' 'Wykres funkcji sinus.' 'sine'
    New-Definition 'formula-w12d' 'cke-formula-2023' 'Wykres funkcji cosinus.' 'cosine'
    New-Definition 'formula-w12e' 'cke-formula-2023' 'Wykres funkcji tangens z asymptotami.' 'tangent'
    New-Definition 'formula-w12f' 'cke-formula-2023' 'Tabela wartości funkcji trygonometrycznych dla wybranych kątów.' 'trig-table'

    New-Definition 'exam-mp21-z9' 'cke-2021-correction' 'Układ współrzędnych z prostą k nachyloną pod kątem 45 stopni.' 'line-angle'
    New-Definition 'exam-mp21-z12' 'cke-2021-correction' 'Wykres paraboli skierowanej ramionami w dół.' 'parabola'
    New-Definition 'exam-mp21-z17' 'cke-2021-correction' 'Okrąg z czworokątem ABCD i kątem 20 stopni.' 'circle-angles'
    New-Definition 'exam-mp21-z18' 'cke-2021-correction' 'Okrąg wpisany w trójkąt ABC i kąt środkowy 100 stopni.' 'incircle'
    New-Definition 'exam-mp21-z19' 'cke-2021-correction' 'Okrąg z przecinającymi się cięciwami oraz kątami 55 i 140 stopni.' 'intersecting-chords'
    New-Definition 'exam-mp21-z20' 'cke-2021-correction' 'Kwadrat ABCD z odcinkami wewnętrznymi.' 'square'
    New-Definition 'exam-mp21-z24' 'cke-2021-correction' 'Graniastosłup o krawędziach długości 2.' 'exam-prism'
    New-Definition 'exam-mp21-z32' 'cke-2021-correction' 'Trójkąt z podstawą podzieloną odcinkiem długości 6 i kątami 60 oraz 30 stopni.' 'exam-triangle'
    New-Definition 'exam-mp21-z33' 'cke-2021-correction' 'Trapez ABCD z przekątnymi przecinającymi się w punkcie S.' 'trapezoid-diagonals'

    New-Definition 'course-right-triangle' 'adam-course' 'Trójkąt prostokątny z przyprostokątnymi a i b, przeciwprostokątną c oraz kątem alfa.' 'right-triangle'
    New-Definition 'course-circle-angles' 'adam-course' 'Okrąg z kątem środkowym AOB i kątem wpisanym ACB opartymi na tym samym łuku.' 'circle-angles'
    New-Definition 'course-coordinate-vector' 'adam-course' 'Układ współrzędnych z punktami A i B oraz składowymi wektora od A do B.' 'coordinate-segment'
    New-Definition 'course-cube-section' 'adam-course' 'Sześcian z zaznaczoną płaszczyzną przekroju przechodzącą przez trzy wierzchołki.' 'cube-section'
    New-Definition 'course-vector-1' 'legacy-vectors' 'Wektor AB z początkiem, końcem, długością, kierunkiem i zwrotem.' 'vector-anatomy'
    New-Definition 'course-vector-2' 'legacy-vectors' 'Trzy wektory o różnych kierunkach i zwrotach.' 'vectors'
    New-Definition 'course-vector-3' 'legacy-vectors' 'Równoległobok z wektorami AD i BC oraz AB.' 'vector-parallelogram'
    New-Definition 'course-vector-4' 'legacy-vectors' 'Wektor o składowych 3 i 4 w układzie współrzędnych.' 'vector-components'
    New-Definition 'course-vector-5' 'legacy-vectors' 'Wektor AB o współrzędnych 8 i minus 6.' 'vector-coordinate'
    New-Definition 'course-vector-6' 'legacy-vectors' 'Dwa równe wektory przesunięte równolegle.' 'vector-equal'
    New-Definition 'course-vector-7' 'legacy-vectors' 'Dodawanie wektorów v i w metodą trójkąta.' 'vector-sum'
    New-Definition 'course-vector-8' 'legacy-vectors' 'Różnica wektorów v i w przedstawiona geometrycznie.' 'vector-difference'
)

if ($definitions.Count -ne 57) {
    throw "Katalog musi zawierać dokładnie 57 diagramów, a zawiera $($definitions.Count)."
}

$catalog = [ordered]@{ schemaVersion = 1; diagrams = $definitions }
$json = $catalog | ConvertTo-Json -Depth 100
$normalized = $json.Replace("`r`n", "`n") + "`n"
$target = [IO.Path]::GetFullPath($OutputPath)
[IO.Directory]::CreateDirectory([IO.Path]::GetDirectoryName($target)) | Out-Null
[IO.File]::WriteAllText($target, $normalized, [Text.UTF8Encoding]::new($false))
Write-Host "Wygenerowano 57 diagramów wektorowych: $target"
