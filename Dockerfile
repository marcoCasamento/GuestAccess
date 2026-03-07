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

# Create non-root user for security
RUN adduser --disabled-password --gecos '' appuser && chown -R appuser /app
USER appuser

COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "GuestAccess.dll"]
