# Project Requirements Document (prd.md)

## Project: Registration Number Plate Search System

## Overview

This project is a .NET 6 microservices-based application for managing and marketing vehicle registration number plates. The solution demonstrates clean, readable, and maintainable code with a test-first approach, solid code coverage, and adherence to SOLID design principles.

---

## Objectives

- Deliver a maintainable, scalable, and testable microservices solution.
- Apply SOLID principles and best practices throughout the codebase.
- Provide evidence of a test-first approach and meaningful unit test coverage.
- Enable easy extension and modification for future requirements.

---

## Technical Requirements

- **.NET 6 SDK**  
  [Download](https://dotnet.microsoft.com/en-us/download/dotnet/6.0)
- **IDE**  
  Visual Studio Community ([Download](https://visualstudio.microsoft.com/vs/community/)) or Visual Studio Code ([Download](https://code.visualstudio.com/))
- **Docker Desktop** (required for microservices)  
  [Download](https://www.docker.com/products/docker-desktop)
- **MassTransit** and **RabbitMQ** for message bus communication  
  [MassTransit Documentation](https://masstransit-project.com/)
- **xUnit** for unit testing (nUnit may be used if preferred)
- **Serilog** for structured logging
- **Polly** for resilience and transient fault handling
- **Swagger/OpenAPI** for API documentation

---

## Solution Structure

- ASP.NET 6 MVC Project
- ASP.NET 6 API Project for a Catalogue of Plates
- DbContext and Plate Model with Seed Method for sample data
- xUnit Test Project for each service
- Docker Compose file for orchestration
- MassTransit and RabbitMQ message bus integration

---

## Functional Requirements

### User Story 1: List Plates for Sale

**As** head of commercial operations  
**I want** to build a list of available plates for sale  
**So that** we can market them to the public

**Acceptance Criteria:**
- Output a list showing Plate, PurchasePrice, and SalePrice.
- Adding a new plate updates the list.
- SalePrice includes a 20% markup.
- [Advanced] Paginate large lists (e.g., 60,000,000 plates), showing 20 plates per page.

---

### User Story 2: Order Plates by Price

**As** head of marketing  
**I want** to order the list of plates by price  
**So that** customers can view plates in their price range

**Acceptance Criteria:**
- Ability to order the list by price.
- Plates are displayed in price order.

---

### User Story 3: Filter Plates by Numbers or Letters

**As** head of marketing  
**I want** to filter the list of plates by numbers or letters  
**So that** customers can find plates matching their initials or age

**Acceptance Criteria:**
- Filter plates by age or initials.
- Display plates containing the filter criteria.
- [Advanced] Name-based filtering (e.g., "Danny" matches "DA12 NNY").

---

### User Story 4: Reserve Plates

**As** head of commercial operations  
**I want** to mark plates as reserved  
**So that** we are not selling plates that are reserved

**Acceptance Criteria:**
- Mark plates as reserved.
- Reserved status is visible in search results.
- [Advanced] Log reservation/unreservation actions for audit.

---

### User Story 5: Filter Only Plates for Sale

**As** head of commercial operations  
**I want** only plates “for sale” to appear in filtered search  
**So that** we do not confuse customers

**Acceptance Criteria:**
- Only plates for sale are shown in search results.

---

### User Story 6: Sell a Plate

**As** sales director  
**I want** to sell a plate  
**So that** we can start making revenue

**Acceptance Criteria:**
- Mark plate as sold when purchased.
- Sold status is visible in search results.
- Total revenue label is incremented when a plate is sold.
- [Advanced] Calculate and display average profit margin for all sales.

---

### User Story 7: Promotions and Discounts

**As** head of marketing  
**I want** to offer money off promotions  
**So that** we can improve sales

**Acceptance Criteria:**
- Promo code "DISCOUNT" applies a £25 discount.
- [Advanced] Promo code "PERCENTOFF" applies a 15% discount.

---

### User Story 8: Discount Control

**As** head of marketing  
**I want** to ensure plates are not sold under 90% of the sale price  
**So that** we can control discount codes

**Acceptance Criteria:**
- Prevent sales under 90% of sale price when using discounts.
- Advise customer if discount code is not applicable.

---

## Non-Functional Requirements

- **SOLID Principles:** All code must adhere to SOLID design principles.
- **Dependency Injection:** Use for all services and repositories.
- **Async/Await:** Prefer for I/O-bound operations.
- **Unit Testing:** All features and bug fixes must have unit tests.
- **Thin Controllers:** Move business logic to services.
- **Configuration:** Use `appsettings.json` for environment-specific settings.
- **API Documentation:** Use Swagger/OpenAPI.
- **API Versioning:** Support future changes.
- **HTTP Caching:** Use where appropriate.
- **Entity Framework Core:** Use migrations, repository pattern, and avoid business logic in DbContext.
- **Logging:** Use Serilog with appropriate log levels.
- **Resilience:** Use Polly for retries and circuit breakers.
- **Inter-Service Communication:** Use MassTransit and RabbitMQ.
- **Docker:** Use Dockerfiles and docker-compose for orchestration.
- **Secrets Management:** Never commit secrets to source control.
- **Naming & Structure:** Follow .NET conventions and organize by feature/folder.
- **Version Control:** Use Git, feature branches, and semantic versioning.
- **Pull Requests:** Run all tests and request reviews for non-trivial changes.

---

## References

- [ASP.NET Core Documentation](https://docs.microsoft.com/aspnet/core/)
- [Entity Framework Core Documentation](https://docs.microsoft.com/ef/core/)
- [Serilog Documentation](https://serilog.net/)
- [Polly Documentation](https://github.com/App-vNext/Polly)
- [xUnit Documentation](https://xunit.net/)
- [MassTransit Documentation](https://masstransit-project.com/)

---

_This PRD outlines the functional and non-functional requirements for the Registration Number Plate Search System. All contributors must adhere to these requirements to ensure code quality, maintainability, and scalability._