# Education Provider Registry — Core
This repository currently implements the query side of the Education Provider Registry using Clean Architecture, modular dependency injection, and a fully containerised integration‑testing pipeline. The solution is designed for reliability, determinism, maintainability, and ease of extension.

## 1. Architecture Overview
The solution follows Clean Architecture, separating concerns into distinct layers:

* Application Layer (Use Cases, Request/Response Models)
* Domain Layer (Entities, Value Objects, Domain Rules)
* Infrastructure Layer (Repositories, Mappers, Postgres Access)
* Integration Test Infrastructure (Testcontainers, Database Factories)

Each layer depends only on the layer beneath it, ensuring a strict and maintainable dependency flow.

## 2. Application Layer
The Application layer orchestrates domain logic and infrastructure access. It contains use cases, request/response models, and interfaces for repositories and services.

### 2.1 Use Case Pattern
All use cases implement:
```
IUseCase<TRequest, UseCaseResponse<TResponse>>
```
Example: GetEstablishments
* Request: GetEstablishmentsRequest
* Response: UseCaseResponse<IReadOnlyCollection<Establishment>>
* Implementation: GetEstablishmentsUseCase

The use case accepts a request, calls the repository, maps results, and returns a response.

### 2.2 DI Registration
```
services.AddEstablishmentsUseCaseDependencies()
```
This registers the use case, required mappers, and supporting services.

## 3. Domain Layer
The Domain layer contains the core business logic and has no dependencies on other layers.

It includes the Establishment entity, value objects, and domain invariants.
The domain is pure and testable without infrastructure.

## 4. Infrastructure Layer
The Infrastructure layer implements the interfaces defined by the Application layer.

It includes:

* IEstablishmentsRepository
* Postgres repository implementation
* Mappers
* Data access logic

### 4.1 DI Registration
```
services.AddEstablishmentsInfrastructureDependencies()
```
This ensures the application layer receives the correct concrete implementations.

## 5. Testing Strategy
The solution includes:

* Unit tests
* DI composition tests
* Full‑graph resolution tests
* Integration tests using real Postgres containers

### 5.1 DI Assertion Helpers
ServiceCollectionAssertionExtensions provides fluent assertions such as:
```
services.ShouldContain<IService, Impl>(ServiceLifetime.Scoped)
```
This ensures DI correctness without brittle test code.

## 6. Integration Testing Infrastructure
The integration test framework provides fully isolated Postgres containers, strongly typed configuration, automatic startup and disposal, scoped execution helpers, and deterministic behaviour.

### 6.1 IntegrationTestBase
IntegrationTestBase provides:

* Test lifecycle management
* Configuration merging
* Application DI bootstrapping
* Scoped execution helpers
* Database lifecycle hooks

This is the foundation for all integration tests.

## 7. Postgres Testcontainers Pipeline
The Postgres integration system is modular and extensible.

### 7.1 Options
PostgresContainerOptions defines container‑level configuration:

• Image name, tag, digest
• Static or random host port
• Server arguments
• Mounted resources

PostgresDatabaseOptions defines database‑level configuration:

• Database name
• Username
• Password

### 7.2 Container Factory
IContainerFactory defines a simple abstraction for creating Testcontainers containers.

PostgresBuilderContainerFactory builds a fully configured Postgres container by applying:

* Image
* Ports
* Startup commands
* Mounted resources
* Clean‑state enforcement

### 7.3 Builder Extensions
PostgresSqlBuilderExtensions encapsulates common builder configuration:

* WithExposedPorts
* WithStartupCommands
* WithMountedResources

These keep container setup consistent and reusable.

### 7.4 Database Wrapper
PostgresContainerDatabase implements IDatabase over a Testcontainers Postgres instance. It provides:

* Thread‑safe startup
* Lazy container creation
* Connection string generation
* SQL execution
* Clean disposal

### 7.5 Database Factory
PostgresDatabaseFactory implements IDatabaseFactory and creates a new PostgresContainerDatabase per test, ensuring full isolation.

### 7.6 DI Registration
AddPostgresDatabase wires everything into IServiceCollection:

* PostgresContainerOptions
* PostgresDatabaseOptions
* IContainerFactory
* IDatabaseFactory
* Resolved option values

This is the entry point for Postgres integration.

## 8. End‑to‑End Integration Test Flow
StartTestAsync
* IDatabaseFactory.CreateAsync
* PostgresContainerDatabase.StartAsync
* Container starts
* Connection string generated
* Application DI built
* Test executes
* Database disposed
* Container removed

Every test is fresh, isolated, deterministic, and reproducible.

## 9. Summary
This solution provides:

* A clean, modular architecture
* Strong DI boundaries
* Fully isolated Postgres integration tests
* Deterministic behaviour
* High maintainability
* Enterprise‑grade test infrastructure

It is a complete, production‑ready Clean Architecture implementation with a robust, container‑based testing pipeline.

## Dependencies
- .NET 10 required
