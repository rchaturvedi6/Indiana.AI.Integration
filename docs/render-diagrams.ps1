Add-Type -AssemblyName System.Drawing

$OutDir = "C:\Users\rchaturvedi6\.copilot\chats\2026-09-23\automatic-adventure-ee0d0d94\Indiana.AI.Integration\docs\images"
New-Item -ItemType Directory -Force -Path $OutDir | Out-Null

# ---- palette ----
$colBg      = [System.Drawing.Color]::FromArgb(255,255,255)
$colInk     = [System.Drawing.Color]::FromArgb(31,41,55)
$colMuted   = [System.Drawing.Color]::FromArgb(107,114,128)
$colBlue    = [System.Drawing.Color]::FromArgb(37,99,235)
$colBlueBg  = [System.Drawing.Color]::FromArgb(219,234,254)
$colGreen   = [System.Drawing.Color]::FromArgb(22,163,74)
$colGreenBg = [System.Drawing.Color]::FromArgb(220,252,231)
$colAmber   = [System.Drawing.Color]::FromArgb(217,119,6)
$colAmberBg = [System.Drawing.Color]::FromArgb(254,243,199)
$colGrayBg  = [System.Drawing.Color]::FromArgb(243,244,246)
$colGray    = [System.Drawing.Color]::FromArgb(156,163,175)

function New-Canvas([int]$w,[int]$h) {
    $bmp = New-Object System.Drawing.Bitmap $w,$h
    $g = [System.Drawing.Graphics]::FromImage($bmp)
    $g.SmoothingMode = [System.Drawing.Drawing2D.SmoothingMode]::AntiAlias
    $g.TextRenderingHint = [System.Drawing.Text.TextRenderingHint]::ClearTypeGridFit
    $g.Clear($colBg)
    return @{ bmp=$bmp; g=$g }
}

function Get-Font([single]$size,[string]$style="Regular",[string]$family="Segoe UI") {
    return New-Object System.Drawing.Font($family,$size,[System.Drawing.FontStyle]::$style)
}

function Draw-RoundRect($g,[int]$x,[int]$y,[int]$w,[int]$h,[int]$r,$fill,$stroke,[single]$sw=2) {
    $path = New-Object System.Drawing.Drawing2D.GraphicsPath
    $d = $r*2
    $path.AddArc($x,$y,$d,$d,180,90)
    $path.AddArc($x+$w-$d,$y,$d,$d,270,90)
    $path.AddArc($x+$w-$d,$y+$h-$d,$d,$d,0,90)
    $path.AddArc($x,$y+$h-$d,$d,$d,90,90)
    $path.CloseFigure()
    if ($fill) { $b = New-Object System.Drawing.SolidBrush $fill; $g.FillPath($b,$path); $b.Dispose() }
    if ($stroke) { $p = New-Object System.Drawing.Pen($stroke,$sw); $g.DrawPath($p,$path); $p.Dispose() }
    $path.Dispose()
}

function Draw-Text($g,[string]$text,[int]$x,[int]$y,[int]$w,[int]$h,$font,$color,[string]$align="Center",[string]$vAlign="Center") {
    $b = New-Object System.Drawing.SolidBrush $color
    $fmt = New-Object System.Drawing.StringFormat
    switch ($align)  { "Center" { $fmt.Alignment = 1 } "Near" { $fmt.Alignment = 0 } "Far" { $fmt.Alignment = 2 } }
    switch ($vAlign) { "Center" { $fmt.LineAlignment = 1 } "Near" { $fmt.LineAlignment = 0 } "Far" { $fmt.LineAlignment = 2 } }
    $rect = New-Object System.Drawing.RectangleF($x,$y,$w,$h)
    $g.DrawString($text,$font,$b,$rect,$fmt)
    $b.Dispose(); $fmt.Dispose()
}

function Draw-Arrow($g,[int]$x1,[int]$y1,[int]$x2,[int]$y2,$color,[single]$sw=2.2,[bool]$dashed=$false) {
    $p = New-Object System.Drawing.Pen($color,$sw)
    $cap = New-Object System.Drawing.Drawing2D.AdjustableArrowCap(5,5)
    $p.CustomEndCap = $cap
    if ($dashed) { $p.DashStyle = [System.Drawing.Drawing2D.DashStyle]::Dash }
    $g.DrawLine($p,$x1,$y1,$x2,$y2)
    $p.Dispose()
}

