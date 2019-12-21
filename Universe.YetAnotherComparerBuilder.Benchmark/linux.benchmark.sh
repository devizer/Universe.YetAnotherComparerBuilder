# work=$HOME/build/devizer; mkdir -p $work; cd $work; git clone https://github.com/devizer/Universe.YetAnotherComparerBuilder; cd Universe.YetAnotherComparerBuilder; git pull; cd Universe.YetAnotherComparerBuilder.Benchmark; bash linux.benchmark.sh
dotnet build -c Release
dotnet benchmark bin/Release/netcoreapp2.2/Universe.YetAnotherComparerBuilder.Benchmark.dll -r netcoreapp2.2 Mono -filter * -j short
