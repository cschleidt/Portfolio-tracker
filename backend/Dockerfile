FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["src/API/PortfolioTracker.API.csproj", "src/API/"]
COPY ["src/Application/PortfolioTracker.Application.csproj", "src/Application/"]
COPY ["src/Domain/PortfolioTracker.Domain.csproj", "src/Domain/"]
COPY ["src/Infrastructure/PortfolioTracker.Infrastructure.csproj", "src/Infrastructure/"]
RUN dotnet restore "src/API/PortfolioTracker.API.csproj"
COPY . .
RUN dotnet build "src/API/PortfolioTracker.API.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "src/API/PortfolioTracker.API.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "PortfolioTracker.API.dll"]
