#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/core/aspnet:3.1 AS base

FROM mcr.microsoft.com/dotnet/core/sdk:3.1 AS build
WORKDIR /src
COPY ["Services/Authorization/AuthorizationService/AuthorizationService.csproj", "Services/Authorization/AuthorizationService/"]
COPY ["Services/Authorization/ApplicationLogic/ApplicationLogic.csproj", "Services/Authorization/ApplicationLogic/"]
COPY ["Services/Authorization/Models/Models.csproj", "Services/Authorization/Models/"]
COPY ["Services/CommonLibrary/CommonLibrary.csproj", "Services/CommonLibrary/"]
COPY ["Services/Authorization/DataAccessLayer/DataAccessLayer.csproj", "Services/Authorization/DataAccessLayer/"]
COPY ["Services/Authorization/Infrastructure/Infrastructure.csproj", "Services/Authorization/Infrastructure/"]
COPY ["BuildingBlocks/HostingHelpers/HostingHelpers.csproj", "BuildingBlocks/HostingHelpers/"]
COPY ["BuildingBlocks/EventBus/EventBus.csproj", "BuildingBlocks/EventBus/"]

RUN dotnet restore "Services/Authorization/AuthorizationService/AuthorizationService.csproj"
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