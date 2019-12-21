# work=$HOME/build/devizer; mkdir -p $work; cd $work; git clone https://github.com/devizer/Universe.YetAnotherComparerBuilder; cd Universe.YetAnotherComparerBuilder; git pull; cd Universe.YetAnotherComparerBuilder.Benchmark; bash linux.benchmark.sh
dotnet build -c Release -v q || true
msbuild /t:Rebuild /p:Configuration=Release /v:q
dotnet benchmark bin/Release/net462/Universe.YetAnotherComparerBuilder.Benchmark.dll -r netcoreapp2.2 Mono -filter * -j short
