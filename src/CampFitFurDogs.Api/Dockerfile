# ============================
# BUILD STAGE
# ============================
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /app

# Copy solution + project files first (cache-friendly)
COPY CampFitFurDogs.sln ./
COPY Directory.Packages.props ./

COPY src/CampFitFurDogs.Api/CampFitFurDogs.Api.csproj src/CampFitFurDogs.Api/
COPY src/CampFitFurDogs.Application/CampFitFurDogs.Application.csproj src/CampFitFurDogs.Application/
COPY src/CampFitFurDogs.Infrastructure/CampFitFurDogs.Infrastructure.csproj src/CampFitFurDogs.Infrastructure/
COPY src/SharedKernel/SharedKernel.csproj src/SharedKernel/
COPY src/SharedKernel.Api/SharedKernel.Api.csproj src/SharedKernel.Api/

# Restore dependencies
RUN dotnet restore

# Copy the rest of the source
COPY . .

# Publish with ReadyToRun enabled
RUN dotnet publish src/CampFitFurDogs.Api/CampFitFurDogs.Api.csproj \
    -c Release \
    -o /app/publish \
    -p:PublishReadyToRun=true \
    -p:PublishSingleFile=false \
    -p:PublishTrimmed=false

# ============================
# RUNTIME STAGE
# ============================
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app

COPY --from=build /app/publish .

ENTRYPOINT ["dotnet", "CampFitFurDogs.Api.dll"]
