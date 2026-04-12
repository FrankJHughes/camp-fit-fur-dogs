.DEFAULT_GOAL := help

# -- Configuration ----------------------------------------------------------
CONFIGURATION ?= Release

# -- Targets ----------------------------------------------------------------

.PHONY: help restore build test clean infra-up infra-down all

help: ## Show available commands
	@grep -E '^[a-zA-Z_-]+:.*?## .*$$' $(MAKEFILE_LIST) | \
		awk 'BEGIN {FS = ":.*?## "}; {printf "  \033[36m%-15s\033[0m %s\n", $$1, $$2}'

restore: ## Restore NuGet packages
	dotnet restore

build: restore ## Build the solution
	dotnet build --no-restore --configuration $(CONFIGURATION)

test: build ## Run all tests
	dotnet test --no-build --configuration $(CONFIGURATION) --verbosity normal

clean: ## Remove build artifacts
	dotnet clean --configuration $(CONFIGURATION)
	find . -type d \( -name bin -o -name obj \) -exec rm -rf {} + 2>/dev/null || true

infra-up: ## Start infrastructure services
	docker compose up -d

infra-down: ## Stop infrastructure services
	docker compose down

all: restore build test ## Full pipeline: restore > build > test
# ── Frontend ──────────────────────────────────────────────────

.PHONY: frontend-install frontend-build frontend-lint frontend-dev

frontend-install:
	npm ci --prefix src/frontend

frontend-build: frontend-install
	npm run build --prefix src/frontend

frontend-lint: frontend-install
	npm run lint --prefix src/frontend

frontend-dev:
	npm run dev --prefix src/frontend
