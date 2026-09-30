<#
    Test-EnsoAccess.ps1
    Run this ON YOUR on-network machine (VPN / corporate / VPC) that can route the
    private Enso endpoint. It checks, in order:
      1. DNS resolution
      2. TCP 443 reachability
      3. TLS handshake
      4. (optional) an authenticated POST to /pe/document_upload

    Connectivity checks (1-3) need NO API key.
    The authenticated call (4) needs the issued X-API-Key + X-Source.

    USAGE:
      # connectivity only
      ./Test-EnsoAccess.ps1

      # full test with credentials
      ./Test-EnsoAccess.ps1 -ApiKey "<ISSUED_KEY>" -Source "provider_portal"
#>
[CmdletBinding()]
param(
    [string]$BaseUrl = "https://enso-apis.enso.dev.pgwcloud.net",
    [string]$UploadPath = "/pe/document_upload",
    [string]$ApiKey,
    [string]$Source = "provider_portal",
    [int]$TimeoutSec = 15
)

$ErrorActionPreference = "Continue"
$uri = [Uri]$BaseUrl
$targetHost = $uri.Host
$port = if ($uri.Port -gt 0) { $uri.Port } else { 443 }

function Line($t) { Write-Host ("-" * 60); Write-Host $t; Write-Host ("-" * 60) }

Line "1. DNS resolution: $targetHost"
try {
    $dns = Resolve-DnsName $targetHost -ErrorAction Stop
    $dns | Select-Object Name, Type, IPAddress, NameHost | Format-Table -AutoSize
    $ips = ($dns | Where-Object { $_.IPAddress }).IPAddress
    Write-Host "Resolved IP(s): $($ips -join ', ')"
} catch {
    Write-Host "DNS FAILED: $($_.Exception.Message)" -ForegroundColor Red
    return
}

Line "2. TCP connectivity: ${targetHost}:${port}"
$tcp = Test-NetConnection -ComputerName $targetHost -Port $port -WarningAction SilentlyContinue
Write-Host ("TcpTestSucceeded: {0}  RemoteAddress: {1}" -f $tcp.TcpTestSucceeded, $tcp.RemoteAddress)
if (-not $tcp.TcpTestSucceeded) {
    Write-Host "Cannot reach the endpoint. Confirm VPN / VPC route / firewall egress." -ForegroundColor Red
    return
}

Line "3. TLS handshake"
try {
    $client = [System.Net.Sockets.TcpClient]::new($targetHost, $port)
    $ssl = [System.Net.Security.SslStream]::new($client.GetStream(), $false, { $true })
    $ssl.AuthenticateAsClient($targetHost)
    Write-Host ("TLS OK  Protocol: {0}  Cipher: {1}" -f $ssl.SslProtocol, $ssl.CipherAlgorithm)
    if ($ssl.RemoteCertificate) {
        $cert = [System.Security.Cryptography.X509Certificates.X509Certificate2]$ssl.RemoteCertificate
        Write-Host ("Server cert: {0}  (expires {1})" -f $cert.Subject, $cert.NotAfter)
    }
    $ssl.Dispose(); $client.Dispose()
} catch {
    Write-Host "TLS FAILED: $($_.Exception.Message)" -ForegroundColor Yellow
}

if (-not $ApiKey) {
    Line "4. Authenticated call: SKIPPED (no -ApiKey supplied)"
    Write-Host "Connectivity looks reachable. Re-run with -ApiKey to test auth + contract."
    return
}

Line "4. Authenticated POST $UploadPath"
$payload = @{
    route = @{
        domain = "prov-enroll"; apiVer = 1.5; tenantID = "Indiana"
        appClient = "provider_portal"; requestGUID = [guid]::NewGuid().ToString()
        requestType = "document_upload"; timeStampId = (Get-Date -Format "yyMMddHHmmssfff")
    }
    enrollment = @{
        trackingID = "TRK-CONN-TEST"
        providerInformation = @{
            npi = "1269201798"
            businessInformation = @{ legalName = "Connectivity Test" }
            attachments = @(@{ fileId = "00000000-0000-0000-0000-000000000001"; fileName = "test.pdf"; attachmentTypeCode = "Federal"; attachmentTypeName = "Federal W-9 Form" })
        }
    }
} | ConvertTo-Json -Depth 8

$headers = @{ "X-API-Key" = $ApiKey; "X-Source" = $Source }
try {
    $resp = Invoke-WebRequest -Uri ($BaseUrl.TrimEnd('/') + $UploadPath) -Method Post `
        -Body $payload -ContentType "application/json" -Headers $headers -TimeoutSec $TimeoutSec
    Write-Host ("HTTP {0}" -f [int]$resp.StatusCode) -ForegroundColor Green
    Write-Host $resp.Content
} catch {
    $r = $_.Exception.Response
    if ($r) {
        Write-Host ("HTTP {0} {1}" -f [int]$r.StatusCode, $r.StatusCode) -ForegroundColor Yellow
        try { $sr = [System.IO.StreamReader]::new($r.GetResponseStream()); Write-Host $sr.ReadToEnd() } catch {}
        Write-Host "401/403 = reachable but auth/permission issue. 200/4xx-with-body = Enso is responding."
    } else {
        Write-Host "REQUEST FAILED: $($_.Exception.Message)" -ForegroundColor Red
    }
}
