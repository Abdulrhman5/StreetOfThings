#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/core/aspnet:latest AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/core/sdk:latest AS build
WORKDIR /src
COPY ["Gateways/MobileApiGateway/MobileApiGateway.csproj", "Gateways/MobileApiGateway/"]
COPY ["Services/Authorization/CommonLibrary/CommonLibrary.csproj", "Services/Authorization/CommonLibrary/"]
RUN dotnet restore "Gateways/MobileApiGateway/MobileApiGateway.csproj"
COPY . .
WORKDIR "/src/Gateways/MobileApiGateway"
RUN dotnet build "MobileApiGateway.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "MobileApiGateway.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "MobileApiGateway.dll"]