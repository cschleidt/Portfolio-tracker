FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["src/API/PortfolioTracker.API.csproj", "src/API/"]
COPY ["src/Application/PortfolioTracker.Application.csproj", "src/Application/"]
COPY ["src/Domain/PortfolioTracker.Domain.csproj", "src/Domain/"]
COPY ["src/Infrastructure/PortfolioTracker.Infrastructure.csproj", "src/Infrastructure/"]
RUN dotnet restore "src/API/PortfolioTracker.API.csproj"
COPY . .
RUN dotnet publish "src/API/PortfolioTracker.API.csproj" -c Release -o /app/publish

# Official Playwright .NET image — has Chromium + all dependencies pre-installed
FROM mcr.microsoft.com/playwright/dotnet:v1.50.0-jammy AS final
WORKDIR /app
EXPOSE 8080
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "PortfolioTracker.API.dll"]
