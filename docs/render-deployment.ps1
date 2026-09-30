Add-Type -AssemblyName System.Drawing

$OutDir = "C:\Users\rchaturvedi6\.copilot\chats\2026-09-23\automatic-adventure-ee0d0d94\Indiana.AI.Integration\docs\images"
New-Item -ItemType Directory -Force -Path $OutDir | Out-Null

# ---- palette ----
$colBg      = [System.Drawing.Color]::White
$colInk     = [System.Drawing.Color]::FromArgb(31,41,55)
$colMuted   = [System.Drawing.Color]::FromArgb(107,114,128)
$colAws     = [System.Drawing.Color]::FromArgb(255,153,0)
$colVpc     = [System.Drawing.Color]::FromArgb(37,99,235)
$colSubnet  = [System.Drawing.Color]::FromArgb(22,163,74)
$colSubBg   = [System.Drawing.Color]::FromArgb(240,253,244)
$colGreen   = [System.Drawing.Color]::FromArgb(22,163,74)
$colOrange  = [System.Drawing.Color]::FromArgb(217,119,6)
$colPurple  = [System.Drawing.Color]::FromArgb(124,58,237)
$colRed      = [System.Drawing.Color]::FromArgb(220,38,38)
$colTeal    = [System.Drawing.Color]::FromArgb(13,148,136)
$colGray    = [System.Drawing.Color]::FromArgb(156,163,175)
$colBlueBg  = [System.Drawing.Color]::FromArgb(219,234,254)
$colOrgBg   = [System.Drawing.Color]::FromArgb(254,243,199)
$colPurpBg  = [System.Drawing.Color]::FromArgb(237,233,254)
$colTealBg  = [System.Drawing.Color]::FromArgb(204,251,241)
$colRedBg    = [System.Drawing.Color]::FromArgb(254,226,226)
$colGrayBg  = [System.Drawing.Color]::FromArgb(243,244,246)

$W = 1600; $H = 1000
$bmp = New-Object System.Drawing.Bitmap $W,$H
$g = [System.Drawing.Graphics]::FromImage($bmp)
$g.SmoothingMode = [System.Drawing.Drawing2D.SmoothingMode]::AntiAlias
$g.TextRenderingHint = [System.Drawing.Text.TextRenderingHint]::ClearTypeGridFit
$g.Clear($colBg)

function Font([single]$s,[string]$st="Regular") { New-Object System.Drawing.Font("Segoe UI",$s,[System.Drawing.FontStyle]::$st) }

function RR($x,$y,$w,$h,$r,$fill,$stroke,[single]$sw=2,[bool]$dash=$false) {
    $p = New-Object System.Drawing.Drawing2D.GraphicsPath
    $d = $r*2
    $p.AddArc($x,$y,$d,$d,180,90); $p.AddArc($x+$w-$d,$y,$d,$d,270,90)
    $p.AddArc($x+$w-$d,$y+$h-$d,$d,$d,0,90); $p.AddArc($x,$y+$h-$d,$d,$d,90,90); $p.CloseFigure()
    if ($fill) { $b = New-Object System.Drawing.SolidBrush $fill; $g.FillPath($b,$p); $b.Dispose() }
    if ($stroke) { $pen = New-Object System.Drawing.Pen($stroke,$sw); if($dash){$pen.DashStyle=[System.Drawing.Drawing2D.DashStyle]::Dash}; $g.DrawPath($pen,$p); $pen.Dispose() }
    $p.Dispose()
}

function T($text,$x,$y,$w,$h,$font,$color,[string]$al="Center",[string]$va="Center") {
    $b = New-Object System.Drawing.SolidBrush $color
    $f = New-Object System.Drawing.StringFormat
    switch($al){"Center"{$f.Alignment=1}"Near"{$f.Alignment=0}"Far"{$f.Alignment=2}}
    switch($va){"Center"{$f.LineAlignment=1}"Near"{$f.LineAlignment=0}"Far"{$f.LineAlignment=2}}
    $g.DrawString($text,$font,$b,(New-Object System.Drawing.RectangleF($x,$y,$w,$h)),$f)
    $b.Dispose(); $f.Dispose()
}

function Arrow($x1,$y1,$x2,$y2,$color,[single]$sw=2.2,[bool]$dash=$false) {
    $pen = New-Object System.Drawing.Pen($color,$sw)
    $pen.CustomEndCap = New-Object System.Drawing.Drawing2D.AdjustableArrowCap(5,5)
    if($dash){$pen.DashStyle=[System.Drawing.Drawing2D.DashStyle]::Dash}
    $g.DrawLine($pen,$x1,$y1,$x2,$y2); $pen.Dispose()
}

