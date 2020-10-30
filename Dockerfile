FROM mcr.microsoft.com/dotnet/core/sdk:3.1 AS build

WORKDIR /app
COPY . .
RUN dotnet publish -c Release -o out --self-contained true -r linux-x64 Source/Runners/PierogiesBot.Runners.Console/PierogiesBot.Runners.Console.csproj
#RUN MSBuild ./PierogiesBot.Host/PierogiesBot.Host.csproj /p:Configuration=Release /p:Platform=AnyCPU /p:RuntimeIdentifier=linux-arm /p:OutputPath=out /t:publish

FROM mcr.microsoft.com/dotnet/core/runtime:3.1 AS runtime
WORKDIR /app
COPY --from=build /app/out ./
RUN chmod +x ./Runner.Console

ENTRYPOINT ["./Runner.Console"]
