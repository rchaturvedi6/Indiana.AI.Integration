$md = "C:\Users\rchaturvedi6\.copilot\chats\2026-09-23\automatic-adventure-ee0d0d94\Indiana.AI.Integration\docs\ARCHITECTURE.md"
$docx = "C:\Users\rchaturvedi6\.copilot\chats\2026-09-23\automatic-adventure-ee0d0d94\Indiana.AI.Integration\docs\ARCHITECTURE.docx"
$imgDir = "C:\Users\rchaturvedi6\.copilot\chats\2026-09-23\automatic-adventure-ee0d0d94\Indiana.AI.Integration\docs"
$lines = Get-Content $md

$word = New-Object -ComObject Word.Application
$word.Visible = $false
$doc = $word.Documents.Add()
$sel = $word.Selection

function Set-Style($sel, $name) { try { $sel.Style = $name } catch {} }

$inCode = $false
foreach ($line in $lines) {
    # image embed:  ![alt](images/xyz.png)
    if ($line -match '^\!\[.*\]\((.+)\)\s*$') {
        $rel = $Matches[1]
        $path = Join-Path $imgDir $rel
        if (Test-Path $path) {
            $shape = $sel.InlineShapes.AddPicture($path)
            # scale to ~6.2 inches wide
            $targetW = 6.2 * 72
            if ($shape.Width -gt $targetW) {
                $ratio = $targetW / $shape.Width
                $shape.Height = [int]($shape.Height * $ratio)
                $shape.Width = [int]$targetW
            }
            $sel.TypeParagraph()
        }
        continue
    }
    if ($line -match '^```') { $inCode = -not $inCode; continue }
    if ($inCode) {
        Set-Style $sel "No Spacing"
        $sel.Font.Name = "Consolas"; $sel.Font.Size = 9
        $sel.TypeText($line); $sel.TypeParagraph()
        $sel.Font.Name = "Calibri"; $sel.Font.Size = 11
        continue
    }
    if ($line -match '^# (.*)')      { Set-Style $sel "Heading 1"; $sel.TypeText($Matches[1]); $sel.TypeParagraph(); Set-Style $sel "Normal"; continue }
    if ($line -match '^## (.*)')     { Set-Style $sel "Heading 2"; $sel.TypeText(($Matches[1] -replace '[#`*]','')); $sel.TypeParagraph(); Set-Style $sel "Normal"; continue }
    if ($line -match '^### (.*)')    { Set-Style $sel "Heading 3"; $sel.TypeText(($Matches[1] -replace '[#`*]','')); $sel.TypeParagraph(); Set-Style $sel "Normal"; continue }
    if ($line -match '^---\s*$')     { $sel.TypeParagraph(); continue }
    $clean = $line -replace '\*\*','' -replace '`','' -replace '^\s*[-|]\s*','  * ' -replace '\|',' '
    Set-Style $sel "Normal"
    $sel.TypeText($clean); $sel.TypeParagraph()
}

$doc.SaveAs([ref]$docx, [ref]16)
$doc.Close()
$word.Quit()
[System.Runtime.InteropServices.Marshal]::ReleaseComObject($word) | Out-Null
Write-Host "Saved docx:"; Get-Item $docx | Select-Object Name,Length
