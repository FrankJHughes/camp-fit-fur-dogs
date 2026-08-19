# Hosting Guide

The Camp Fit Fur Dogs platform supports multiple hosting environments, ranging from local development to full enterprise‑grade cloud deployments.  
This guide explains each hosting option, how the **Host layer** orchestrates startup, and how environment‑specific behavior (including hosting modules) is applied.

The **CampFitFurDogs.Host** project is the executable entry point for all hosting environments.  
It configures the web host, applies hosting modules, registers platform services, maps endpoints, and runs the application.

---

# Local Development

Local development uses Docker‑based infrastructure and the Host project running directly via `dotnet run`.

### Start local services

```bash
docker-compose up
dotnet run --project src/CampFitFurDogs.Host
```

### Provides

- PostgreSQL database  
- PgAdmin for database management  
- Hot reload for code changes  
- Full debugging support  
- Local environment variables via `appsettings.Development.json`

Local development is the recommended environment for day‑to‑day feature work.

---

# Render.com

Render is ideal for small‑to‑medium deployments and supports **PR Preview environments**, which integrate directly with the platform’s hosting modules.

### Deploy from Git

```bash
git push origin main
```

### Benefits

- Simple Git‑based deployment  
- Automatic SSL/TLS  
- Environment variables managed via dashboard  
- Built‑in PostgreSQL  
- **PR Preview environments** (fully supported by hosting modules)

### Drawbacks

- Smaller instance types  
- Limited horizontal scaling  
- Newer platform (less mature)

### Host Layer Behavior on Render

The Host project:

- detects Render PR Preview environments  
- executes hosting modules  
- fetches GitHub Actions artifacts  
- applies PR‑specific configuration overrides  
- runs the API with environment‑adapted settings

Render is the recommended hosting option for **small production deployments**.

---

# AWS

AWS is suitable for enterprise‑grade production deployments requiring scalability, global distribution, and advanced monitoring.

### Relevant Services

- **EC2** — Virtual machines  
- **ECS** — Docker container orchestration  
- **RDS** — Managed PostgreSQL  
- **API Gateway** — Request routing  
- **CloudFront** — CDN  

### Benefits

- Highly scalable  
- Global distribution  
- Advanced monitoring and alerting  
- Enterprise SLA  

### Drawbacks

- Complex setup  
- Higher cost at scale  
- Requires DevOps expertise

### Host Layer Behavior on AWS

The Host project runs normally with:

- environment variables from ECS/EC2  
- configuration from SSM Parameter Store or Secrets Manager  
- CloudWatch logs enriched with correlation IDs and observability events

AWS is recommended for **large production deployments**.

---

# Azure

Azure is ideal for Microsoft‑centric environments and teams already using Azure DevOps.

### Relevant Services

- **App Service** — Managed ASP.NET Core hosting  
- **Azure Database for PostgreSQL** — Managed database  
- **Application Insights** — Monitoring  
- **Azure DevOps** — CI/CD pipelines  

### Benefits

- Native .NET support  
- Integrated CI/CD  
- Good documentation  
- Competitive pricing  

### Drawbacks

- Some services have regional limitations  
- Scaling models differ from AWS

### Host Layer Behavior on Azure

The Host project integrates with:

- Application Insights telemetry  
- Azure App Service environment variables  
- Azure PostgreSQL connection strings  
- Azure logging and diagnostics

Azure is recommended for **large production deployments** within Microsoft ecosystems.

---

# Docker (Generic Hosting)

Docker provides a portable hosting option for any environment that supports containers.

### Build and run

```bash
docker build -t campfitfurdogs:latest .
docker run -p 8080:8080 campfitfurdogs:latest
```

### Deployment Options

- Docker Hub  
- Private registry  
- Kubernetes (K8s)  
- Docker Swarm  

### Host Layer Behavior in Docker

The Host project runs inside the container with:

- environment variables passed via `docker run` or orchestrator  
- configuration from mounted files or secrets  
- observability events emitted to container logs

Docker is recommended for **custom hosting environments** or **Kubernetes deployments**.

---

# Host Layer Responsibilities Across All Environments

Regardless of hosting platform, the **CampFitFurDogs.Host** project:

1. Builds the WebApplicationBuilder  
2. Applies hosting modules  
3. Registers platform services  
4. Registers API + Identity endpoints  
5. Activates Frank.Core + Frank.Identity middleware  
6. Maps endpoints under `/api`  
7. Runs the application  

This ensures consistent startup behavior across all environments.

---

# Recommended Hosting Options

- **Development**: Local `docker-compose`  
- **Small production**: Render.com (simplicity + PR previews)  
- **Large production**: AWS (scalability) or Azure (Microsoft ecosystem)

---

# Summary

This hosting guide explains how the Camp Fit Fur Dogs platform runs across environments and how the **Host layer** ensures consistent startup, configuration, and observability.

The Host project is responsible for:

- environment detection  
- hosting module execution  
- platform registration  
- endpoint activation  
- middleware configuration  
- application execution  

The API assembly remains pure and host‑agnostic, enabling clean vertical‑slice architecture and predictable behavior across all hosting platforms.
