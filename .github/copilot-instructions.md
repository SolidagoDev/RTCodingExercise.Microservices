# Copilot Instructions & Best Practices

This solution is a .NET 6 microservices-based application using ASP.NET Core, Entity Framework Core, Docker, Serilog, Polly, MassTransit and xUnit. Follow these best practices when using GitHub Copilot or contributing code:

## General Guidelines

- **Follow SOLID principles** for maintainable and testable code.
- **Use dependency injection** for all services and repositories.
- **Prefer async/await** for I/O-bound operations.
- **Write unit tests** for all new features and bug fixes.
- **Keep controllers thin**; move business logic to services.
- **Use configuration files** (e.g., `appsettings.json`) for environment-specific settings.
- **Document public APIs** and complex logic with XML comments.
- **Use Git for version control**; commit often with clear messages.
- **Use feature branches** for new development; merge to main only after code review.
- **Use semantic versioning** for releases (e.g., v1.0.0).

## Microservices & API

- **Each microservice** should have its own API, domain, and unit test projects.
- **Use DTOs/ViewModels** for API input/output, not domain entities.
- **Validate input** using model validation attributes.
- **Return appropriate HTTP status codes** in controllers.
- **Handle exceptions globally** using middleware.
- **Use Swagger/OpenAPI** for API documentation.
- **Use API versioning** to manage breaking changes.
- **Use asynchronous programming** for API endpoints to improve scalability.
- **Use HTTP caching** where appropriate to reduce load on services.

## Entity Framework Core

- **Use migrations** for database schema changes.
- **Avoid business logic in DbContext**; keep it in domain/services.
- **Use `AsNoTracking()`** for read-only queries.
- **Seed data** using the provided boilerplate pattern.
- **Use repository pattern** for data access to abstract EF Core specifics.
- **Use `IQueryable`** for queryable data access, but avoid exposing it directly in APIs.
- **Use `Include` for eager loading** related entities when necessary.
- **Use `ConfigureAwait(false)`** in library code to avoid deadlocks in UI applications.
- **Infer the database structure from the domain model**; avoid using raw SQL unless necessary.

## Logging & Resilience

- **Use Serilog** for structured logging.
- **Log at appropriate levels** (Information, Warning, Error).
- **Use MassTransit** for inter-service communication where possible.
- **Use Polly** for transient fault handling (retries, circuit breakers).

## Docker & Deployment

- **Use Dockerfiles** for each service.
- **Keep images small** by using multi-stage builds.
- **Use docker-compose** for local development orchestration.
- **Store secrets securely** (never commit secrets to source control).

## Testing

- **Use xUnit** for unit tests.
- **Mock dependencies** using Moq or similar libraries.
- **Test both positive and negative cases**.
- **Follow the pattern**: for each service, create a corresponding UnitTests project.

## Naming & Structure

- **Follow .NET naming conventions** (PascalCase for types, camelCase for variables).
- **Organize code** by feature/folder (API, Domain, UnitTests).
- **Keep files small and focused**.
- **Use meaningful names** for classes, methods, and variables.

## Pull Requests & Code Reviews

- **Write clear commit messages**.
- **Run all tests** before submitting a PR.
- **Request reviews** for all non-trivial changes.

## References

- [ASP.NET Core Documentation](https://docs.microsoft.com/aspnet/core/)
- [Entity Framework Core Documentation](https://docs.microsoft.com/ef/core/)
- [Serilog Documentation](https://serilog.net/)
- [Polly Documentation](https://github.com/App-vNext/Polly)
- [xUnit Documentation](https://xunit.net/)

---

_This file is intended for use with GitHub Copilot and as a reference for developers contributing to the project. It outlines best practices and guidelines to ensure code quality and maintainability._