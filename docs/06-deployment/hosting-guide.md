# Hosting Guide
Choose a hosting platform based on your needs and budget.
## Local development
For development and testing:
\\\ash
docker-compose up
dotnet run
\\\
Provides:
- PostgreSQL database
- PgAdmin for database management
- Hot reload for code changes
- Full debugging support
## Render.com
Suitable for small to medium projects:
\\\ash
# Deploy from git
git push origin main
\\\
Benefits:
- Simple git-based deployment
- Automatic SSL/TLS
- Environment variables in dashboard
- PostgreSQL database included
- PR preview environments
Drawbacks:
- Smaller instance types
- Limited scaling options
- Newer platform (less mature)
## AWS
For production with enterprise requirements:
Services:
- **EC2** ΓÇö Virtual machines
- **ECS** ΓÇö Docker container orchestration
- **RDS** ΓÇö Managed PostgreSQL
- **API Gateway** ΓÇö Request routing
- **CloudFront** ΓÇö CDN
Benefits:
- Highly scalable
- Global distribution
- Advanced monitoring/alerting
- Enterprise SLA
Drawbacks:
- Complex setup
- Higher cost at scale
- Requires DevOps expertise
## Azure
For Microsoft-centric environments:
Services:
- **App Service** ΓÇö Managed ASP.NET Core hosting
- **Azure Database for PostgreSQL** ΓÇö Managed database
- **Application Insights** ΓÇö Monitoring
- **Azure DevOps** ΓÇö CI/CD pipelines
Benefits:
- Native .NET support
- Integrated CI/CD
- Good documentation
- Competitive pricing
## Docker
For any platform supporting Docker:
\\\ash
docker build -t campfitfurdogs:latest .
docker run -p 8080:8080 campfitfurdogs:latest
\\\
Deployment:
- Docker Hub
- Private registry
- Kubernetes (K8s)
- Docker Swarm
## Recommended for this project
**Development**: Local docker-compose
**Small production**: Render.com (simplicity)
**Large production**: AWS (scalability) or Azure (Microsoft ecosystem)
