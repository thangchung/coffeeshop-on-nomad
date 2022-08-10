#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/nightly/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 5001
EXPOSE 15001

FROM mcr.microsoft.com/dotnet/nightly/sdk:7.0 AS build
WORKDIR /src
COPY ["src/ProductService/ProductService.csproj", "src/ProductService/"]
COPY ["src/Libs/N8T.Core/N8T.Core.csproj", "src/Libs/N8T.Core/"]
COPY ["src/Libs/N8T.Infrastructure.EfCore/N8T.Infrastructure.EfCore.csproj", "src/Libs/N8T.Infrastructure.EfCore/"]
COPY ["src/Libs/N8T.Infrastructure/N8T.Infrastructure.csproj", "src/Libs/N8T.Infrastructure/"]
RUN dotnet restore "src/ProductService/ProductService.csproj"
COPY . .
WORKDIR "/src/src/ProductService"
RUN dotnet build "ProductService.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "ProductService.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "ProductService.dll"]