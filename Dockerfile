FROM mcr.microsoft.com/dotnet/core/sdk:3.1 AS build

WORKDIR /app
COPY . .
RUN dotnet publish -c Release -o out -r linux-x64 Source/Runners/Runner.Console/Runner.Console.csproj

FROM mcr.microsoft.com/dotnet/core/runtime:3.1 AS runtime
WORKDIR /app
COPY --from=build /app/out ./

ENTRYPOINT ["dotnet", "/app/PierogiesBot.Console.dll"]
