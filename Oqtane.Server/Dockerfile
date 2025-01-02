# Base runtime image for production
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
EXPOSE 8080

# Build stage
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

# Copy all solution and project files
COPY Oqtane.Framework/*.sln ./Oqtane.Framework/
COPY Oqtane.Framework/Oqtane.Server/Oqtane.Server.csproj Oqtane.Framework/Oqtane.Server/
COPY Oqtane.Framework/Oqtane.Client/Oqtane.Client.csproj Oqtane.Framework/Oqtane.Client/
COPY Oqtane.Framework/Oqtane.Shared/Oqtane.Shared.csproj Oqtane.Framework/Oqtane.Shared/
COPY Oqtane.Framework/Oqtane.Database.MySQL/Oqtane.Database.MySQL.csproj Oqtane.Framework/Oqtane.Database.MySQL/
COPY Oqtane.Framework/Oqtane.Database.PostgreSQL/Oqtane.Database.PostgreSQL.csproj Oqtane.Framework/Oqtane.Database.PostgreSQL/
COPY Oqtane.Framework/Oqtane.Database.Sqlite/Oqtane.Database.Sqlite.csproj Oqtane.Framework/Oqtane.Database.Sqlite/
COPY Oqtane.Framework/Oqtane.Database.SqlServer/Oqtane.Database.SqlServer.csproj Oqtane.Framework/Oqtane.Database.SqlServer/
COPY csdp/*.sln ./csdp/
COPY csdp/Server/Biote.Module.Csdp.Server.csproj csdp/Server/
COPY csdp/Client/Biote.Module.Csdp.Client.csproj csdp/Client/
COPY csdp/Shared/Biote.Module.Csdp.Shared.csproj csdp/Shared/

# Restore dependencies for all projects
RUN dotnet restore Oqtane.Framework/Oqtane.Server/Oqtane.Server.csproj
RUN dotnet restore csdp/Server/Biote.Module.Csdp.Server.csproj

# Copy source code
COPY . .

# Build all database dependencies
WORKDIR /src/Oqtane.Framework/Oqtane.Database.MySQL
RUN dotnet build -c $BUILD_CONFIGURATION
WORKDIR /src/Oqtane.Framework/Oqtane.Database.PostgreSQL
RUN dotnet build -c $BUILD_CONFIGURATION
WORKDIR /src/Oqtane.Framework/Oqtane.Database.Sqlite
RUN dotnet build -c $BUILD_CONFIGURATION
WORKDIR /src/Oqtane.Framework/Oqtane.Database.SqlServer
RUN dotnet build -c $BUILD_CONFIGURATION

# Build csdp submodule
WORKDIR /src/csdp/Server
RUN dotnet build -c $BUILD_CONFIGURATION

WORKDIR /src/csdp/Client
RUN dotnet build -c $BUILD_CONFIGURATION

WORKDIR /src/csdp/Shared
RUN dotnet build -c $BUILD_CONFIGURATION

# Publish Oqtane.Server
WORKDIR /src/Oqtane.Framework/Oqtane.Server
RUN dotnet publish Oqtane.Server.csproj -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# Publish Biote.Module.Csdp.Server
WORKDIR /src/csdp/Server
RUN dotnet publish Biote.Module.Csdp.Server.csproj -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# Final runtime stage
FROM base AS final
WORKDIR /app
COPY --from=build /app/publish .

# Entrypoint
ENTRYPOINT ["dotnet", "Oqtane.Server.dll"]
