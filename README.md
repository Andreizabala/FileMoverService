# FileMoverService

A Windows Service that monitors `C:\Folder1` and moves new files automatically to `C:\Folder2`.

---

## How to Publish

Run this in your project directory or Visual Studio Developer Command Prompt:

```
dotnet publish -c Release -r win-x64 --self-contained false /p:PublishSingleFile=true
```
## How to Install Service Manually
Open PowerShell or Command Prompt as Administrator and run:

```
sc create FileMoverService binPath= "C:\path\to\publish\FileMoverService.exe" start= auto obj= "NT AUTHORITY\LocalService" // Create Service

sc start FileMoverService // Start Service

sc query FileMoverService // Stop Service
