#!/usr/local/bin/powershell
$buildPath = "./build"
$testResultsPath = "./TestResults"
If (Test-Path $buildPath)
{ Remove-Item -Confirm -Recurse -Path $buildPath }
If (Test-Path $testResultsPath)
{ Remove-Item -Confirm -Recurse -Path  $testResultsPath }