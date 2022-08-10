#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/nightly/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 5002

FROM mcr.microsoft.com/dotnet/nightly/sdk:7.0 AS build
WORKDIR /src
COPY ["src/CounterService/CounterService.csproj", "src/CounterService/"]
COPY ["src/Libs/N8T.Core/N8T.Core.csproj", "src/Libs/N8T.Core/"]
COPY ["src/Libs/N8T.Infrastructure.EfCore/N8T.Infrastructure.EfCore.csproj", "src/Libs/N8T.Infrastructure.EfCore/"]
COPY ["src/Libs/N8T.Infrastructure/N8T.Infrastructure.csproj", "src/Libs/N8T.Infrastructure/"]
COPY ["src/Libs/CoffeeShop.Contracts/CoffeeShop.Contracts.csproj", "src/Libs/CoffeeShop.Contracts/"]
RUN dotnet restore "src/CounterService/CounterService.csproj"
COPY . .
WORKDIR "/src/src/CounterService"
RUN dotnet build "CounterService.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "CounterService.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "CounterService.dll"]