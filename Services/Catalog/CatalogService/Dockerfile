#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/core/aspnet:3.1 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/core/sdk:3.1 AS build
WORKDIR /src
COPY ["Services/Catalog/CatalogService/CatalogService.csproj", "Services/Catalog/CatalogService/"]
COPY ["Services/Authorization/Models/Models.csproj", "Services/Authorization/Models/"]
COPY ["Services/Catalog/Catalog.DataAccessLayer/Catalog.DataAccessLayer.csproj", "Services/Catalog/Catalog.DataAccessLayer/"]
COPY ["Services/CommonLibrary/CommonLibrary.csproj", "Services/CommonLibrary/"]
COPY ["Services/Catalog/Catalog.Models/Catalog.Models.csproj", "Services/Catalog/Catalog.Models/"]
COPY ["Services/Catalog/Catalog.ApplicationLogic/Catalog.ApplicationLogic.csproj", "Services/Catalog/Catalog.ApplicationLogic/"]
COPY ["BuildingBlocks/HostingHelpers/HostingHelpers.csproj", "BuildingBlocks/HostingHelpers/"]
COPY ["BuildingBlocks/EventBus/EventBus.csproj", "BuildingBlocks/EventBus/"]

RUN dotnet restore "Services/Catalog/CatalogService/CatalogService.csproj"
COPY . .
WORKDIR "/src/Services/Catalog/CatalogService"
RUN dotnet build "CatalogService.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "CatalogService.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "CatalogService.dll"]