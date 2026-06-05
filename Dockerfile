# ============================
# BUILD STAGE
# ============================
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /app

# Copy solution + project files first (cache-friendly)
COPY CampFitFurDogs.slnx ./
COPY Directory.Packages.props ./

COPY src/CampFitFurDogs.Api/CampFitFurDogs.Api.csproj src/CampFitFurDogs.Api/
COPY src/CampFitFurDogs.Application/CampFitFurDogs.Application.csproj src/CampFitFurDogs.Application/
COPY src/CampFitFurDogs.Infrastructure/CampFitFurDogs.Infrastructure.csproj src/CampFitFurDogs.Infrastructure/
COPY src/Frank/Frank.csproj src/Frank/
COPY src/Frank.Api/Frank.Api.csproj src/Frank.Api/

RUN dotnet restore src/CampFitFurDogs.Api/CampFitFurDogs.Api.csproj

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
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final
WORKDIR /app

# Install Kerberos/GSSAPI native libs required by HttpClientHandler on Linux
RUN apt-get update && apt-get install -y libgssapi-krb5-2

ENV ASPNETCORE_URLS=http://0.0.0.0:${PORT}

COPY --from=build /app/publish .

ENTRYPOINT ["dotnet", "CampFitFurDogs.Api.dll"]
