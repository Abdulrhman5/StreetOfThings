#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/core/aspnet:3.1 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/core/sdk:3.1 AS build
WORKDIR /src
COPY ["Services/Transaction/Transaction.Service/Transaction.Service.csproj", "Services/Transaction/Transaction.Service/"]
COPY ["Services/Transaction/Transaction.DataAccessLayer/Transaction.DataAccessLayer.csproj", "Services/Transaction/Transaction.DataAccessLayer/"]
COPY ["Services/Transaction/Transaction.Models/Transaction.Models.csproj", "Services/Transaction/Transaction.Models/"]
COPY ["Services/CommonLibrary/CommonLibrary.csproj", "Services/CommonLibrary/"]
COPY ["Services/Transaction/Transaction.BusinessLogic/Transaction.BusinessLogic.csproj", "Services/Transaction/Transaction.BusinessLogic/"]
COPY ["BuildingBlocks/EventBus/EventBus.csproj", "BuildingBlocks/EventBus/"]
COPY ["BuildingBlocks/HostingHelpers/HostingHelpers.csproj", "BuildingBlocks/HostingHelpers/"]

RUN dotnet restore "Services/Transaction/Transaction.Service/Transaction.Service.csproj"
COPY . .
WORKDIR "/src/Services/Transaction/Transaction.Service"
RUN dotnet build "Transaction.Service.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Transaction.Service.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Transaction.Service.dll"]