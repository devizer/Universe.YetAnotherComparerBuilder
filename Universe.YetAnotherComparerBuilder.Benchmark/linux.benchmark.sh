#!/usr/bin/env bash
# work=$HOME/build/devizer; mkdir -p $work; cd $work; git clone https://github.com/devizer/Universe.YetAnotherComparerBuilder; cd Universe.YetAnotherComparerBuilder; git pull; cd Universe.YetAnotherComparerBuilder.Benchmark; bash linux.benchmark.sh
dotnet build -c Release -v q || true
msbuild /t:Rebuild /p:Configuration=Release /v:q
rm -f bin/Release/net462/Universe.YetAnotherComparerBuilder.Benchmark.exe.config
dotnet benchmark bin/Release/net462/Universe.YetAnotherComparerBuilder.Benchmark.exe -r netcoreapp2.2 Mono -filter * -j short
