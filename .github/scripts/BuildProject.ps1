param (
    [string]$Project,
    [string]$Head,
    [string]$Configuration
)

# Calculate the fully qualified project name
if ($Project -eq "Riverside.Extensions.WinUI") {
    $FullyQualifiedName = "Riverside.Extensions.$Head"
} else {
    $FullyQualifiedName = "$($Project -replace '\.Toolkit\.', ".Toolkit.$Head.")"
}

# Define the project directory
$ScriptDirectory = Split-Path -Parent $MyInvocation.MyCommand.Path
$ProjectRoot = Join-Path -Path $ScriptDirectory -ChildPath "..\.."
$ProjectDirectory = Join-Path -Path $ProjectRoot -ChildPath "src\platforms\$FullyQualifiedName"

# Check if the project directory exists
if (-Not (Test-Path -Path $ProjectDirectory)) {
    Write-Output "Project directory '$ProjectDirectory' does not exist, skipping build."
    exit 0
}

msbuild $ProjectDirectory /t:Restore /p:Configuration=$Configuration
# Run MSBuild
msbuild $ProjectDirectory /t:Build /p:Configuration=$Configuration

# Return to the original location
Pop-Location