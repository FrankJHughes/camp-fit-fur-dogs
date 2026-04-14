.DEFAULT_GOAL := help

# ── Configuration ─────────────────────────────────────────────
CONFIGURATION ?= Release

# ── Help ──────────────────────────────────────────────────────

.PHONY: help

help: ## Show available commands
	@grep -E '^[a-zA-Z_-]+:.*?## .*$$' $(MAKEFILE_LIST) | \
		awk 'BEGIN {FS = ":.*?## "}; {printf "  \033[36m%-20s\033[0m %s\n", $, $}'

# ── Backend (API) ─────────────────────────────────────────────

.PHONY: api-restore api-build api-test api-clean api-up

api-restore: ## Restore NuGet packages
	dotnet restore

api-build: api-restore ## Build the .NET solution
	dotnet build --no-restore --configuration $(CONFIGURATION)

api-test: api-build ## Run backend tests
	dotnet test --no-build --configuration $(CONFIGURATION) --verbosity normal

api-clean: ## Remove backend build artifacts
	dotnet clean --configuration $(CONFIGURATION)
	find . -type d \( -name bin -o -name obj \) -exec rm -rf {} + 2>/dev/null || true

api-up: ## Run the API
	dotnet run --project src/CampFitFurDogs.Api

# ── Frontend ──────────────────────────────────────────────────

.PHONY: frontend-install frontend-build frontend-test frontend-lint frontend-clean frontend-up

frontend-install: ## Install frontend dependencies
	npm ci --prefix frontend

frontend-build: frontend-install ## Build the frontend
	npm run build --prefix frontend

frontend-test: frontend-install ## Run frontend tests
	npm test --prefix frontend

frontend-lint: frontend-install ## Lint the frontend
	npm run lint --prefix frontend

frontend-clean: ## Remove frontend build artifacts
	rm -rf frontend/.next frontend/node_modules

frontend-up: ## Start the frontend dev server
	npm run dev --prefix frontend

# ── Infrastructure ────────────────────────────────────────────

.PHONY: infra-up infra-down

infra-up: ## Start infrastructure services
	docker compose up -d

infra-down: ## Stop infrastructure services
	docker compose down

# ── Aggregates ────────────────────────────────────────────────

.PHONY: restore build test clean all

restore: api-restore frontend-install ## Restore all dependencies

build: api-build frontend-build ## Build everything

test: api-test frontend-test ## Run all tests

clean: api-clean frontend-clean ## Clean everything

all: restore build test ## Full pipeline: restore > build > test

# ── Development ───────────────────────────────────────────────

.PHONY: dev dev-down

dev: infra-up ## Start full stack (infra + API + frontend)
	@trap 'kill 0' EXIT; \
	dotnet run --project src/CampFitFurDogs.Api & \
	npm run dev --prefix frontend

dev-down: infra-down ## Stop all dev services
	@echo "Infrastructure stopped."
