dotnet build -c Release
dotnet benchmark bin\Release\netcoreapp2.2\Universe.YetAnotherComparerBuilder.Benchmark.dll -r netcoreapp2.2 net462 -filter * -j short