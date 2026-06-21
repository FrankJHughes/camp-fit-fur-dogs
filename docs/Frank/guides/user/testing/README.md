# Frank Testing — User Guide

This guide explains how to **use the Frank Testing capability** to write deterministic, isolated, mutation‑driven tests for applications built on the Frank Framework.  
It is intended for **Frank users** — developers writing tests *for their product*, not contributors to Frank itself.

If you are extending Frank.Testing, see the **Frank Developer Testing Guide**.  
If you are validating Frank.Testing itself, see the **Frank Tester Testing Guide**.

---

# 1. Purpose of Frank.Testing (User Perspective)

Frank.Testing provides a deterministic test harness that allows you to test your application without:

- real environment variables  
- real configuration sources  
- real hosting providers  
- real startup modules  
- nondeterministic behavior  

It gives you:

- a predictable test host  
- a mutation model for environment, configuration, DI, hosting, and startup  
- a unified way to create test clients  
- a safe, isolated test environment  

You focus on writing tests — Frank handles the infrastructure.

---

# 2. The MutatedWebApplicationFactory

The core entry point for testing is:

```csharp
var factory = new MutatedWebApplicationFactory();
var client = factory.CreateClient();
```

This factory:

- boots your application in a deterministic test host  
- applies mutations before startup  
- prevents access to real environment variables  
- prevents loading real configuration  
- prevents using real hosting providers  

It is the **only** supported way to test a Frank application.

---

# 3. Writing Your First Test

Example:

```csharp
[Fact]
public async Task Can_Create_Customer()
{
    var factory = new MutatedWebApplicationFactory();

    var client = factory.CreateClient();

    var response = await client.PostAsJsonAsync("/customers", new {
        Name = "Alice"
    });

    response.EnsureSuccessStatusCode();
}
```

This test:

- boots the app in a deterministic test host  
- uses no real environment variables  
- uses no real configuration  
- uses no real hosting providers  

Everything is isolated and predictable.

---

# 4. Applying Mutations

Frank.Testing allows you to mutate:

- environment  
- configuration  
- DI container  
- hosting provider  
- startup modules  

Mutations are applied **before** the application starts.

## **4.1 Environment Mutation**

```csharp
var factory = new MutatedWebApplicationFactory()
    .WithEnvironment(env => env.Set("FEATURE_X_ENABLED", "true"));
```

## **4.2 Configuration Mutation**

```csharp
var factory = new MutatedWebApplicationFactory()
    .WithConfiguration(cfg => cfg.AddInMemoryCollection(new Dictionary<string, string> {
        ["MySettings:Value"] = "123"
    }));
```

## **4.3 DI Mutation**

```csharp
var factory = new MutatedWebApplicationFactory()
    .WithServices(services => {
        services.AddSingleton<IMyService, FakeMyService>();
    });
```

## **4.4 Hosting Provider Mutation**

```csharp
var factory = new MutatedWebApplicationFactory()
    .WithHostingProvider(new TestHostingProvider());
```

## **4.5 Startup Module Mutation**

```csharp
var factory = new MutatedWebApplicationFactory()
    .WithStartupModule<OverrideStartupModule>();
```

---

# 5. Creating Test Clients

Frank.Testing supports:

## **5.1 Standard HTTP Client**

```csharp
var client = factory.CreateClient();
```

## **5.2 Named Clients**

```csharp
var client = factory.CreateClient("MyClient");
```

## **5.3 Raw Service Access**

```csharp
var service = factory.Services.GetRequiredService<IMyService>();
```

---

# 6. Testing Handlers

Frank applications use handlers instead of controllers.

Example:

```csharp
var response = await client.PostAsJsonAsync("/orders", new {
    ProductId = 1,
    Quantity = 2
});
```

Frank.Testing ensures:

- validation runs  
- pipeline runs  
- error boundaries run  
- security headers run  

Your tests exercise the **real pipeline**, not a mock.

---

# 7. Testing Validation

Validation errors are returned as structured responses.

Example:

```csharp
var response = await client.PostAsJsonAsync("/orders", new { });

var problem = await response.Content.ReadFromJsonAsync<ProblemDetails>();

Assert.Equal("ValidationFailed", problem.Type);
```

---

# 8. Testing Error Boundaries

Frank ensures consistent error handling.

Example:

```csharp
var response = await client.GetAsync("/throw");

var problem = await response.Content.ReadFromJsonAsync<ProblemDetails>();

Assert.Equal("UnhandledException", problem.Type);
```

---

# 9. Testing Configuration‑Dependent Behavior

Use configuration mutation:

```csharp
var factory = new MutatedWebApplicationFactory()
    .WithConfiguration(cfg => cfg.AddInMemoryCollection(new Dictionary<string, string> {
        ["Features:EnableBeta"] = "true"
    }));
```

Then assert behavior:

```csharp
var client = factory.CreateClient();

var response = await client.GetAsync("/beta");

Assert.Equal(HttpStatusCode.OK, response.StatusCode);
```

---

# 10. Testing Environment‑Dependent Behavior

Use environment mutation:

```csharp
var factory = new MutatedWebApplicationFactory()
    .WithEnvironment(env => env.Set("APP_MODE", "Test"));
```

---

# 11. Testing DI‑Dependent Behavior

Override services:

```csharp
var factory = new MutatedWebApplicationFactory()
    .WithServices(services => {
        services.AddSingleton<IClock, FakeClock>();
    });
```

---

# 12. What You Should *Not* Do

Frank.Testing prevents:

- accessing real environment variables  
- loading real configuration  
- using real hosting providers  
- mutating DI after startup  
- mutating environment after startup  
- mutating configuration after startup  
- using static state  
- relying on nondeterministic behavior  

If you try, tests will fail loudly.

---

# 13. Summary

Frank.Testing gives you:

- deterministic test hosting  
- deterministic environment, configuration, DI, hosting, and startup mutation  
- a unified test harness  
- safe, isolated test environments  
- predictable behavior across runs  
- full pipeline execution  

As a Frank user, you write tests that focus on **your application logic**, while Frank handles the infrastructure.

