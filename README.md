# DistributedBank Microservices Project

A distributed microservices banking platform built using C\#, .NET 8, ASP.NET Core, and PostgreSQL. The system is designed with Domain-Driven Design (DDD), Command Query Responsibility Segregation (CQRS), and Clean Architecture, ensuring modularity, scalability, and maintainability.

---

## Overview

This project simulates a comprehensive banking platform with the following microservices:
- **CustomerService:** Manages customer information.
- **AccountService:** Handles account operations.
- **TransactionService:** Processes funds transfers and maintains transaction records.

An **Ocelot API Gateway** centralizes client request routing and abstracts cross-cutting concerns. The system leverages containerization and CI/CD pipelines for efficient, automated deployments, with robust logging, tracing, and resilience mechanisms.

---

## Key Technologies

- **Languages & Frameworks:** C\#, .NET 8, ASP.NET Core, Entity Framework Core
- **Database:** PostgreSQL
- **Architecture:** DDD, CQRS, Clean Architecture
- **API Gateway:** Ocelot
- **Containerization:** Podman
- **CI/CD:** Azure DevOps / GitHub Actions
- **Observability & Resilience:** Elastic Stack, OpenTelemetry, Polly
- **Orchestration:** Kubernetes

---

## Features

- **Distributed Microservices:** Design and implementation of modular services for core banking functionalities.
- **Centralized API Gateway:** Seamless routing and cross-cutting management with Ocelot.
- **Automated CI/CD:** End-to-end pipelines for building, testing, and deploying microservices.
- **Robust Logging & Tracing:** End-to-end observability using Elastic Stack and OpenTelemetry.
- **Fault Tolerance:** Resilience strategies with Polly for enhanced system stability.
- **Containerization & Orchestration:** Deployment readiness with Podman and optional Kubernetes integration. (pending)

---

## Getting Started

### Prerequisites

- [.NET 8 SDK](https://dotnet.microsoft.com/download)
- [PostgreSQL](https://www.postgresql.org/download/)
- [Podman](https://podman.io/getting-started/) (or Docker if preferred)
- CI/CD tools (Azure DevOps or GitHub Actions)
- (Optional) A Kubernetes cluster

### Setup Instructions

1. **Clone the Repository:**
    ```bash
    git clone <repository-url>
    cd DistributedBank-Microservices
    ```

2. **Configure the Environment:**
   - Update database connection strings and other settings in the configuration files.
   - Create and configure an instance of PostgreSQL.

3. **Run Database Migrations:**
    ```bash
    dotnet ef database update
    ```

4. **Build and Run the Services:**
    ```bash
    dotnet build
    dotnet run
    ```

5. **Containerization & Deployment:**
   - Use the provided Podman configuration to containerize each microservice.
   - Set up and trigger CI/CD pipelines using Azure DevOps or GitHub Actions.
   - For Kubernetes deployment, refer to the orchestration scripts and documentation.

---

## Architecture

The system is architected into distinct microservices, each addressing specific business capabilities:
- The **Ocelot API Gateway** routes incoming requests to the appropriate service.
- Each microservice is independently deployable and maintains its own data store with PostgreSQL.
- The overall design follows best practices with clear separation of concerns and strong adherence to SOLID principles.
