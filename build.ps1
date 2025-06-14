$ErrorActionPreference = "Stop"

dotnet tool restore
dotnet build

AddToPath .\ConditionalStartup\bin\Debug\
