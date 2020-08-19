# SteamConsoleUploader
A C# Console to run SteamSDK and autoupload a build to Steam Store. (Teamcity) 

# Use cases

run console passing a file with content:

```
system.umake.steam.buildscript=<VDF Steam Script Path>

system.umake.steam.login=<Your Steam Login>

system.umake.steam.password=<Your Steam password>

system.umake.steam.sdkpath=<Steam SDK Path>

system.umake.steam.subfoldercontent=<The relative to build path where the program will copy the build files to upload to steam>

system.umake.buildpath=<Game Build path>
```

REMEMBER: you need run first the CMD of steam, to try logout and save the Steam Guard access.
