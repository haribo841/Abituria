[CmdletBinding()]
param(
    [Parameter(Mandatory)]
    [string]$SiteDirectory,

    [string]$ProjectPath = "Abituria",

    [string]$ExternalLinkPolicyPath,

    [switch]$CheckExternalLinks,

    [ValidateSet("Warn", "Fail")]
    [string]$ExternalLinkFailureAction = "Warn"
)

Set-StrictMode -Version Latest
$ErrorActionPreference = "Stop"

$siteRoot = (Resolve-Path -LiteralPath $SiteDirectory).Path
$sitePrefix = $siteRoot.TrimEnd([IO.Path]::DirectorySeparatorChar, [IO.Path]::AltDirectorySeparatorChar) +
    [IO.Path]::DirectorySeparatorChar
if (-not $ExternalLinkPolicyPath) {
    $ExternalLinkPolicyPath = Join-Path $PSScriptRoot "external-links-policy.json"
}
$policyPath = (Resolve-Path -LiteralPath $ExternalLinkPolicyPath).Path
$policy = Get-Content -LiteralPath $policyPath -Raw -Encoding UTF8 | ConvertFrom-Json
if ($policy.schemaVersion -ne 1 -or $policy.retryCount -lt 1 -or $policy.retryCount -gt 5 -or
    $policy.timeoutSeconds -lt 1 -or $policy.timeoutSeconds -gt 60) {
    throw "External link policy '$policyPath' has invalid version, retry count or timeout."
}

$allowedHosts = @{}
foreach ($hostName in @($policy.allowedHosts)) {
    $normalizedHost = ([string]$hostName).Trim().ToLowerInvariant()
    if (-not $normalizedHost -or $normalizedHost -notmatch '^[a-z0-9.-]+$' -or
        $allowedHosts.ContainsKey($normalizedHost)) {
        throw "External link policy contains an invalid or duplicate host '$hostName'."
    }
    $allowedHosts[$normalizedHost] = $true
}

$onlineCheckExclusions = @($policy.onlineCheckExclusions)
$localRepositoryBlobPrefix = [string]$policy.localRepositoryBlobPrefix
if (-not $localRepositoryBlobPrefix.StartsWith("https://github.com/haribo841/Abituria/blob/main/", [StringComparison]::Ordinal)) {
    throw "External link policy must identify the canonical main-branch blob prefix."
}
foreach ($exclusion in $onlineCheckExclusions) {
    $prefix = [string]$exclusion.urlPrefix
    $reason = [string]$exclusion.reason
    if (-not $prefix.StartsWith("https://", [StringComparison]::Ordinal) -or -not $reason.Trim()) {
        throw "Every online-check exclusion must contain an HTTPS prefix and a non-empty reason."
    }
}

$missingTargets = [Collections.Generic.List[string]]::new()
$externalReferences = @{}
$repositoryRoot = (Resolve-Path (Join-Path $PSScriptRoot "../..")).Path
$repositoryPrefix = $repositoryRoot.TrimEnd(
    [IO.Path]::DirectorySeparatorChar,
    [IO.Path]::AltDirectorySeparatorChar
) + [IO.Path]::DirectorySeparatorChar
$pathComparison = if ([Runtime.InteropServices.RuntimeInformation]::IsOSPlatform(
    [Runtime.InteropServices.OSPlatform]::Windows
)) {
    [StringComparison]::OrdinalIgnoreCase
}
else {
    [StringComparison]::Ordinal
}
$attributePattern = [regex]::new(
    '\b(?:href|src)\s*=\s*["'']([^"'']+)["'']',
    [Text.RegularExpressions.RegexOptions]::IgnoreCase
)

