#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 8080
# HEALTHCHECK CMD curl --fail http://localhost:8080 || exit 1

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["src/ReverseProxy/ReverseProxy.csproj", "src/ReverseProxy/"]
COPY ["src/Libs/N8T.Infrastructure/N8T.Infrastructure.csproj", "src/Libs/N8T.Infrastructure/"]
COPY ["src/Libs/N8T.Core/N8T.Core.csproj", "src/Libs/N8T.Core/"]
RUN dotnet restore "src/ReverseProxy/ReverseProxy.csproj"
COPY . .
WORKDIR "/src/src/ReverseProxy"
# RUN dotnet build "ReverseProxy.csproj" -c Release -o /app/build -r linux-x64 /p:PublishReadyToRun=true
RUN dotnet build "ReverseProxy.csproj" -c Release -o /app/build

FROM build AS publish
# RUN dotnet publish "ReverseProxy.csproj" -c Release -o /app/publish -r linux-x64 --self-contained true --no-restore /p:PublishTrimmed=true /p:PublishReadyToRun=true /p:PublishSingleFile=true
RUN dotnet publish "ReverseProxy.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
# CMD ["./ReverseProxy"]
ENTRYPOINT ["dotnet", "ReverseProxy.dll"]