# Building project instruction

Windows:

```pwsh
dotnet build ".\WonderSongs.Uno\WonderSongs.Uno.csproj" -f:net10.0-windows10.0.26100 -c:Debug -clp:NoSummary -p:GenerateFullPaths=true -p:UnoForceSingleTFM=true -bl:".\WonderSongs.Uno\net10.0-windows10.0.26100-Debug-win-x64.binlog" 
```