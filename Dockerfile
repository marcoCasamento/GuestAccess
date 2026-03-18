# Build stage
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

# Copy csproj and restore dependencies
COPY ["GuestAccess.csproj", "./"]
RUN dotnet restore "GuestAccess.csproj"

# Copy everything else and build
COPY . .
RUN dotnet build "GuestAccess.csproj" -c Release -o /app/build

# Publish stage
FROM build AS publish
RUN dotnet publish "GuestAccess.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

# Create non-root user for security with a fixed UID/GID (1001).
# The host volume ./data must be owned by this UID: sudo chown -R 1001:1001 ./data
RUN groupadd -g 1001 appuser && useradd -u 1001 -g appuser -m -s /bin/bash appuser && chown -R appuser:appuser /app
USER appuser

COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "GuestAccess.dll"]
