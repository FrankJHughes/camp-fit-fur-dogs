#!/usr/bin/env bash
set -euo pipefail

RED='\033[0;31m'
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
CYAN='\033[0;36m'
BOLD='\033[1m'
NC='\033[0m'

START_TIME=$(date +%s)

print_header() {
    echo ""
    echo -e "${CYAN}${BOLD}==================================================${NC}"
    echo -e "${CYAN}${BOLD}  Camp Fit Fur Dogs - Local Bootstrap${NC}"
    echo -e "${CYAN}${BOLD}==================================================${NC}"
    echo ""
}

print_phase() {
    echo ""
    echo -e "${YELLOW}${BOLD}> $1${NC}"
    echo ""
}

check_command() {
    local cmd="$1"
    local hint="$2"
    if ! command -v "$cmd" &> /dev/null; then
        echo -e "  ${RED}x $cmd is not installed.${NC}"
        echo "    $hint"
        exit 1
    fi
    local version
    version=$("$cmd" --version 2>&1 | head -1)
    echo -e "  ${GREEN}+${NC} $cmd -- $version"
}

print_header

# -- Phase 1: Validate Prerequisites ------------------------------------------
print_phase "Phase 1/4 -- Validating prerequisites"

check_command "docker" \
    "Install Docker Desktop: https://www.docker.com/products/docker-desktop/"
check_command "dotnet" \
    "Install .NET SDK: https://dot.net/download"
check_command "make" \
    "Install make: sudo apt install -y make"

if ! docker info &> /dev/null; then
    echo -e "  ${RED}x Docker daemon is not running.${NC}"
    echo "    Start Docker Desktop and try again."
    exit 1
fi
echo -e "  ${GREEN}+${NC} Docker daemon is running"

# -- Phase 2: Start Infrastructure --------------------------------------------
print_phase "Phase 2/4 -- Starting infrastructure"

docker compose up -d --wait

echo -e "  ${GREEN}+${NC} Infrastructure services are healthy"

# -- Phase 3: Build & Test ----------------------------------------------------
print_phase "Phase 3/4 -- Building and testing"

make all

# -- Phase 4: Readiness Report ------------------------------------------------
END_TIME=$(date +%s)
ELAPSED=$((END_TIME - START_TIME))

echo ""
echo -e "${CYAN}${BOLD}==================================================${NC}"
echo -e "${CYAN}${BOLD}  Readiness Report${NC}"
echo -e "${CYAN}${BOLD}==================================================${NC}"
echo ""
echo -e "  ${BOLD}Services${NC}"
echo -e "  PostgreSQL          :5432    ${GREEN}+${NC}"
echo -e "  Redis               :6379    ${GREEN}+${NC}"
echo -e "  RabbitMQ            :5672    ${GREEN}+${NC}"
echo -e "  RabbitMQ Management :15672   ${GREEN}+${NC}"
echo ""
echo -e "  ${BOLD}Pipeline${NC}"
echo -e "  Infrastructure      ${GREEN}PASS${NC}"
echo -e "  Build & Test        ${GREEN}PASS${NC}"
echo ""
echo -e "  ${BOLD}Elapsed${NC}              ${ELAPSED}s"
echo ""
echo -e "  ${GREEN}${BOLD}Ready to develop!${NC}"
echo ""