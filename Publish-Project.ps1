# Script to Publish the Project for FTP upload
$ProjectDir = "AppQLHui"
$PublishDir = "$ProjectDir\publish"

Write-Host "--- Starting Publish Process ---" -ForegroundColor Cyan

# Clean old publish directory
if (Test-Path $PublishDir) {
    Write-Host "Cleaning old publish files..." -ForegroundColor Gray
    Remove-Item -Path "$PublishDir\*" -Recurse -Force -ErrorAction SilentlyContinue
} else {
    New-Item -ItemType Directory -Path $PublishDir -Force | Out-Null
}

# Run dotnet publish with explicit output directory
Write-Host "Running dotnet publish..." -ForegroundColor Yellow
dotnet publish "$ProjectDir\$ProjectDir.csproj" -c Release -o "$PublishDir"

if ($LASTEXITCODE -eq 0) {
    # Delete unnecessary files for FTP if any
    # e.g., if you want to exclude appsettings.Development.json manually
    if (Test-Path "$PublishDir\appsettings.Development.json") {
        Remove-Item "$PublishDir\appsettings.Development.json" -Force
    }

    Write-Host "`n--- Publish Successful ---" -ForegroundColor Green
    $FullPath = (Get-Item $PublishDir).FullName
    Write-Host "Files are ready for FTP at: $FullPath" -ForegroundColor Green
    Write-Host "Instructions:" -ForegroundColor Yellow
    Write-Host "1. Open your FTP client (e.g., FileZilla)."
    Write-Host "2. Connect to your Mắt Bão hosting."
    Write-Host "3. Upload ALL contents of the '$PublishDir' folder to the server's root."
} else {
    Write-Host "`n--- Publish Failed ---" -ForegroundColor Red
}
