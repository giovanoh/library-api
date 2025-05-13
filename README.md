## Table of Contents

- [library-api](#library-api)
  - [About](#about)
  - [Features & Best Practices](#features--best-practices)
  - [Getting Started](#getting-started)
    - [Prerequisites](#prerequisites)
    - [Installing dependencies](#installing-dependencies)
    - [Running the API](#running-the-api)
    - [API Documentation](#api-documentation)
  - [Automated Tests](#automated-tests)
  - [Manual API Testing](#manual-api-testing)
  - [Running Code Coverage](#running-code-coverage)
  - [Project Structure](#project-structure)
  - [Observability with Jaeger, Prometheus, Loki and Grafana](#observability-with-jaeger-prometheus-loki-and-grafana)
    - [Start all observability services](#start-all-observability-services)
    - [Stop the services](#stop-the-services)
    - [Observability - Main packages used](#observability---main-packages-used)
    - [Containers summary](#containers-summary)
  - [License](#license)

# library-api

> :globe_with_meridians: Read this in other languages: [Português (Brasil)](README.pt-br.md)

Prototype of a .NET 8 Web API for a Library system

## About

This project demonstrates how to build a RESTful API using **.NET 8**, following modern best practices for architecture, testing, and maintainability.  
It simulates a simple library system with authors and books, and is designed for learning and demonstration purposes.

## Features & Best Practices

- **.NET 8** with modern C# features
- **Traditional Web API with Controllers** for clear separation of concerns and scalability
- **Domain-driven structure** (Domain, Infrastructure, DTOs, etc.)
- **Dependency Injection** for services and repositories, making the code more testable and maintainable
- **In-memory database** using Entity Framework Core for easy setup and testing
- **AutoMapper** for mapping between domain models and DTOs
- **Custom validation attributes** and use of built-in validation
- **Consistent and centralized error handling** with RFC-compliant `ApiProblemDetails` responses
- **Unit and integration tests** (xUnit, Moq, FluentAssertions), with mocks to isolate dependencies
- **Automatic API documentation** with Swagger/OpenAPI
- **Separation of concerns** and SOLID principles
- **Environment-based configuration** via `appsettings.json`
- **Lowercase URLs** and JSON serialization options for cleaner APIs

## Getting Started

### Prerequisites

- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)

### Installing dependencies

Before running the API for the first time, restore the project dependencies:

```sh
dotnet restore
```

### Running the API

```sh
dotnet run --project src/Library.API/Library.API.csproj
```

The API will be available at `https://localhost:5001` (or as configured).

### API Documentation

After running, access Swagger UI at:  
`https://localhost:5001/swagger`

### Automated Tests

The project includes comprehensive test coverage. Unit and integration tests are located in the `tests/` directory.

#### Types of Tests
- **Unit Tests**: Validation of individual components and methods in isolation
- **Integration Tests**: Verification of interactions between different application components

#### Running Tests
To execute all tests, use the command:
```sh
dotnet test
```

### Manual API Testing

#### Important: Manual Tests vs Automated Tests

:warning: **Note**: This section describes manual tests for exploration and development. 
These tests are different from the **automated unit and integration tests** located in the `tests/` directory, 
which are run with `dotnet test`.

#### Testing Methods

There are two main ways to perform manual API testing:

##### 1. Swagger UI (Recommended for Interactive Development)

After starting the application, access Swagger UI at `https://localhost:5001/swagger`. 
This interactive interface enables:
- Exploration of available endpoints
- Direct request testing in the browser
- Data model visualization
- Custom parameter request sending

##### 2. Example Requests in `.http` File

The `src/Library.API/Library.API.http` file contains ready-to-use example requests. 
These are useful for quick manual tests and workflow documentation.

Recommended tools:
- REST Client extension in VS Code (like "REST Client" by Huachao Mao)
- Postman or Insomnia

Example of manual test workflow in the `.http` file:
- Create an author
- List authors
- Update an author
- Create a book for the author
- List books
- Update a book
- Remove book and author

#### Additional Tip

Ensure the API is running (`dotnet run`) before performing any manual tests.

### Running Code Coverage

The commands below will generate a code coverage report in the `coveragereport` directory at the root of the project. You can view the coverage results by opening the `index.html` file in that directory with your web browser.

```sh
dotnet test --collect:"XPlat Code Coverage"
reportgenerator -reports:"tests/**/TestResults/**/coverage.cobertura.xml" -targetdir:coveragereport -reporttypes:Html
```

### Project Structure

```
src/Library.API/           # Main API project
  Controllers/             # API endpoints
  Domain/                  # Domain models, repositories, services
  Infrastructure/          # EF Core context, repositories, services
  DTOs/                    # Data Transfer Objects
  Validation/              # Custom validation attributes
  Mapping/                 # AutoMapper profiles
tests/Library.API.Tests/   # Unit tests
tests/Library.API.IntegrationTests/ # Integration tests
observability/             # Observability configs (Grafana dashboards, Prometheus, Loki, provisioning)
```

### Observability with Jaeger, Prometheus, Loki and Grafana

To run the API together with Jaeger, Prometheus, Loki, Promtail, and Grafana, use the `docker-compose.yml` file:

#### Start all observability services

```sh
docker compose up --build
```

- Access the API at: http://localhost:5000 or https://localhost:5001
- Access Jaeger web interface (traces) at: http://localhost:16686
- Access Prometheus web interface (metrics) at: http://localhost:9090
- Access Grafana web interface (dashboards, logs, metrics, traces) at: http://localhost:3000 (default user/password: admin/admin)
- Access API metrics directly at: http://localhost:5000/metrics
- Access Loki API (logs) at: http://localhost:3100 (used by Grafana/Promtail)
- Access Promtail status (log collector) at: http://localhost:9080 (optional)

#### Stop the services

```sh
docker compose down
```

#### Observability - Main packages used

- `OpenTelemetry.Exporter.OpenTelemetryProtocol` (for Jaeger via OTLP)
- `OpenTelemetry.Exporter.Prometheus.AspNetCore` (for Prometheus)
- `OpenTelemetry.Extensions.Hosting`
- `OpenTelemetry.Instrumentation.AspNetCore`
- `OpenTelemetry.Instrumentation.Http`
- `Serilog.AspNetCore` and `Serilog.Enrichers.Span` (structured logs)

#### Containers summary

- **library-api**: Your main .NET API, exposes REST endpoints, metrics, and logs.
- **jaeger**: Collector and visualizer for distributed traces (OpenTelemetry/Jaeger).
- **prometheus**: Metrics collector and database, scrapes the API's /metrics endpoint.
- **loki**: Backend for storing and indexing structured logs.
- **promtail**: Log collector, reads logs from containers and sends them to Loki.
- **grafana**: Centralized visualization for metrics, logs, and traces (dashboards, queries, alerts).

## License

MIT
