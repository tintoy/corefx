[CmdletBinding()]
param (
    [string]$CMakeVersion = "3.7",
    [string]$CMakeBinaryFolder = (Join-Path $env:TEMP "CMake")
)

$CMakePath = $null
try {
    #$CMakeCompressedBinary = "cmake-$CMakeVersion-"
    $CMakeCompressedBinary = "cmake-$CMakeVersion.1-"

    if ([Environment]::Is64BitOperatingSystem) {
        $CMakeCompressedBinary += "win64-x64.zip"
    } else {
        $CMakeCompressedBinary = "win32-x86.zip"
    }

    # Example URL is https://cmake.org/files/v3.7/cmake-3.7.1-win64-x64.zip
    $CMakeDownloadUrl = "https://cmake.org/files/v$CMakeVersion/$CMakeCompressedBinary"
    #Write-Host "Attempting to download CMake from $CMakeDownloadUrl"

    # If none specified then, create a folder to download and extract CMake binary.
    $CMakeCompressedBinaryPath = Join-Path $CMakeBinaryFolder $CMakeCompressedBinary
    if (Test-Path -Path $CMakeBinaryFolder) {
        Remove-Item -Path $CMakeBinaryFolder -Recurse -Force
    }
    New-Item -Path $CMakeBinaryFolder -ItemType directory | Out-Null

    # Download the binary file.
    $webclient = New-Object System.Net.WebClient
    $webclient.DownloadFile($CMakeDownloadUrl,$CMakeCompressedBinaryPath)

    #Write-Host "Download complete. Attempting to extract the downloaded binary."
    Expand-Archive -Path $CMakeCompressedBinaryPath -DestinationPath $CMakeBinaryFolder -Force

    # Remove the downloaded compressed binary file.
    Remove-Item $CMakeCompressedBinaryPath -Force -ErrorAction Continue

    $CMakeFolderName = [System.IO.Path]::GetFileNameWithoutExtension($CMakeCompressedBinaryPath)
    $CMakePath = [System.IO.Path]::Combine($CMakeBinaryFolder, $CMakeFolderName, "bin\cmake.exe")
    #Write-Host $CMakePath

} catch {
    #Write-Warning $_.Exception.ToString()
}

return $CMakePath