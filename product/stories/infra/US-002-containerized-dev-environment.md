# Containerized Development Environment

## Intent
A dev container configuration exists in the repository so that a developer
can open the project in a containerized environment and land in a fully
working setup — SDK installed, tools available, infrastructure running,
tests passing — without installing anything beyond Docker on their local
machine.

## Value
Some developers prefer containerized environments for isolation, consistency,
or because their host OS differs from the project target. A dev container
configuration provides a reproducible, self-contained environment.
Compatibility with cloud-hosted services such as Codespaces is a side
effect of shipping this configuration, not an explicit goal.

## Acceptance Criteria
- [ ] A dev container configuration exists in the repository
- [ ] The environment includes the correct SDK, required CLI tools, and editor extensions
- [ ] Infrastructure dependencies start automatically using the shared declarative definitions
- [ ] Restore, build, and test succeed without manual intervention after the environment is ready
- [ ] README documents how to launch the containerized environment
- [ ] Environment provisioning completes in under 5 minutes on a warm cache

## Out of Scope
- Prescribing a specific container technology or cloud platform
- Production deployment configuration
- Native local development workflow (covered by One-Command Local Bootstrap)

## Emotional Guarantees: N/A
