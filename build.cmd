@ECHO OFF

dotnet tool restore
dotnet build -- %*

AddToPath .\ConditionalStartup\bin\Debug\
