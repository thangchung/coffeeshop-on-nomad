#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["NuGet.Config", "."]
COPY ["Directory.Packages.props", "."]
COPY ["src/ReverseProxy/ReverseProxy.csproj", "src/ReverseProxy/"]
COPY ["src/Libs/N8T.Infrastructure/N8T.Infrastructure.csproj", "src/Libs/N8T.Infrastructure/"]
COPY ["src/Libs/N8T.Core/N8T.Core.csproj", "src/Libs/N8T.Core/"]
RUN dotnet restore "src/ReverseProxy/ReverseProxy.csproj"
COPY . .
WORKDIR "/src/src/ReverseProxy"
RUN dotnet build "ReverseProxy.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "ReverseProxy.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "ReverseProxy.dll"]