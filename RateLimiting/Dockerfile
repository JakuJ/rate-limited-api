FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build

# Restore dependencies
WORKDIR /source
COPY Server/Server.csproj Server/
RUN dotnet restore Server/Server.csproj

# Build the app
COPY Server/ Server/
WORKDIR Server
RUN dotnet build -c Release --no-restore

# Publish the app
FROM build AS publish
RUN dotnet publish -c Release --no-build -o /app

# Run the app
FROM mcr.microsoft.com/dotnet/aspnet:5.0-alpine
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "Server.dll"]
