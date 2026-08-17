# Observability
Observability provides correlation, request tracing, and measurable runtime signals for production diagnostics.
## Core components
### Correlation Context
Every request gets a unique correlation ID that flows through request handling, application layer, infrastructure, and logging. This enables end-to-end request tracing.
### Request Observation Context
During request processing, observation data is collected:
- Request start time and duration
- HTTP method and path
- Response status code
- User ID (if authenticated)
- Errors and exceptions
### Observation Middleware
Two middleware pieces establish observability:
1. \InboundObservationContextMiddleware\ ΓÇö creates context at request start
2. \OutboundObservationContextHandler\ ΓÇö includes correlation ID in responses
## How it works
For each request:
1. Inbound middleware creates/reads correlation ID
2. Context is available throughout request lifetime
3. All logging includes correlation ID
4. Response includes \X-Correlation-ID\ header
5. Related requests share the same correlation ID
The \ErrorBoundaryObserver\ captures unhandled exceptions with full context for debugging.
