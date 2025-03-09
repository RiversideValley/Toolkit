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
    # Check in src\extensions if not found in src\platforms
    $ProjectDirectory = Join-Path -Path $ProjectRoot -ChildPath "src\extensions\$Project"
    if (-Not (Test-Path -Path $ProjectDirectory)) {
        Write-Output "Project directory '$ProjectDirectory' does not exist, skipping build."
        exit 0
    }
}

# Navigate to the project directory
Set-Location -Path $ProjectDirectory

# Run MSBuild
msbuild /t:Restore /p:Configuration=$Configuration
msbuild /t:Build /p:Configuration=$Configuration /p:Platform=x64

# Return to the original location
Pop-Location