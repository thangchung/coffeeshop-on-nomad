#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["src/Client/DataGen.csproj", "src/Client/"]
RUN dotnet restore "src/Client/DataGen.csproj"
COPY . .
WORKDIR "/src/src/Client"
RUN dotnet build "DataGen.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "DataGen.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "DataGen.dll"]