# Feature Flag Service 🚀

A production-grade, high-performance Feature Flag Service built with **.NET 9**, **Clean Architecture**, and **CQRS**. This service allows you to control feature rollouts dynamically via userId, roles, or deterministic percentage-based targeting without redeploying code.

## ✨ Features

- **🎯 Granular Targeting**: Enable features for specific user IDs or roles.
- **📈 Deterministic Rollout**: Support 0-100% rollout using stable FNV hashing (same user always gets the same experience).
- **⚡ High Performance**: Cache-aside pattern with In-Memory (Phase 1) and Redis (Phase 2) support.
- **🛡️ Resilience**: Includes a thin client package with **Polly** (Retry + Circuit Breaker) for fail-safe integration.
- **🧹 Clean Architecture**: Strictly separated layers (Domain, Application, Infrastructure, API).
- **📝 API Documentation**: Fully documented with **Swagger/OpenAPI**.

## 🏗️ Technology Stack

| Concern | Technology |
| :--- | :--- |
| **Runtime** | ASP.NET Core 9 Web API |
| **Logic** | C# with MediatR (CQRS) |
| **ORM / DB** | Entity Framework Core + PostgreSQL |
| **Caching** | IMemoryCache / Redis |
| **Testing** | xUnit, Moq, FluentAssertions |
| **Resilience** | Polly |

## 📁 Project Structure

```text
feature-flag-service/
├── src/
│   ├── FeatureFlagService.API            # Controllers, DI, Middleware
│   ├── FeatureFlagService.Application    # Use cases (MediatR Handlers, DTOs)
│   ├── FeatureFlagService.Domain         # Entities, Repository Interfaces
│   ├── FeatureFlagService.Infrastructure # persistence (EF Core), Caching
│   └── FeatureFlagService.Client         # Thin HTTP Client with Resilience
└── tests/
    └── FeatureFlagService.Tests          # Unit & Application Tests
```

## 🚀 Getting Started

### Prerequisites

- [.NET 9 SDK](https://dotnet.microsoft.com/download/dotnet/9.0)
- [PostgreSQL](https://www.postgresql.org/)
- [Redis](https://redis.io/) (Optional for Phase 1)

### Setup

1. **Clone the repository**:
   ```bash
   git clone https://github.com/your-repo/feature-flag-service.git
   cd feature-flag-service
   ```

2. **Configure Database**:
   Update `src/FeatureFlagService.API/appsettings.json` with your PostgreSQL connection string:
   ```json
   "ConnectionStrings": {
     "DefaultConnection": "Host=localhost;Database=featureflags;Username=postgres;Password=yourpassword"
   }
   ```

3. **Run Migrations**:
   ```bash
   dotnet ef database update --project src/FeatureFlagService.Infrastructure --startup-project src/FeatureFlagService.API
   ```

4. **Run the API**:
   ```bash
   dotnet run --project src/FeatureFlagService.API
   ```

## 🔌 API Endpoints

| Method | Route | Description |
| :--- | :--- | :--- |
| `GET` | `/api/flags` | List all feature flags |
| `POST` | `/api/flags` | Create or update a flag |
| `GET` | `/api/flags/{key}/evaluate` | Evaluate a flag for a user/role |
| `DELETE` | `/api/flags/{key}` | Delete a flag |

## 📦 Client Integration

Consuming services can use the `FeatureFlagService.Client` package for easy integration:

```csharp
// Register in Program.cs
builder.Services.AddFeatureFlagClient(options => {
    options.BaseUrl = "http://localhost:5000";
    options.TimeoutSeconds = 2;
});

// Inject and use in your service
public class MyService(IFeatureFlagClient flags) 
{
    public async Task DoWork(string userId) 
    {
        if (await flags.IsEnabledAsync("new-ui-feature", userId)) {
            // New logic
        }
    }
}
```

## 🧪 Testing

Run all tests using the .NET CLI:
```bash
dotnet test
```

## 📜 License

MIT License.