function Badge($n,$x,$y,$color) {
    RR $x $y 26 26 13 $color $null
    T "$n" $x $y 26 26 (Font 11 "Bold") ([System.Drawing.Color]::White)
}

# ===== Title =====
T "Indiana AI Integration Platform - Target Deployment Architecture (AWS)" 0 16 $W 30 (Font 19 "Bold") $colInk
T "Reusable, tenant-aware AI Document Verification integration - Provider Enrollment (Phase 1)" 0 46 $W 20 (Font 11 "Italic") $colMuted

# ===== On-prem / State side (left) =====
RR 30 90 250 470 10 $colGrayBg $colGray 2
T "Indiana State Data Center / Portal Network" 30 98 250 20 (Font 10 "Bold") $colInk "Center" "Near"

# InterChange / Portal
RR 55 135 200 70 8 $colBlueBg $colVpc 2
T "InterChange / HCP Portal" 55 148 200 20 (Font 11 "Bold") $colInk
T "provider uploads + submits" 55 170 200 18 (Font 8) $colMuted

# Oracle BLOB
RR 55 235 200 70 8 $colOrgBg $colOrange 2
T "CoreMMIS - Oracle BLOB" 55 248 200 20 (Font 10 "Bold") $colInk
T "attachments stored at upload" 55 270 200 18 (Font 8) $colMuted

# FMS / EDMS service
RR 55 335 200 80 8 $colOrgBg $colOrange 2
T "File Mgmt Server (FMS)" 55 348 200 20 (Font 10 "Bold") $colInk
T "GetEDMSFileQuery" 55 370 200 16 (Font 8 "Italic") $colOrange
T "Indiana-owned; no direct DB" 55 388 200 16 (Font 8) $colMuted

# Identity
RR 55 445 200 95 8 $colPurpBg $colPurple 2
T "Gainwell Identity Broker" 55 458 200 20 (Font 10 "Bold") $colInk
T "(PingOne) OIDC / SAML" 55 480 200 16 (Font 8) $colMuted
T "JWT issuer - X-API-Key / X-Source" 55 500 200 16 (Font 8) $colMuted

# ===== Connectivity =====
Arrow 280 170 470 300 $colGreen 2.6
Badge 1 360 205 $colGreen
T "Site-to-Site VPN / Direct Connect (IPsec, TLS 1.2)" 300 250 220 30 (Font 8 "Italic") $colMuted "Center"

# ===== AWS Cloud boundary =====
RR 470 90 900 620 12 $colBg $colAws 2.5
T "AWS Cloud" 486 98 200 20 (Font 12 "Bold") $colAws "Near" "Near"

# ---- Shared Services / Security VPC ----
RR 490 130 860 70 10 $colRedBg $colRed 2 $true
T "Security / Edge" 500 136 150 16 (Font 9 "Bold") $colRed "Near" "Near"
RR 520 158 130 34 6 $colBg $colRed 1.5; T "AWS WAF" 520 158 130 34 (Font 9) $colInk
Badge 2 655 165 $colRed
RR 690 158 150 34 6 $colBg $colRed 1.5; T "API Gateway" 690 158 150 34 (Font 9) $colInk
RR 880 158 170 34 6 $colBg $colRed 1.5; T "App Load Balancer" 880 158 170 34 (Font 8) $colInk
RR 1090 158 240 34 6 $colBg $colPurple 1.5; T "Auth: X-API-Key / X-Source / JWT" 1090 158 240 34 (Font 8) $colInk

# ---- VPC ----
RR 490 220 860 480 10 $colBg $colVpc 2
T "VPC  (bps-hc-aws  -  us-east-1)" 502 226 300 18 (Font 10 "Bold") $colVpc "Near" "Near"

# ---- Private Subnet: AI Integration Platform (EKS) ----
RR 510 260 830 300 10 $colSubBg $colSubnet 2
T "Private Subnet  -  AI Integration Platform (EKS Auto / containers)" 522 266 600 18 (Font 10 "Bold") $colSubnet "Near" "Near"
Badge 3 1305 262 $colSubnet

