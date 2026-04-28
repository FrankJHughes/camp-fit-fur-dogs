---
id: US-143
title: "Outbox Pattern"
epic: Infrastructure
milestone: M2
status: backlog
domain: infra
vertical_slice: false
product: Frank
dependencies:
  - US-050
  - US-108
---

# US-143: Outbox Pattern

## Intent

As a **system operator**, I need domain events to be reliably converted into
outbound messages within the same database transaction so that no communication
is lost if an external service is unavailable.

## Value

The Outbox Pattern is the bridge between "something happened in the domain" and
"someone needs to know about it." Without it, sending emails or notifications
directly from command handlers creates two failure modes: the email fires but the
database write fails (phantom notification), or the database write succeeds but
the email fails (lost notification). The outbox eliminates both by writing
outbound messages transactionally alongside domain changes.

Implementing this as a Frank capability means any future consumer gets guaranteed
message delivery for free — same interceptor pattern as IAuditable and
ISoftDeletable.

## Acceptance Criteria

### Frank (the capability)
- [ ] `IOutboxMessage` interface in `Frank/Abstractions/` declares message identity, type, payload, and status
- [ ] `OutboxInterceptor` in `Frank/Persistence/` converts collected domain events on `ISoftDeletable` aggregates into `OutboxMessage` rows during `SaveChangesAsync`
- [ ] Outbox entries are written in the same database transaction as the domain change
- [ ] `IOutboxProcessor` interface defines the contract for background message dispatch
- [ ] `IOutboxMessageHandler<TMessage>` interface defines per-message-type handling
- [ ] Outbox messages track: Id, EventType, Payload (JSON), CreatedAt, ProcessedAt, Error, RetryCount
- [ ] Auto-registers via Frank DI conventions
- [ ] Frank unit tests verify transactional write, idempotent processing, and retry behavior

### Camp Fit Fur Dogs (the consumer)
- [ ] `OutboxMessage` EF entity and table migration
- [ ] Background service (`IHostedService`) polls the outbox at a configurable interval
- [ ] Processed messages are marked with `ProcessedAt` timestamp (not deleted — audit trail)
- [ ] Failed messages retry with configurable backoff and max retry count
- [ ] Dead-letter handling — messages exceeding max retries are flagged for manual review
- [ ] Health check reports outbox backlog depth

## Emotional Guarantees

- **EG-01 No Surprises** — If the system said it would notify you, it will
- **EG-03 Calm Protection** — No phantom notifications; no lost communications

## Architecture

```
AggregateRoot raises DomainEvent
       |
SaveChangesAsync (Unit of Work)
       |
OutboxInterceptor converts events --> OutboxMessage rows (same transaction)
       |
BackgroundService polls outbox
       |
IOutboxMessageHandler dispatches to email/SMS/push
       |
Marks OutboxMessage.ProcessedAt
```

## Design Seam: Pipeline Behaviors

> The OutboxInterceptor operates at the persistence layer (SaveChanges), not the
> dispatch pipeline. Pipeline behaviors (US-124) handle cross-cutting concerns
> BEFORE persistence. The outbox handles what happens AFTER persistence succeeds.
> They are complementary, not competing.

## Notes

- Dependencies: US-050 (Unit of Work), US-108 (Foundation Extraction) — both shipped
- The outbox payload is JSON-serialized domain event data — deserialized by handlers
- Consider: should processed messages be purged after N days, or kept permanently?
- Consider: polling interval vs. change-notification (e.g., LISTEN/NOTIFY for PostgreSQL)
- **Demo:** Create a customer account, check the outbox table — see the message, watch the background service process it
