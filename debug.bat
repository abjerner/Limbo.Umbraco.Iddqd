@echo off
dotnet build src/Limbo.Umbraco.Iddqd --configuration Debug /t:rebuild /t:pack -p:PackageOutputPath=c:/nuget