# Gateway pods row
$px = 535; $py = 300; $pw = 150; $ph = 60; $gap = 18
$pods = @(
  @{t="Upload API";s="ack 1002"},
  @{t="Validate API";s="failedDocs / summary"},
  @{t="Payload Mapper";s="-> AI payload"},
  @{t="Router";s="tenant-aware"},
  @{t="Adapter Framework";s="resolve by tenant"}
)
$i=0
foreach($p in $pods){
  $x = $px + $i*($pw+$gap)
  RR $x $py $pw $ph 8 $colBlueBg $colVpc 2
  T $p.t $x ($py+8) $pw 20 (Font 10 "Bold") $colInk
  T $p.s $x ($py+32) $pw 18 (Font 8) $colMuted
  if($i -gt 0){ Arrow ($x-$gap+2) ($py+$ph/2) ($x-2) ($py+$ph/2) $colVpc 2 }
  $i++
}

# Tenant adapters row
$ay = 400
$adapters = @(
  @{t="IndianaOracleAdapter";s="Indiana -> FMS";c=$colOrange;bg=$colOrgBg},
  @{t="SqlAdapter";s="Nevada -> SQL";c=$colTeal;bg=$colTealBg},
  @{t="FileStoreAdapter";s="FutureState -> File";c=$colPurple;bg=$colPurpBg}
)
$i=0
foreach($a in $adapters){
  $x = 535 + $i*($pw+$gap)
  RR $x $ay $pw 56 8 $a.bg $a.c 2
  T $a.t $x ($ay+8) $pw 20 (Font 9 "Bold") $colInk
  T $a.s $x ($ay+30) $pw 18 (Font 8) $colMuted
  $i++
}
# Router -> adapters
Arrow (535+3*($pw+$gap)+$pw/2) ($py+$ph) (535+0*($pw+$gap)+$pw/2) ($ay) $colVpc 1.8 $true

# Artifact Retrieval + Mock FMS + Verification (right side of subnet)
$rx = 535 + 3*($pw+$gap)
RR $rx $ay ($pw*2+$gap) 56 8 $colGrayBg $colSubnet 2
T "Artifact Retrieval  +  Mock GetEDMSFileQuery" $rx ($ay+8) ($pw*2+$gap) 20 (Font 9 "Bold") $colInk
T "returns Base64 + metadata (stand-in FMS)" $rx ($ay+30) ($pw*2+$gap) 18 (Font 8) $colMuted

# Verification boundary (AI team)
RR 535 480 ($pw*2+$gap) 56 8 $colRedBg $colRed 2 $true
T "AI Verification (mock)  |  AI Team boundary" 535 488 ($pw*2+$gap) 20 (Font 9 "Bold") $colInk
T "OCR / classification / extraction (external)" 535 510 ($pw*2+$gap) 18 (Font 8) $colMuted

# Result persistence
RR $rx 480 ($pw*2+$gap) 56 8 $colBlueBg $colVpc 2
T "Verification Result Persistence" $rx ($ay+80) ($pw*2+$gap) 20 (Font 9 "Bold") $colInk
T "fileId, trackingId, status, codes, correlationId" $rx ($ay+102) ($pw*2+$gap) 18 (Font 8) $colMuted

# ---- Data store (Aurora) private subnet strip ----
RR 510 578 830 106 10 $colSubBg $colSubnet 2
T "Private Subnet  -  Data / Results Store" 522 584 400 18 (Font 10 "Bold") $colSubnet "Near" "Near"
RR 535 608 210 60 8 $colBg $colVpc 2; T "Amazon Aurora" 535 618 210 20 (Font 10 "Bold") $colInk; T "verification results (prod)" 535 640 210 16 (Font 8) $colMuted
RR 770 608 250 60 8 $colBg $colVpc 2; T "Activity Log / Correlation" 770 618 250 20 (Font 10 "Bold") $colInk; T "structured logs by trackingId" 770 640 250 16 (Font 8) $colMuted
RR 1045 608 270 60 8 $colBg $colGray 1.5 2 $true; T "Monitoring / KPIs" 1045 618 270 20 (Font 10 "Bold") $colInk; T "metrics, latency, failure codes" 1045 640 270 16 (Font 8) $colMuted

# ===== Enso AI Verification Service (separate account) =====
RR 1400 130 170 250 10 $colBg $colOrange 2.5
T "AWS Account" 1408 136 150 16 (Font 9 "Bold") $colOrange "Near" "Near"
RR 1415 165 140 120 8 $colRedBg $colRed 2
T "ENSO" 1415 175 140 20 (Font 12 "Bold") $colInk
T "AI Verification" 1415 197 140 16 (Font 9) $colInk
T "Service" 1415 213 140 16 (Font 9) $colInk
T "/pe/document_upload" 1415 235 140 14 (Font 7 "Italic") $colMuted
T "/pe/document_validate" 1415 249 140 14 (Font 7 "Italic") $colMuted
T "(private EKS NLB)" 1415 267 140 14 (Font 7) $colRed
Badge 4 1560 165 $colRed

