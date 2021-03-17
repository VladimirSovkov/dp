set PathToWebApp=..\Valuator\
set PathToRankCalculator=..\RankCalculator\
set PathToEventsLogger=..\EventsLogger\
set PathToNginx=..\nginx\

start "localhost:5001" /d %PathToWebApp% dotnet run --no-build --urls "http://localhost:5001"
start "localhost:5002" /d %PathToWebApp% dotnet run --no-build --urls "http://localhost:5002"

start "RankCalculator1" /d %PathToRankCalculator% dotnet run --no-build
start "RankCalculator2" /d %PathToRankCalculator% dotnet run --no-build


start "EventsLogger1" /d %PathToEventsLogger% dotnet run --no-build
start "EventsLogger2" /d %PathToEventsLogger% dotnet run --no-build

start "nginx" /d %PathToNginx% nginx.exe


