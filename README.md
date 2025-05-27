[![Continuous Integration](https://github.com/giovanoh/library-api/actions/workflows/ci.yml/badge.svg)](https://github.com/giovanoh/library-api/actions/workflows/ci.yml)
[![Continuous Deployment](https://github.com/giovanoh/library-api/actions/workflows/cd.yml/badge.svg)](https://github.com/giovanoh/library-api/actions/workflows/cd.yml)

---

# library-api

> :globe_with_meridians: Read this in other languages: [Português (Brasil)](README.pt-br.md)

Prototype of a .NET 8 Web API for a Library system

## About

This project demonstrates how to build a RESTful API using **.NET 8**, following modern best practices for architecture, testing, and maintainability.  
It simulates a simple library system with authors and books, and is designed for learning and demonstration purposes.

In addition to traditional CRUD operations, the project features an event-driven checkout workflow (EDA) using RabbitMQ and MassTransit, enabling asynchronous order processing and service decoupling.

## Table of Contents

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
- [Continuous Integration and Deployment (CI/CD)](#continuous-integration-and-deployment-cicd)
- [Observability with Jaeger, Prometheus, Loki and Grafana](#observability-with-jaeger-prometheus-loki-and-grafana)
  - [Start all observability services](#start-all-observability-services)
  - [Stop the services](#stop-the-services)
  - [Observability - Main packages used](#observability---main-packages-used)
  - [Containers summary](#containers-summary)
- [EDA: Messaging and Event-Driven Architecture](#eda-messaging-and-event-driven-architecture)
- [Architecture Overview](#architecture-overview)
- [Example: Checkout Event Flow](#example-checkout-event-flow)
- [Observability: Tracing & Metrics](#observability-tracing--metrics)
- [License](#license)

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
- **Event-Driven Architecture (EDA)** using RabbitMQ and MassTransit for asynchronous communication between services
- **Message-based workflow** for checkout and order processing
- **Separation of concerns** and SOLID principles
- **Environment-based configuration** via `appsettings.json`
- **Lowercase URLs** and JSON serialization options for cleaner APIs
- **Richardson Maturity Model**: This API reaches **Level 2** of the Richardson Maturity Model, meaning it uses distinct resources and proper HTTP verbs (GET, POST, PUT, DELETE) for each operation. However, it does not yet implement HATEOAS (Level 3), which would include hypermedia links in responses to guide clients dynamically. See [Richardson Maturity Model](https://martinfowler.com/articles/richardsonMaturityModel.html) for more details.
- **Unified Response Pattern**: All successful responses follow the `ApiResponse<T>` format, ensuring consistency and predictability for API consumers. All error responses follow the [RFC 7807](https://datatracker.ietf.org/doc/html/rfc7807) standard using the `ApiProblemDetails` object, making error handling standardized and interoperable.

## Getting Started

### Prerequisites

- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)

### Installing dependencies

Before running the API for the first time, restore the project dependencies:

```sh
dotnet restore
```

### 1. Run Only the API (Authors/Books)
For quick local development or unit testing of authors and books:
```sh
dotnet run --project src/Library.API/Library.API.csproj
```
The API will be available at https://localhost:5001.

> **Note:** Order/checkout endpoints require RabbitMQ and the full stack running.

### 2. Run the Full Stack (Recommended for Orders/Checkout)
To test the event-driven order flow, messaging, and observability:
```sh
docker compose up --build
```
This will start the API, Checkout worker, RabbitMQ, and all observability tools.

### API Documentation

The API documentation is available via Swagger UI at:
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
- Create a book order (checkout)
- Check the status of a book order

### Running Code Coverage

The commands below will generate a code coverage report in the `coveragereport` directory at the root of the project. You can view the coverage results by opening the `index.html` file in that directory with your web browser.

```sh
dotnet test --collect:"XPlat Code Coverage" --settings .coverlet.runsettings
reportgenerator -reports:"tests/**/TestResults/**/coverage.opencover.xml" -targetdir:coveragereport -reporttypes:Html
```

### Project Structure

```
src/Library.API/                    # Main API project
  Controllers/                      # API endpoints
  Domain/                           # Domain models, repositories, services
  DTOs/                             # Data Transfer Objects
  Extensions/                       # Extension methods and helpers
  Infrastructure/                   # EF Core context, repositories, services, middlewares
  Mapping/                          # AutoMapper profiles
  Validation/                       # Custom validation attributes
src/Library.Events/                 # Event contracts (messages shared between services)
src/Library.Checkout/               # Worker service for processing order events
tests/Library.API.Tests/            # Unit tests
tests/Library.API.IntegrationTests/ # Integration tests
observability/                      # Observability configs (Grafana dashboards, Prometheus, Loki, provisioning)
```

### Continuous Integration and Deployment (CI/CD)

#### Workflow Overview

The project implements a comprehensive and automated Continuous Integration and Continuous Deployment (CI/CD) pipeline using GitHub Actions:

##### Continuous Integration (CI)
- **Triggers**: 
  - Pushes to `main` branch
  - Pushes to `docs/*`, `feature/*`, `refactor/*`, and `test/*` branches
  - Pull requests to `main` branch

- **Build and Test Process**:
  - Sets up .NET 8 SDK
  - Restores project dependencies
  - Builds the project in Release configuration
  - Runs comprehensive test suites:
    - Unit Tests
    - Integration Tests
  - Generates code coverage reports
  - Uploads coverage reports to Codecov for tracking

##### Continuous Deployment (CD)
- **Trigger**: Successful completion of Continuous Integration workflow on `main` branch

- **Docker Image Publishing**:
  - Builds a Docker image for the Library API
  - Publishes image to Docker Hub
  - Generates multiple tags:
    - Branch-specific tags
    - Semantic versioning tags
    - `latest` tag

#### Key Benefits
- Automated testing for every code change
- Consistent build and deployment process
- Immediate feedback on code quality
- Automatic Docker image generation
- Code coverage tracking

### Observability with Jaeger, Prometheus, Loki and Grafana

To run the API together with Jaeger, Prometheus, Loki, Promtail, Grafana, and the Otel Collector, use the `docker-compose.yml` file:

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

#### Containers summary

- **otel-collector**: Receives traces, metrics, and logs from the services and forwards them to Jaeger, Prometheus, Loki, and other observability tools.
- **rabbitmq**: Message broker used for event-driven communication between services (management UI at http://localhost:15672, default user/password: guest/guest).
- **library-api**: Main .NET API, exposes REST endpoints, metrics, and logs.
- **jaeger**: Collector and visualizer for distributed traces (OpenTelemetry/Jaeger).
- **prometheus**: Metrics collector and database, scrapes the API's /metrics endpoint.
- **loki**: Backend for storing and indexing structured logs.
- **promtail**: Log collector, reads logs from containers and sends them to Loki.
- **grafana**: Centralized visualization for metrics, logs, and traces (dashboards, queries, alerts).

#### Observability - Main packages used

- `OpenTelemetry.Exporter.OpenTelemetryProtocol` (for Jaeger via OTLP)
- `OpenTelemetry.Exporter.Prometheus.AspNetCore` (for Prometheus)
- `OpenTelemetry.Extensions.Hosting`
- `OpenTelemetry.Instrumentation.AspNetCore`
- `OpenTelemetry.Instrumentation.Http`
- `OpenTelemetry.Instrumentation.MassTransit` (for distributed tracing of messaging)
- `Serilog.AspNetCore` and `Serilog.Enrichers.Span` (structured logs)

### Architecture Overview

The system is based on Event-Driven Architecture (EDA) using RabbitMQ and MassTransit. The main flow is:

The event-driven workflow relies on RabbitMQ as the central message broker, running as a container in the stack.

```
[Client] ---> [Library.API] --(OrderPlacedEvent)--> [RabbitMQ] --(OrderPlacedEvent)--> [Library.Checkout]
    ^             |                                                    |
    |             |<--(Status events: PaymentConfirmed,                |
    |             |    OrderProcessing, OrderShipped, OrderDelivered,  |
    |             |    OrderCompleted, PaymentFailed)                  |
    |             |                                                    |
    |<-------------------(Status update via API)-----------------------|
```

- **Library.API**: Exposes REST endpoints, publishes events to RabbitMQ, and updates order status based on events.
- **Library.Checkout**: Worker service that consumes events from RabbitMQ, processes business logic (payment, shipping), and emits new events.
- **Library.Events**: Shared project with event/message contracts.

#### EDA: Messaging and Event-Driven Architecture - Main packages used

- `MassTransit` (core library for distributed application messaging)
- `MassTransit.RabbitMQ` (RabbitMQ transport for MassTransit)
- `OpenTelemetry.Instrumentation.MassTransit` (distributed tracing for messaging)

## Example: Checkout Event Flow

Before creating a book order, you need to:

1. Create an Author
2. Create a Book (using the authorId from the previous response)

After that, you can:

- Create a Book Order (using the bookId from the previous response)
- The API will publish an OrderPlacedEvent to RabbitMQ
- The Checkout service will process the event and emit the following events: PaymentConfirmed, PaymentFailed, OrderProcessing, OrderShipped, OrderDelivered, and OrderCompleted.
- You can check the order status via the API

## Observability: Tracing & Metrics

- **Jaeger**: http://localhost:16686 — visualize distributed traces for each order event.
- **Grafana**: http://localhost:3000 — dashboards for metrics and traces.
- **Prometheus**: http://localhost:9090 — raw metrics.

## License

MIT
