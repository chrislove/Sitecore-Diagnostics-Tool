# get params
[CmdletBinding()]
Param(
    [Parameter(Mandatory=$True)] [string]$BuildFolder,
    [Parameter(Mandatory=$True)] [string]$ProjectFile,
    [Parameter(Mandatory=$True)] [string]$BuildOptions
)

# normalize paths
$ProjectFile =  [System.IO.Path]::GetFullPath($ProjectFile)
$BuildFolder = [System.IO.Path]::GetFullPath($BuildFolder)

# restore nuget packages
DIR ..\src *.sln -Recurse |
  ForEach-Object { 
    Write-Host "> tools\nuget.exe restore `"$_`" -Wait -RedirectStandardOutput stdout.txt -RedirectStandardError stderr.txt"
    Start-Process -FilePath "tools\nuget.exe" -ArgumentList "restore `"$_`"" -Wait -RedirectStandardOutput stdout.txt -RedirectStandardError stderr.txt
    Get-Content stdout.txt | Write-Host
    Get-Content stderr.txt | Write-Host
  }

Write-Host "> Done. Restored NuGet packages"

# clean up files
"" | Set-Content "stderr.txt"
"" | Set-Content "stdout.txt"

# do build
Write-Host "> tools\MSBuild.exe `"$ProjectFile`" $BuildOptions /p:OutputPath=`"$BuildFolder`""
Start-Process -FilePath "tools\MSBuild.exe" -ArgumentList "`"$ProjectFile`" /p:PlatformTarget=x86 /consoleloggerparameters:ErrorsOnly /p:OutputPath=`"$BuildFolder`""  -Wait -RedirectStandardOutput stdout.txt -RedirectStandardError stderr.txt
Get-Content stdout.txt | Write-Host
Get-Content stderr.txt | Write-Host

Write-Host "> Done. Compiled solution to [$BuildFolder] folder"
Write-Host ""