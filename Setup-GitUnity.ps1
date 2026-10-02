param(
    [string]$PersonalBranch = "dev/jakub",
    [string]$RemoteUrl = ""
)

$ErrorActionPreference = "Stop"

function Require-Command {
    param([string]$Name)
    if (-not (Get-Command $Name -ErrorAction SilentlyContinue)) {
        throw "Required command not found: $Name"
    }
}

function Run-Git {
    param([Parameter(ValueFromRemainingArguments = $true)][string[]]$Args)
    & git @Args
    if ($LASTEXITCODE -ne 0) {
        throw "Git command failed: git $($Args -join ' ')"
    }
}

Require-Command "git"

Write-Host "== Unity Git setup ==" -ForegroundColor Cyan

if (-not (Test-Path ".git")) {
    Run-Git init -b main
} else {
    Write-Host "Git repository already exists; skipping git init." -ForegroundColor Yellow
}

Run-Git config core.autocrlf false
Run-Git config fetch.prune true
Run-Git config pull.ff only

& git lfs version *> $null
if ($LASTEXITCODE -ne 0) {
    throw "Git LFS is not installed. Install Git LFS and run this script again."
}
Run-Git lfs install

$folders = @(
    "Assets/_Project",
    "Assets/_Local",
    "ArtSource",
    "ArtSource/_Local",
    "ThirdParty",
    "Docs"
)

foreach ($folder in $folders) {
    New-Item -ItemType Directory -Force -Path $folder | Out-Null
}

$userName = & git config user.name
$userEmail = & git config user.email

if ([string]::IsNullOrWhiteSpace($userName)) {
    Write-Host 'Git user.name is not configured. Run: git config --global user.name "First Last"' -ForegroundColor Yellow
}
if ([string]::IsNullOrWhiteSpace($userEmail)) {
    Write-Host 'Git user.email is not configured. Run: git config --global user.email "email@example.com"' -ForegroundColor Yellow
}

& git rev-parse --verify HEAD *> $null
$hasHead = ($LASTEXITCODE -eq 0)

if (-not $hasHead) {
    if ([string]::IsNullOrWhiteSpace($userName) -or [string]::IsNullOrWhiteSpace($userEmail)) {
        throw "Configure git user.name and user.email before creating the initial commit."
    }

    Run-Git add .
    Run-Git commit -m "chore: initialize Unity repository"
}

$branches = @(& git branch --format="%(refname:short)")

if ($branches -notcontains "main") {
    throw "Branch 'main' was not found."
}
if ($branches -notcontains "develop") {
    Run-Git branch develop main
}
if ($branches -notcontains $PersonalBranch) {
    Run-Git branch $PersonalBranch develop
}

Run-Git checkout $PersonalBranch

if (-not [string]::IsNullOrWhiteSpace($RemoteUrl)) {
    $remotes = @(& git remote)
    if ($remotes -notcontains "origin") {
        Run-Git remote add origin $RemoteUrl
    } else {
        Run-Git remote set-url origin $RemoteUrl
    }

    Run-Git push -u origin main
    Run-Git push -u origin develop
    Run-Git push -u origin $PersonalBranch
}

Write-Host ""
Write-Host "Done. Active branch: $PersonalBranch" -ForegroundColor Green
Write-Host "Next: create dev/<name> branches for teammates and protect main/develop on GitHub." -ForegroundColor Green