foreach ($htmlFile in Get-ChildItem -LiteralPath $siteRoot -Filter "*.html" -File -Recurse) {
    $html = Get-Content -LiteralPath $htmlFile.FullName -Raw -Encoding UTF8
    foreach ($match in $attributePattern.Matches($html)) {
        $reference = [Net.WebUtility]::HtmlDecode($match.Groups[1].Value.Trim())
        if (-not $reference -or $reference.StartsWith('#')) {
            continue
        }

        if ($reference.StartsWith('//')) {
            throw "Protocol-relative external link is not allowed: '$reference' in '$($htmlFile.FullName)'."
        }

        $absoluteUri = $null
        if ($reference -match '^[A-Za-z][A-Za-z0-9+.-]*:') {
            if (-not [Uri]::TryCreate($reference, [UriKind]::Absolute, [ref]$absoluteUri)) {
                throw "Invalid absolute link '$reference' in '$($htmlFile.FullName)'."
            }
            if ($absoluteUri.Scheme -eq [Uri]::UriSchemeMailto) {
                continue
            }
            if ($absoluteUri.Scheme -ne [Uri]::UriSchemeHttps) {
                throw "External link must use HTTPS: '$reference' in '$($htmlFile.FullName)'."
            }

            $hostName = $absoluteUri.IdnHost.ToLowerInvariant()
            if (-not $allowedHosts.ContainsKey($hostName)) {
                throw "External link host '$hostName' is not present in '$policyPath'."
            }

            $builder = [UriBuilder]::new($absoluteUri)
            $builder.Fragment = ""
            if ($builder.Uri.AbsoluteUri.StartsWith($localRepositoryBlobPrefix, [StringComparison]::Ordinal)) {
                $relativeRepositoryPath = [Uri]::UnescapeDataString(
                    $builder.Uri.AbsoluteUri.Substring($localRepositoryBlobPrefix.Length)
                ).TrimEnd('/').Replace('/', [IO.Path]::DirectorySeparatorChar)
                $candidateRepositoryPath = [IO.Path]::GetFullPath(
                    (Join-Path $repositoryRoot $relativeRepositoryPath)
                )
                if (-not $candidateRepositoryPath.StartsWith($repositoryPrefix, $pathComparison) -or
                    -not (Test-Path -LiteralPath $candidateRepositoryPath -PathType Leaf)) {
                    throw "Same-repository source link does not resolve to a local source file: '$reference'."
                }
                continue
            }
            $externalReferences[$builder.Uri.AbsoluteUri] = $true
            continue
        }

        $pathPart = ($reference -split '[?#]', 2)[0]
        if (-not $pathPart) {
            continue
        }

        $pathPart = [Uri]::UnescapeDataString($pathPart).Replace('/', [IO.Path]::DirectorySeparatorChar)
        if ($pathPart.StartsWith([IO.Path]::DirectorySeparatorChar)) {
            $pathPart = $pathPart.TrimStart([IO.Path]::DirectorySeparatorChar)
            $projectPrefix = $ProjectPath.Trim('/') + [IO.Path]::DirectorySeparatorChar
            if ($pathPart.StartsWith($projectPrefix, [StringComparison]::OrdinalIgnoreCase)) {
                $pathPart = $pathPart.Substring($projectPrefix.Length)
            }
            $candidatePath = Join-Path $siteRoot $pathPart
        }
        else {
            $candidatePath = Join-Path $htmlFile.DirectoryName $pathPart
        }

        if ($reference -match '/(?:[?#].*)?$') {
            $candidatePath = Join-Path $candidatePath "index.html"
        }

        $candidateFullPath = [IO.Path]::GetFullPath($candidatePath)
        if (Test-Path -LiteralPath $candidateFullPath -PathType Container) {
            $candidateFullPath = Join-Path $candidateFullPath "index.html"
        }
        if (-not $candidateFullPath.StartsWith($sitePrefix, [StringComparison]::OrdinalIgnoreCase)) {
            throw "Generated link escapes the Pages site: '$reference' in '$($htmlFile.FullName)'."
        }

        if (-not (Test-Path -LiteralPath $candidateFullPath)) {
            $relativeHtml = $htmlFile.FullName.Substring($sitePrefix.Length)
            $missingTargets.Add("$relativeHtml -> $reference")
        }
    }
}

