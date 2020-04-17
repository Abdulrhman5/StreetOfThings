#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/core/aspnet:latest AS base

FROM mcr.microsoft.com/dotnet/core/sdk:latest AS build
WORKDIR /src
COPY ["AuthorizationService/AuthorizationService.csproj", "AuthorizationService/"]
COPY ["ApplicationLogic/ApplicationLogic.csproj", "ApplicationLogic/"]
COPY ["Models/Models.csproj", "Models/"]
COPY ["CommonLibrary/CommonLibrary.csproj", "CommonLibrary/"]
COPY ["DataAccessLayer/DataAccessLayer.csproj", "DataAccessLayer/"]
RUN dotnet restore "AuthorizationService/AuthorizationService.csproj"
COPY . .
WORKDIR "/src/AuthorizationService"
RUN dotnet build "AuthorizationService.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "AuthorizationService.csproj" -c Release -o /app/publish

FROM base AS final
EXPOSE 80
EXPOSE 443
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "AuthorizationService.dll"]