# =====================================================================
# Diagram 1 — System Context
# =====================================================================
$c = New-Canvas 1000 560
$g = $c.g
Draw-Text $g "System Context — Indiana AI Document Verification" 0 18 1000 30 (Get-Font 18 "Bold") $colInk

$hUpload = Get-Font 12 "Bold"
$hSmall  = Get-Font 9 "Regular"
$hLbl    = Get-Font 9 "Italic"

# Provider Portal
Draw-RoundRect $g 60 120 220 90 10 $colBlueBg $colBlue 2
Draw-Text $g "Provider Portal" 60 138 220 24 $hUpload $colInk
Draw-Text $g "uploads documents,`nrequests validation" 60 162 220 40 $hSmall $colMuted

# Gateway (this POC)
Draw-RoundRect $g 390 100 300 130 12 $colGreenBg $colGreen 3
Draw-Text $g "AI Integration Gateway" 390 120 300 26 (Get-Font 13 "Bold") $colInk
Draw-Text $g "(this POC — AI Verification Service boundary)" 390 148 300 20 $hLbl $colGreen
Draw-Text $g "mapping · routing · tracking ids`norchestration · persistence" 390 170 300 44 $hSmall $colMuted

# FMS
Draw-RoundRect $g 780 120 170 90 10 $colAmberBg $colAmber 2
Draw-Text $g "File Mgmt Server`n(FMS / EDMS)" 780 140 170 40 $hUpload $colInk
Draw-Text $g "stores files" 780 178 170 20 $hSmall $colMuted

# Screener (greyed / out of scope)
Draw-RoundRect $g 60 340 220 90 10 $colGrayBg $colGray 2
Draw-Text $g "Screener Service`n(Appian)" 60 358 220 40 $hUpload $colMuted
Draw-Text $g "APIs 4 & 5 — out of scope" 60 396 220 20 $hLbl $colGray

# arrows Portal <-> Gateway
Draw-Arrow $g 280 150 388 150 $colBlue
Draw-Text $g "document_upload /`ndocument_validate" 250 108 200 26 (Get-Font 8) $colMuted "Center"
Draw-Arrow $g 388 185 282 185 $colBlue
Draw-Text $g "ack / results" 250 190 200 18 $hSmall $colMuted "Center"

# arrows Gateway <-> FMS
Draw-Arrow $g 690 150 778 150 $colAmber
Draw-Text $g "GetEDMSFileQuery" 686 128 98 16 (Get-Font 7) $colMuted "Center"
Draw-Arrow $g 778 182 692 182 $colAmber
Draw-Text $g "file bytes (Base64)" 686 186 98 16 (Get-Font 7) $colMuted "Center"

# Screener dashed to gateway
Draw-Arrow $g 280 385 388 220 $colGray 2 $true

# legend
Draw-Text $g "Solid = implemented in POC     Dashed / grey = out of scope" 0 500 1000 20 $hLbl $colMuted
$c.bmp.Save("$OutDir\01-system-context.png",[System.Drawing.Imaging.ImageFormat]::Png)
$c.g.Dispose(); $c.bmp.Dispose()

# =====================================================================
# Diagram 2 — Clean Architecture layers (concentric)
# =====================================================================
$c = New-Canvas 900 640
$g = $c.g
Draw-Text $g "Clean Architecture — dependencies point inward" 0 18 900 30 (Get-Font 18 "Bold") $colInk

# concentric rounded rects
Draw-RoundRect $g 90 80 720 500 16 ([System.Drawing.Color]::FromArgb(239,246,255)) $colBlue 2
Draw-Text $g "Api   —   controllers · Swagger · DI host · health" 90 92 720 24 (Get-Font 12 "Bold") $colBlue "Center" "Near"

Draw-RoundRect $g 170 150 560 370 14 ([System.Drawing.Color]::FromArgb(240,253,244)) $colGreen 2
Draw-Text $g "Infrastructure   —   mocks · live HTTP clients · repositories" 170 162 560 22 (Get-Font 11 "Bold") $colGreen "Center" "Near"

