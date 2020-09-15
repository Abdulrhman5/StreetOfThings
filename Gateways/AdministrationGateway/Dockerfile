#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/core/aspnet:3.1-buster-slim AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/core/sdk:3.1-buster AS build
WORKDIR /src
COPY ["Gateways/AdministrationGateway/AdministrationGateway.csproj", "Gateways/AdministrationGateway/"]
COPY ["Services/CommonLibrary/CommonLibrary.csproj", "Services/CommonLibrary/"]
COPY ["BuildingBlocks/HostingHelpers/HostingHelpers.csproj", "BuildingBlocks/HostingHelpers/"]
RUN dotnet restore "Gateways/AdministrationGateway/AdministrationGateway.csproj"
COPY . .
WORKDIR "/src/Gateways/AdministrationGateway"
RUN dotnet build "AdministrationGateway.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "AdministrationGateway.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "AdministrationGateway.dll"]