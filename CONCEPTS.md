# Concepts & Architecture 🧠

This document explains the technical foundation and design decisions behind the Feature Flag Service.

---

## 1. Core Theory: Why Feature Flags?

Feature flags (or toggles) allow you to decouple **Deployment** from **Release**. 
- **Deployment**: Getting the code into production.
- **Release**: Enabling the feature for users.

By separating these, you can test in production, perform "canary" releases, and instantly "kill" buggy features without a rollback.

---

## 2. Technical Concepts Explained

### 🎯 Targeting & Evaluation
The service evaluates a flag based on three criteria:
1.  **State**: Is the flag globally `IsEnabled`?
2.  **Targeting Lists**: Is the `userId` in the `AllowedUserIds` list? Is the `role` in the `AllowedRoles` list?
3.  **Rollout Percentage**: If the above don't match, does the user fall within the `X%` rollout?

### 📈 Deterministic Hashing
To ensure a user has a consistent experience (doesn't see a feature flicker on/off between refreshes), we use **Deterministic Hashing**.
- **Theory**: We hash the combination of `FlagKey` + `UserId`.
- **Practice**: Using a stable FNV-style hash (`GetStableHashCode`), we get a large number, then use `hash % 100`. If the result is less than the `RolloutPercentage`, the feature is enabled.
- **Result**: The same user always lands in the same "bucket" for a specific flag.

### 🏛️ Clean Architecture & CQRS
The project follows **Clean Architecture** to ensure the core logic (Domain) is independent of external tools (EF Core, Redis).
- **Domain**: Pure C# logic. Zero dependencies.
- **Application**: Use cases defined as **Commands** (writes) and **Queries** (reads).
- **CQRS**: Using **MediatR**, we separate the logic for *changing* a flag from the logic for *reading* a flag. This simplifies the code and allows for independent optimization (e.g., separate read/write data paths).

### ⚡ Cache-Aside Pattern
Feature flags are on the **critical path** of every request. They must be extremely fast (<5ms).
1.  **Read**: Check **Redis/InMemory Cache** first.
2.  **Miss**: If not found, read from **PostgreSQL**, then populate the cache.
3.  **Invalidation**: When a flag is updated/deleted, we immediately purge it from the cache to ensure consistency.

### 🛡️ Resilience (The "Fail-Safe" Principle)
If the Feature Flag Service goes down, the consuming applications (Orders, Payments, etc.) should **not** crash.
- **Circuit Breaker**: If the service is timing out, the client stops trying (breaks the circuit) to prevent cascading failures.
- **Fail-Safe**: In the `FeatureFlagHttpClient`, if any error occurs (network, 500, timeout), we `catch` and return `false` (Feature Off).

---

## 3. How It Works in Practice (Code Flow)

### Step 1: The Request
A microservice (e.g., Order Service) calls `IsEnabledAsync("new-checkout", "user-123")`.

### Step 2: The Client (FeatureFlagService.Client)
The client uses a **Polly-wrapped HttpClient**. It sends a GET request to the flag service. If the service is slow, Polly retries; if it's dead, the client returns `false`.

### Step 3: The API (FeatureFlagService.API)
The API receives the request and sends an `EvaluateFlagQuery` into the **MediatR** pipeline.

### Step 4: The Handler (FeatureFlagService.Application)
The `EvaluateFlagHandler`:
1. Checks `IFeatureFlagCache`.
2. If miss, calls `IFeatureFlagRepository`.
3. Gets the `FeatureFlag` domain entity.

### Step 5: The Domain Entity (FeatureFlagService.Domain)
The `FeatureFlag.IsEnabledFor(userId, role)` method executes the logic:
```csharp
if (!IsEnabled) return false;
if (AllowedUserIds.Contains(userId)) return true;
// ... hash logic ...
return (hash % 100) < RolloutPercentage;
```

### Step 6: The Response
The boolean result travels back up through the layers and is returned to the consuming service as a JSON response.

---

## Summary of Decisions

| Decision | Why? |
| :--- | :--- |
| **MediatR** | Decouples the API from the business logic. Makes testing easy. |
| **Stable Hash** | Avoids `string.GetHashCode()` which changes per process/restart. |
| **JSON Columns** | EF Core stores lists as JSON in Postgres to avoid complex join tables for simple lists. |
| **Interfaces** | Allows swapping `InMemoryCache` for `Redis` or `Postgres` for `SQL Server` with zero logic changes. |
