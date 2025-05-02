# Etapa 1: Build
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /app

COPY . . 

RUN dotnet restore CapitalGainsCalculator.sln

RUN dotnet build CapitalGainsCalculator.sln -c Release

RUN dotnet test tests/CapitalGainsCalculator.UnitTests/CapitalGainsCalculator.UnitTests.csproj --no-build --verbosity normal
RUN dotnet test tests/CapitalGainsCalculator.IntegrationTests/CapitalGainsCalculator.IntegrationTests.csproj --no-build --verbosity normal

FROM mcr.microsoft.com/dotnet/runtime:8.0 AS runtime
WORKDIR /app
COPY --from=build /app/src/CapitalGainsCalculator.CLI/bin/Release/net8.0/ .

ENTRYPOINT ["dotnet", "CapitalGainsCalculator.CLI.dll"]