# gateway -> Enso (PrivateLink)
Arrow 1155 330 1400 250 $colRed 2.6
T "PrivateLink / VPC peering" 1180 280 200 16 (Font 8 "Italic") $colMuted

# Enso -> Mock FMS callback
Arrow 1470 285 1150 460 $colOrange 2 $true
T "GetEDMSFileQuery callback" 1200 400 220 16 (Font 8 "Italic") $colOrange

# ===== CI/CD =====
RR 560 730 470 90 10 $colBg $colAws 2
T "CI / CD" 572 736 120 16 (Font 10 "Bold") $colAws "Near" "Near"
RR 585 760 120 40 6 $colGrayBg $colInk 1.5; T "Developers" 585 760 120 40 (Font 9) $colInk
RR 725 760 130 40 6 $colBlueBg $colVpc 1.5; T "Azure DevOps" 725 760 130 40 (Font 9) $colInk
RR 875 760 130 40 6 $colGrayBg $colGreen 1.5; T "Container Registry" 875 760 130 40 (Font 8) $colInk
Arrow 705 780 725 780 $colInk 1.8; Arrow 855 780 875 780 $colInk 1.8
Arrow 900 760 900 685 $colGreen 2 $true
T "deploy image" 905 700 90 16 (Font 8 "Italic") $colMuted

# ===== Numbered flow legend (right) =====
$lx = 1400; $ly = 410
T "Request Flow" $lx $ly 190 20 (Font 11 "Bold") $colInk "Near" "Near"
$flows = @(
  "1  InterChange -> Gateway (VPN)",
  "2  Edge: WAF / gateway / auth",
  "3  AI Integration Platform (EKS)",
  "   Upload 1002 -> Validate",
  "4  Gateway -> ENSO (PrivateLink)",
  "5  ENSO -> Mock FMS retrieval",
  "6  Base64 doc + metadata back",
  "7  AI verify -> failedDocs",
  "8  Persist result (Aurora)",
  "9  Result queryable by fileId"
)
$fy = $ly+26
foreach($f in $flows){ T $f $lx $fy 190 18 (Font 9) $colMuted "Near" "Near"; $fy += 20 }

# security note box
RR $lx 640 190 90 8 $colGrayBg $colGray 1.5
T "Tenant-aware + config-driven" $lx 648 190 16 (Font 8 "Bold") $colInk "Near" "Near"
T "routing; no hardcoding." $lx 664 190 16 (Font 8) $colMuted "Near" "Near"
T "Mock / live toggle via" $lx 686 190 16 (Font 8) $colMuted "Near" "Near"
T "Integration:UseMocks." $lx 702 190 16 (Font 8) $colMuted "Near" "Near"

# connectivity legend
$cy = 760
T "Connectivity" $lx $cy 190 18 (Font 9 "Bold") $colInk "Near" "Near"
$pen=New-Object System.Drawing.Pen($colGreen,3); $g.DrawLine($pen,$lx,($cy+26),($lx+30),($cy+26)); $pen.Dispose()
T "VPN / Direct Connect" ($lx+38) ($cy+18) 160 16 (Font 8) $colMuted "Near" "Near"
$pen=New-Object System.Drawing.Pen($colRed,3); $g.DrawLine($pen,$lx,($cy+48),($lx+30),($cy+48)); $pen.Dispose()
T "PrivateLink (HTTPS TLS1.2)" ($lx+38) ($cy+40) 160 16 (Font 8) $colMuted "Near" "Near"
$pen=New-Object System.Drawing.Pen($colOrange,2); $pen.DashStyle=[System.Drawing.Drawing2D.DashStyle]::Dash; $g.DrawLine($pen,$lx,($cy+70),($lx+30),($cy+70)); $pen.Dispose()
T "FMS retrieval callback" ($lx+38) ($cy+62) 160 16 (Font 8) $colMuted "Near" "Near"

$bmp.Save("$OutDir\04-aws-deployment.png",[System.Drawing.Imaging.ImageFormat]::Png)
$g.Dispose(); $bmp.Dispose()
Write-Host "Saved 04-aws-deployment.png"
Get-Item "$OutDir\04-aws-deployment.png" | Select-Object Name,Length
