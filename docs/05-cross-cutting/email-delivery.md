# Frank.CrossCutting — Email Delivery

Email delivery handles transactional messages sent to users. It is a **cross‑cutting concern**: multiple vertical slices (Identity, Customer, Dogs, Scheduling, Billing) rely on a consistent, reliable, asynchronous email pipeline.

This document describes the email delivery subsystem under:

```
/docs/05-cross-cutting
```

and maps it back to its implementation under:

```
/src/Frank/Core.Infrastructure
/src/Frank/Identity/Application
/src/Frank/Identity/Infrastructure
```

Email delivery is composed of:

- event‑driven asynchronous sending  
- strongly typed email service abstraction  
- environment‑aware provider selection  
- configuration‑driven templates and sender metadata  
- robust error handling and retry behavior  

---

## Common Scenarios

Transactional emails include:

- **welcome email** after account creation  
- **password reset** links  
- **confirmation** of important actions  
- **notifications** about resource changes  

These scenarios are implemented in vertical slices such as:

- [Welcome Email](ca://s?q=Explain_welcome_email_flow)  
- [Password Reset Email](ca://s?q=Explain_password_reset_email_flow)  
- [Email Verification](ca://s?q=Explain_email_verification_flow)

---

## Implementation Pattern

Email delivery follows an **event‑driven, asynchronous** pattern to ensure that email failures never break the main request flow.

```csharp
// 1. Create session
var session = Session.Create(userId);
await _sessionWriter.AddAsync(session, ct);

// 2. Raise domain event
var emailEvent = new UserSessionCreatedEvent(session);
_eventPublisher.Publish(emailEvent);

// 3. Commit transaction
await _unitOfWork.CommitAsync(ct);

// 4. Async handler sends email (after transaction succeeds)
```

### Why this pattern?

- **Email sending is slow** → must not block the request  
- **Email providers fail** → must not break business logic  
- **Events ensure reliability** → email is sent only after commit  
- **Handlers can retry** → exponential backoff, dead‑letter queues  
- **Infrastructure can swap providers** → SMTP, SendGrid, local file  

See:  
- [Outbox Pattern](ca://s?q=Explain_outbox_pattern)  
- [Domain Events](ca://s?q=Explain_domain_events)

---

## Infrastructure Service

Email delivery uses a strongly typed abstraction:

```csharp
public interface IEmailService
{
    Task SendAsync(string to, string subject, string body, CancellationToken ct);
}
```

### Implementations

- **`SmtpEmailService`** — uses an SMTP server  
- **`SendGridEmailService`** — uses SendGrid’s REST API  
- **`LocalFileEmailService`** — writes emails to disk (development/testing)  

Providers are selected based on configuration and environment.

See: [Email Providers](ca://s?q=Explain_email_provider_selection)

---

## Configuration

Email settings are defined in `appsettings.json`:

```json
{
  "Email": {
    "Provider": "SendGrid",
    "FromAddress": "noreply@example.com",
    "FromName": "Camp Fit Fur Dogs",
    "SendGridApiKey": "YOUR_API_KEY"
  }
}
```

### Configuration Features

- **strongly typed binding**  
- **environment‑aware overrides**  
- **required fields validated at startup**  
- **secret values injected via environment variables**  

See:  
- [Configuration Management](ca://s?q=Explain_crosscutting_configuration_management)  
- [Secret Management](ca://s?q=Explain_crosscutting_secret_management)

---

## Error Handling

Email failures must **never** break the main request.

Correct behavior:

- **do not fail the main request**  
- **log failures** for monitoring and observability  
- **retry with exponential backoff**  
- **fallback notification** (e.g., in‑app alerts)  

This ensures:

- user experience is not degraded  
- operational teams can diagnose issues  
- email delivery remains reliable even under provider outages  

See:  
- [Logging & Observability](ca://s?q=Generate_crosscutting_logging_doc)  
- [Error Handling](ca://s?q=Generate_crosscutting_error_handling_doc)

---

## Runtime Collaboration Points

Email delivery interacts with:

- **Identity Application** — triggers email events  
- **Infrastructure** — provider integration, logging, retry logic  
- **Outbox** — ensures reliable asynchronous delivery  
- **Configuration** — provider selection, sender metadata  
- **Testing** — local file provider, fake providers, mutated contexts  

Email delivery is a foundational cross‑cutting capability that supports multiple vertical slices.

---

## Notes

Keep this document grounded in the actual email delivery implementation.  
Whenever new email flows, providers, or retry strategies are added, update this section to reflect the current architecture.