Draw-RoundRect $g 250 220 400 240 12 ([System.Drawing.Color]::FromArgb(254,249,235)) $colAmber 2
Draw-Text $g "Application   —   use cases + port interfaces" 250 232 400 22 (Get-Font 11 "Bold") $colAmber "Center" "Near"

Draw-RoundRect $g 330 290 240 110 10 ([System.Drawing.Color]::FromArgb(243,244,246)) $colInk 2
Draw-Text $g "Domain" 330 300 240 24 (Get-Font 12 "Bold") $colInk "Center" "Near"
Draw-Text $g "models · enums`n(no dependencies)" 330 326 240 40 (Get-Font 9) $colMuted

# dependency note + inward arrow
Draw-Arrow $g 450 560 450 405 $colInk 2.4
Draw-Text $g "dependencies point inward only" 0 590 900 20 (Get-Font 10 "Italic") $colMuted
$c.bmp.Save("$OutDir\02-clean-architecture.png",[System.Drawing.Imaging.ImageFormat]::Png)
$c.g.Dispose(); $c.bmp.Dispose()

# =====================================================================
# Diagram 3 — Validate request flow (vertical)
# =====================================================================
$c = New-Canvas 980 720
$g = $c.g
Draw-Text $g "Request Lifecycle — POST /api/document-validate" 0 18 980 30 (Get-Font 18 "Bold") $colInk

$steps = @(
    @{ t="DocumentsController.ValidateAsync"; s="thin controller — delegates to the workflow"; col=$colBlue; bg=$colBlueBg },
    @{ t="WorkflowService.ValidateAsync"; s="orchestrates; persists results after"; col=$colGreen; bg=$colGreenBg },
    @{ t="IAiPlatformClient.ValidateAsync"; s="MOCK: LocalAiPlatformClient   |   LIVE: EnsoAiPlatformClient"; col=$colAmber; bg=$colAmberBg },
    @{ t="ValidationEngine — per attachment loop"; s="retrieval -> verification -> aggregation"; col=$colGreen; bg=$colGreenBg },
    @{ t="ArtifactRetrievalService -> Router -> FmsAdapter"; s="GetEDMSFileQuery: returns Base64 PDF from mock EDMS"; col=$colAmber; bg=$colAmberBg },
    @{ t="DocumentVerificationService.Verify"; s="compares entered vs extracted -> validation_errors (3001, 3116 ...)"; col=$colAmber; bg=$colAmberBg },
    @{ t="VerificationResultRepository.SaveAsync"; s="persists per-document VerificationResult (in-memory)"; col=$colGreen; bg=$colGreenBg },
    @{ t="DocumentValidationResponse"; s="route echoed + failedDocs + resultsSummary (2000/2001)"; col=$colBlue; bg=$colBlueBg }
)

$y = 70; $x = 150; $w = 680; $h = 58; $gap = 20
$i = 1
foreach ($st in $steps) {
    Draw-RoundRect $g $x $y $w $h 10 $st.bg $st.col 2
    # number badge
    Draw-RoundRect $g ($x-46) ($y+10) 36 36 18 $st.col $null
    Draw-Text $g "$i" ($x-46) ($y+10) 36 36 (Get-Font 13 "Bold") ([System.Drawing.Color]::White)
    Draw-Text $g $st.t $x ($y+8) $w 24 (Get-Font 12 "Bold") $colInk "Center" "Near"
    Draw-Text $g $st.s $x ($y+32) $w 20 (Get-Font 9) $colMuted "Center" "Near"
    if ($i -lt $steps.Count) { Draw-Arrow $g ($x+$w/2) ($y+$h) ($x+$w/2) ($y+$h+$gap) $colInk 2 }
    $y += $h + $gap; $i++
}
$c.bmp.Save("$OutDir\03-validate-flow.png",[System.Drawing.Imaging.ImageFormat]::Png)
$c.g.Dispose(); $c.bmp.Dispose()

Write-Host "Diagrams written to $OutDir"
Get-ChildItem $OutDir | Select-Object Name,Length
