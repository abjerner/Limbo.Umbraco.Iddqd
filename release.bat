@echo off
dotnet build src/Limbo.Umbraco.Iddqd --configuration Release /t:rebuild /t:pack -p:PackageOutputPath=../../releases/nuget