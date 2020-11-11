#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/core/aspnet:3.1 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/core/sdk:3.1 AS build
WORKDIR /src

COPY "StreetThings.WebApps.sln" "StreetThings.WebApps.sln"

COPY ["Services/Authorization/AuthorizationService/AuthorizationService.csproj", "Services/Authorization/AuthorizationService/"]
COPY ["Services/Authorization/ApplicationLogic/ApplicationLogic.csproj", "Services/Authorization/ApplicationLogic/"]
COPY ["Services/Authorization/Models/Models.csproj", "Services/Authorization/Models/"]
COPY ["Services/Authorization/DataAccessLayer/DataAccessLayer.csproj", "Services/Authorization/DataAccessLayer/"]
COPY ["Services/Authorization/Infrastructure/Infrastructure.csproj", "Services/Authorization/Infrastructure/"]
COPY ["Services/Authorization/tests/AuthorizationTestClient/AuthorizationTestClient.csproj", "Services/Authorization/tests/AuthorizationTestClient/"]
COPY ["Services/Authorization/tests/Infrastructure.Test/Infrastructure.Test.csproj", "Services/Authorization/tests/Infrastructure.Test/"]
COPY ["Services/Authorization/tests/Authorization.ApplicationLogic.Test/Authorization.ApplicationLogic.Test.csproj", "Services/Authorization/tests/Authorization.ApplicationLogic.Test/"]
COPY ["Services/Catalog/CatalogService/CatalogService.csproj", "Services/Catalog/CatalogService/"]
COPY ["Services/Catalog/Catalog.Models/Catalog.Models.csproj", "Services/Catalog/Catalog.Models/"]
COPY ["Services/Catalog/Catalog.ApplicationLogic/Catalog.ApplicationLogic.csproj", "Services/Catalog/Catalog.ApplicationLogic/"]
COPY ["Services/Catalog/Catalog.DataAccessLayer/Catalog.DataAccessLayer.csproj", "Services/Catalog/Catalog.DataAccessLayer/"]
COPY ["Services/Catalog/tests/Catalog.ApplicationLogic.Tests/Catalog.ApplicationLogic.Tests.csproj", "Services/Catalog/tests/Catalog.ApplicationLogic.Tests/"]
COPY ["Services/Transaction/Transaction.Service/Transaction.Service.csproj", "Services/Transaction/Transaction.Service/"]
COPY ["Services/Transaction/Transaction.BusinessLogic/Transaction.BusinessLogic.csproj", "Services/Transaction/Transaction.BusinessLogic/"]
COPY ["Services/Transaction/Transaction.DataAccessLayer/Transaction.DataAccessLayer.csproj", "Services/Transaction/Transaction.DataAccessLayer/"]
COPY ["Services/Transaction/Transaction.Models/Transaction.Models.csproj", "Services/Transaction/Transaction.Models/"]
COPY ["Services/Transaction/tests/Transaction.BusinessLogic.Test/Transaction.BusinessLogic.Test.csproj", "Services/Transaction/tests/Transaction.BusinessLogic.Test/"]
COPY ["Gateways/MobileApiGateway/MobileApiGateway.csproj", "Gateways/MobileApiGateway/"]
COPY ["Gateways/AdministrationGateway/AdministrationGateway.csproj", "Gateways/AdministrationGateway/"]
COPY ["BuildingBlocks/HostingHelpers/HostingHelpers.csproj", "BuildingBlocks/HostingHelpers/"]
COPY ["BuildingBlocks/EventBus/EventBus.csproj", "BuildingBlocks/EventBus/"]
COPY ["BuildingBlocks/EventLog.DataAccessLayer/EventLog.DataAccessLayer.csproj", "BuildingBlocks/EventLog.DataAccessLayer/"]
COPY ["Services/CommonLibrary/CommonLibrary.csproj", "Services/CommonLibrary/"]
COPY ["docker-compose.dcproj", "docker-compose.dcproj"]
COPY ["NuGet.Config","NuGet.Config"]

RUN dotnet restore "StreetThings.WebApps.sln" -v n
COPY . .

WORKDIR "/src/Services/Authorization/AuthorizationService"
RUN dotnet build "AuthorizationService.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "AuthorizationService.csproj" -c Release -o /app/publish

FROM base AS final
EXPOSE 80
EXPOSE 443
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "AuthorizationService.dll"]