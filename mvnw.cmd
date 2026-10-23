@echo off
setlocal
set "MAVEN_VERSION=3.9.16"
where mvn >nul 2>&1
if not errorlevel 1 (
  mvn %*
  exit /b %ERRORLEVEL%
)
set "MAVEN_HOME=%USERPROFILE%\.m2\wrapper\dists\apache-maven-%MAVEN_VERSION%"
if exist "%MAVEN_HOME%\bin\mvn.cmd" goto run_maven
where powershell.exe >nul 2>&1
if errorlevel 1 (
  echo ERROR: Maven is not installed and PowerShell is unavailable to download it.
  exit /b 1
)
echo Maven %MAVEN_VERSION% is not installed. Downloading Maven automatically...
powershell.exe -NoProfile -ExecutionPolicy Bypass -Command "$ErrorActionPreference='Stop'; [Net.ServicePointManager]::SecurityProtocol=[Net.SecurityProtocolType]::Tls12; $v='%MAVEN_VERSION%'; $base=Join-Path $env:USERPROFILE '.m2\wrapper\dists'; $home=Join-Path $base ('apache-maven-'+$v); if (!(Test-Path (Join-Path $home 'bin\mvn.cmd'))) { New-Item -ItemType Directory -Force -Path $base | Out-Null; $zip=Join-Path $env:TEMP ('apache-maven-'+$v+'-bin.zip'); $url='https://dlcdn.apache.org/maven/maven-3/'+$v+'/binaries/apache-maven-'+$v+'-bin.zip'; Invoke-WebRequest -UseBasicParsing -Uri $url -OutFile $zip; Expand-Archive -LiteralPath $zip -DestinationPath $base -Force; Remove-Item -LiteralPath $zip -Force }"
if errorlevel 1 (
  echo ERROR: Could not download Maven. Check your Internet connection, or install Maven manually.
  exit /b 1
)
:run_maven
"%MAVEN_HOME%\bin\mvn.cmd" %*
exit /b %ERRORLEVEL%
