.DEFAULT_GOAL := help

# -- Configuration ---------------------------------------------
CONFIGURATION ?= Release

# -- Help ------------------------------------------------------

.PHONY: help

help: ## Show available commands
	@grep -E '^[a-zA-Z_-]+:.*?## .*$$' $(MAKEFILE_LIST) | \
		awk 'BEGIN {FS = ":.*?## "}; {printf "  \033[36m%-20s\033[0m %s\n", $$1, $$2}'

# -- Infrastructure --------------------------------------------

.PHONY: infra-up infra-down

infra-up: ## Start infrastructure (PostgreSQL, Redis, RabbitMQ)
	docker compose up -d

infra-down: ## Stop infrastructure
	docker compose down

# -- Backend ---------------------------------------------------

.PHONY: backend-restore backend-build backend-test backend-clean backend-up

backend-restore: ## Restore NuGet packages
	dotnet restore

backend-build: backend-restore ## Build .NET solution
	dotnet build --no-restore --configuration $(CONFIGURATION)

backend-test: backend-build ## Run backend tests
	dotnet test --no-build --configuration $(CONFIGURATION) --verbosity normal

backend-clean: ## Remove .NET build artifacts
	dotnet clean --configuration $(CONFIGURATION)
	find . -type d \( -name bin -o -name obj \) -exec rm -rf {} + 2>/dev/null || true

backend-up: infra-up ## Start infrastructure + API
	dotnet run --project src/CampFitFurDogs.Api --configuration $(CONFIGURATION)

# -- Frontend --------------------------------------------------

.PHONY: frontend-install frontend-build frontend-test frontend-lint frontend-clean frontend-up

frontend-install: ## Install frontend dependencies
	npm ci --prefix frontend

frontend-build: frontend-install ## Build frontend
	npm run build --prefix frontend

frontend-test: frontend-install ## Run frontend tests
	npm test --prefix frontend

frontend-lint: frontend-install ## Lint frontend
	npm run lint --prefix frontend

frontend-clean: ## Remove frontend build artifacts
	rm -rf frontend/.next frontend/node_modules

frontend-up: ## Start frontend dev server
	npm run dev --prefix frontend

# -- Aggregates ------------------------------------------------

.PHONY: restore build test clean up down reset all

restore: backend-restore frontend-install ## Restore all dependencies

build: backend-build frontend-build ## Build everything

test: backend-test frontend-test ## Run all tests

clean: backend-clean frontend-clean ## Clean everything

up: infra-up ## Start full stack (infra + API + frontend)
	trap 'kill 0' EXIT; \
	dotnet run --project src/CampFitFurDogs.Api --configuration $(CONFIGURATION) & \
	npm run dev --prefix frontend

down: infra-down ## Stop all containers

reset: clean restore ## Clean slate: wipe artifacts, reinstall deps

all: restore build test ## Full pipeline: restore > build > test
