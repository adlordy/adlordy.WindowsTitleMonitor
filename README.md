# adlordy.WindowsTitleMonitor

## Features
Monitors active foreground window every second and stores this information into the file in the `%APPDATA%\WindowsTitleMonitor` folder.

Every day creates report of the daily file by counting occurences of each file.


## Releases
Go to [Releases](https://github.com/adlordy/adlordy.WindowsTitleMonitor/releases)

## Start in background

`PowerShell> Start-Process .\adlordy.WindowTitleMonitor.exe -WindowStyle Hidden`

To stop use 

`PowerShell> Get-Process adlordy.WindowTitleMonitor | Stop-Process`

