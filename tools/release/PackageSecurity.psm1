Set-StrictMode -Version Latest

function Get-PackagedSecretPatterns {
    return @(
        [pscustomobject]@{
            Name = "private key"
            Pattern = '-----BEGIN (?:RSA |EC |OPENSSH |DSA )?PRIVATE KEY-----'
        },
        [pscustomobject]@{
            Name = "GitHub token"
            Pattern = '(?<![A-Za-z0-9])(?:ghp_[A-Za-z0-9]{20,}|github_pat_[A-Za-z0-9_]{20,})'
        },
        [pscustomobject]@{
            Name = "AWS access key"
            Pattern = '(?<![A-Z0-9])(?:AKIA|ASIA)[A-Z0-9]{16}(?![A-Z0-9])'
        },
        [pscustomobject]@{
            Name = "AWS secret key"
            Pattern = '(?i)aws_(?:secret_access_key|session_token)\s*[=:]\s*[A-Za-z0-9/+=]{20,}'
        },
        [pscustomobject]@{
            Name = "JWT"
            Pattern = '(?<![A-Za-z0-9_-])eyJ[A-Za-z0-9_-]{10,}\.[A-Za-z0-9_-]{10,}\.[A-Za-z0-9_-]{10,}(?![A-Za-z0-9_-])'
        },
        [pscustomobject]@{
            Name = "service token"
            Pattern = '(?<![A-Za-z0-9])(?:xox[baprs]-[A-Za-z0-9-]{20,}|sk_live_[A-Za-z0-9]{16,}|AIza[0-9A-Za-z_-]{30,})(?![A-Za-z0-9])'
        },
        [pscustomobject]@{
            Name = "connection string credential"
            Pattern = '(?i)(?:Password|Pwd|AccountKey|SharedAccessKey|SharedAccessSignature|ClientSecret)\s*=\s*(?:"(?!\*{3,}"|<[^>]+>"|\$\{[^}]+\}")[^"\r\n]{4,}"|''(?!\*{3,}''|<[^>]+>''|\$\{[^}]+\}'')[^''\r\n]{4,}''|(?!\*{3,}|<[^>]+>|\$\{[^}]+\})[^;\s"'']{4,})'
        }
    )
}

function Test-SecretText {
    param(
        [Parameter(Mandatory)]
        [string]$Text,

        [Parameter(Mandatory)]
        [string]$RelativePath,

        [Parameter(Mandatory)]
        [object[]]$Patterns
    )

    foreach ($secretPattern in $Patterns) {
        if ([regex]::IsMatch(
            $Text,
            $secretPattern.Pattern,
            [Text.RegularExpressions.RegexOptions]::CultureInvariant,
            [TimeSpan]::FromSeconds(2)
        )) {
            throw "Package contains a probable $($secretPattern.Name) in '$RelativePath'."
        }
    }
}

function Test-PackagedSecrets {
    [CmdletBinding()]
    param(
        [Parameter(Mandatory)]
        [string]$PackageDirectory
    )

    $packageRoot = (Resolve-Path -LiteralPath $PackageDirectory).Path
    $packagePrefix = $packageRoot.TrimEnd(
        [IO.Path]::DirectorySeparatorChar,
        [IO.Path]::AltDirectorySeparatorChar
    ) + [IO.Path]::DirectorySeparatorChar
    $patterns = @(Get-PackagedSecretPatterns)
    $chunkSize = 1MB
    $overlapSize = 4KB

    foreach ($file in Get-ChildItem -LiteralPath $packageRoot -File -Recurse) {
        $relativePath = $file.FullName.Substring($packagePrefix.Length)
        $stream = [IO.File]::Open($file.FullName, [IO.FileMode]::Open, [IO.FileAccess]::Read, [IO.FileShare]::Read)
        try {
            $buffer = [byte[]]::new($chunkSize)
            $overlap = [byte[]]::new(0)
            while (($read = $stream.Read($buffer, 0, $buffer.Length)) -gt 0) {
                $combined = [byte[]]::new($overlap.Length + $read)
                if ($overlap.Length -gt 0) {
                    [Array]::Copy($overlap, 0, $combined, 0, $overlap.Length)
                }
                [Array]::Copy($buffer, 0, $combined, $overlap.Length, $read)

                Test-SecretText `
                    -Text ([Text.Encoding]::ASCII.GetString($combined)) `
                    -RelativePath $relativePath `
                    -Patterns $patterns
                Test-SecretText `
                    -Text ([Text.Encoding]::Unicode.GetString($combined)) `
                    -RelativePath $relativePath `
                    -Patterns $patterns
                Test-SecretText `
                    -Text ([Text.Encoding]::BigEndianUnicode.GetString($combined)) `
                    -RelativePath $relativePath `
                    -Patterns $patterns

                $overlapLength = [Math]::Min($overlapSize, $combined.Length)
                $overlap = [byte[]]::new($overlapLength)
                [Array]::Copy(
                    $combined,
                    $combined.Length - $overlapLength,
                    $overlap,
                    0,
                    $overlapLength
                )
            }
        }
        finally {
            $stream.Dispose()
        }
    }
}

Export-ModuleMember -Function "Test-PackagedSecrets"