if ($missingTargets.Count -ne 0) {
    throw "Generated documentation contains missing local targets:`n$($missingTargets -join "`n")"
}

Write-Host "All generated local href/src targets resolve and external links satisfy the deterministic policy."

if (-not $CheckExternalLinks) {
    return
}

Add-Type -AssemblyName System.Net.Http
$handler = [Net.Http.HttpClientHandler]::new()
$handler.AllowAutoRedirect = $true
$handler.MaxAutomaticRedirections = 5
if ($handler.PSObject.Properties.Name -contains "MaxConnectionsPerServer") {
    $handler.MaxConnectionsPerServer = 8
}
$client = [Net.Http.HttpClient]::new($handler)
$client.Timeout = [TimeSpan]::FromSeconds([int]$policy.timeoutSeconds)
$client.DefaultRequestHeaders.UserAgent.ParseAdd("Abituria-release-link-check/1.0")
$failedLinks = [Collections.Generic.List[string]]::new()
$pendingLinks = @{}
$lastResults = @{}
try {
    foreach ($uri in @($externalReferences.Keys | Sort-Object)) {
        $excluded = $false
        foreach ($exclusion in $onlineCheckExclusions) {
            if ($uri.StartsWith([string]$exclusion.urlPrefix, [StringComparison]::Ordinal)) {
                Write-Host "Skipping online probe for '$uri': $($exclusion.reason)"
                $excluded = $true
                break
            }
        }
        if ($excluded) {
            continue
        }
        $pendingLinks[$uri] = $true
        $lastResults[$uri] = "no response"
    }

    for ($attempt = 1; $attempt -le [int]$policy.retryCount -and $pendingLinks.Count -gt 0; $attempt++) {
        $pendingUris = @($pendingLinks.Keys | Sort-Object)
        for ($offset = 0; $offset -lt $pendingUris.Count; $offset += 8) {
            $batch = @($pendingUris | Select-Object -Skip $offset -First 8)
            $probes = @(
                foreach ($uri in $batch) {
                    $request = [Net.Http.HttpRequestMessage]::new([Net.Http.HttpMethod]::Get, $uri)
                    [pscustomobject]@{
                        Uri = $uri
                        Request = $request
                        Task = $client.SendAsync(
                            $request,
                            [Net.Http.HttpCompletionOption]::ResponseHeadersRead
                        )
                    }
                }
            )

            foreach ($probe in $probes) {
                $succeeded = $false
                try {
                    $response = $probe.Task.GetAwaiter().GetResult()
                    try {
                        $statusCode = [int]$response.StatusCode
                        $lastResults[$probe.Uri] = "HTTP $statusCode"
                        $succeeded = $statusCode -ge 200 -and $statusCode -lt 400
                    }
                    finally {
                        $response.Dispose()
                    }
                }
                catch {
                    $lastResults[$probe.Uri] = $_.Exception.GetBaseException().Message
                }
                finally {
                    $probe.Request.Dispose()
                    $probe.Task.Dispose()
                }

                if ($succeeded) {
                    $null = $pendingLinks.Remove($probe.Uri)
                    Write-Host "External link is reachable: $($probe.Uri)"
                }
            }
        }

        if ($pendingLinks.Count -gt 0 -and $attempt -lt [int]$policy.retryCount) {
            Start-Sleep -Seconds ([Math]::Min(4, [Math]::Pow(2, $attempt - 1)))
        }
    }

    foreach ($uri in @($pendingLinks.Keys | Sort-Object)) {
        $failedLinks.Add("$uri ($($lastResults[$uri]) after $($policy.retryCount) attempts)")
    }
}
finally {
    $client.Dispose()
    $handler.Dispose()
}

if ($failedLinks.Count -ne 0) {
    $message = "External links were unavailable after retries:`n$($failedLinks -join "`n")"
    if ($ExternalLinkFailureAction -eq "Fail") {
        throw $message
    }
    Write-Warning $message
}
else {
    Write-Host "All non-excluded external links responded successfully."
}
