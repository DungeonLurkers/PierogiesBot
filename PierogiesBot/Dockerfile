FROM mcr.microsoft.com/dotnet/aspnet:5.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build
WORKDIR /src
COPY ["PierogiesBot/PierogiesBot.csproj", "PierogiesBot/"]
RUN dotnet restore "PierogiesBot/PierogiesBot.csproj"
COPY . .
WORKDIR "/src/PierogiesBot"
RUN dotnet build "PierogiesBot.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "PierogiesBot.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "PierogiesBot.dll"]
