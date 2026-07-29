param(
    [string]$OutputRoot = (Join-Path $PSScriptRoot '..\img\course')
)

Set-StrictMode -Version Latest
$ErrorActionPreference = 'Stop'

Add-Type -AssemblyName System.Drawing
New-Item -ItemType Directory -Path $OutputRoot -Force | Out-Null

function New-Pen([System.Drawing.Color]$Color, [float]$Width = 5) {
    $pen = [System.Drawing.Pen]::new($Color, $Width)
    $pen.StartCap = [System.Drawing.Drawing2D.LineCap]::Round
    $pen.EndCap = [System.Drawing.Drawing2D.LineCap]::Round
    return $pen
}

function Save-Diagram([string]$Name, [scriptblock]$Draw) {
    $bitmap = [System.Drawing.Bitmap]::new(1000, 650)
    $graphics = [System.Drawing.Graphics]::FromImage($bitmap)
    $graphics.SmoothingMode = [System.Drawing.Drawing2D.SmoothingMode]::AntiAlias
    $graphics.TextRenderingHint = [System.Drawing.Text.TextRenderingHint]::AntiAliasGridFit
    $graphics.Clear([System.Drawing.Color]::White)
    try {
        & $Draw $graphics
        $path = Join-Path $OutputRoot $Name
        $bitmap.Save($path, [System.Drawing.Imaging.ImageFormat]::Png)
    } finally {
        $graphics.Dispose()
        $bitmap.Dispose()
    }
}

$ink = [System.Drawing.Color]::FromArgb(30, 41, 59)
$blue = [System.Drawing.Color]::FromArgb(37, 99, 235)
$red = [System.Drawing.Color]::FromArgb(220, 38, 38)
$green = [System.Drawing.Color]::FromArgb(5, 150, 105)
$labelFont = [System.Drawing.Font]::new('Arial', 28, [System.Drawing.FontStyle]::Bold)
$smallFont = [System.Drawing.Font]::new('Arial', 22, [System.Drawing.FontStyle]::Regular)
$brush = [System.Drawing.SolidBrush]::new($ink)

try {
    Save-Diagram 'right-triangle.png' {
        param($graphics)
        $mainPen = New-Pen $blue
        $anglePen = New-Pen $red 4
        try {
            $graphics.DrawLine($mainPen, 180, 520, 820, 520)
            $graphics.DrawLine($mainPen, 180, 520, 180, 120)
            $graphics.DrawLine($mainPen, 180, 120, 820, 520)
            $graphics.DrawRectangle($mainPen, 180, 470, 50, 50)
            $graphics.DrawArc($anglePen, 690, 420, 170, 170, 180, 33)
            $graphics.DrawString('a', $labelFont, $brush, 485, 535)
            $graphics.DrawString('b', $labelFont, $brush, 120, 305)
            $graphics.DrawString('c', $labelFont, $brush, 510, 275)
            $graphics.DrawString('α', $labelFont, $brush, 720, 435)
        } finally {
            $mainPen.Dispose()
            $anglePen.Dispose()
        }
    }

    Save-Diagram 'circle-angles.png' {
        param($graphics)
        $circlePen = New-Pen $blue
        $radiusPen = New-Pen $green 4
        $anglePen = New-Pen $red 4
        try {
            $graphics.DrawEllipse($circlePen, 220, 70, 560, 500)
            $graphics.DrawLine($radiusPen, 500, 320, 300, 155)
            $graphics.DrawLine($radiusPen, 500, 320, 730, 200)
            $graphics.DrawLine($anglePen, 300, 155, 505, 550)
            $graphics.DrawLine($anglePen, 505, 550, 730, 200)
            $graphics.DrawString('O', $labelFont, $brush, 475, 290)
            $graphics.DrawString('A', $labelFont, $brush, 255, 115)
            $graphics.DrawString('B', $labelFont, $brush, 745, 165)
            $graphics.DrawString('C', $labelFont, $brush, 490, 560)
            $graphics.DrawString('kąt środkowy', $smallFont, $brush, 45, 280)
            $graphics.DrawString('kąt wpisany', $smallFont, $brush, 760, 500)
        } finally {
            $circlePen.Dispose()
            $radiusPen.Dispose()
            $anglePen.Dispose()
        }
    }

    Save-Diagram 'coordinate-vector.png' {
        param($graphics)
        $axisPen = New-Pen $ink 3
        $vectorPen = New-Pen $red 6
        $guidePen = New-Pen $green 3
        $guidePen.DashStyle = [System.Drawing.Drawing2D.DashStyle]::Dash
        try {
            $graphics.DrawLine($axisPen, 100, 520, 900, 520)
            $graphics.DrawLine($axisPen, 180, 590, 180, 70)
            $graphics.DrawLine($vectorPen, 300, 450, 760, 160)
            $graphics.DrawLine($vectorPen, 760, 160, 710, 170)
            $graphics.DrawLine($vectorPen, 760, 160, 740, 210)
            $graphics.DrawLine($guidePen, 300, 450, 760, 450)
            $graphics.DrawLine($guidePen, 760, 450, 760, 160)
            $graphics.FillEllipse($brush, 290, 440, 20, 20)
            $graphics.FillEllipse($brush, 750, 150, 20, 20)
            $graphics.DrawString('A', $labelFont, $brush, 255, 405)
            $graphics.DrawString('B', $labelFont, $brush, 775, 125)
            $graphics.DrawString('Δx', $smallFont, $brush, 500, 455)
            $graphics.DrawString('Δy', $smallFont, $brush, 775, 300)
            $graphics.DrawString('x', $labelFont, $brush, 905, 500)
            $graphics.DrawString('y', $labelFont, $brush, 155, 35)
        } finally {
            $axisPen.Dispose()
            $vectorPen.Dispose()
            $guidePen.Dispose()
        }
    }

    Save-Diagram 'cube-section.png' {
        param($graphics)
        $cubePen = New-Pen $blue 4
        $sectionPen = New-Pen $red 7
        $fillBrush = [System.Drawing.SolidBrush]::new([System.Drawing.Color]::FromArgb(55, 220, 38, 38))
        try {
            $front = [System.Drawing.Rectangle]::new(220, 210, 430, 330)
            $graphics.DrawRectangle($cubePen, $front)
            $graphics.DrawRectangle($cubePen, 350, 90, 430, 330)
            $graphics.DrawLine($cubePen, 220, 210, 350, 90)
            $graphics.DrawLine($cubePen, 650, 210, 780, 90)
            $graphics.DrawLine($cubePen, 220, 540, 350, 420)
            $graphics.DrawLine($cubePen, 650, 540, 780, 420)
            $section = [System.Drawing.Point[]]@(
                [System.Drawing.Point]::new(220, 210),
                [System.Drawing.Point]::new(780, 90),
                [System.Drawing.Point]::new(650, 540)
            )
            $graphics.FillPolygon($fillBrush, $section)
            $graphics.DrawPolygon($sectionPen, $section)
            $graphics.DrawString('przekrój', $labelFont, $brush, 405, 285)
        } finally {
            $cubePen.Dispose()
            $sectionPen.Dispose()
            $fillBrush.Dispose()
        }
    }
} finally {
    $labelFont.Dispose()
    $smallFont.Dispose()
    $brush.Dispose()
}

Write-Host "Wygenerowano 4 autorskie diagramy PNG w $OutputRoot."
