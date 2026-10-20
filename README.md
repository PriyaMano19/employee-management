# Employee Management System

A full-stack Employee Management System built with **ASP.NET Core Web API, React, TypeScript, PostgreSQL, and Docker**. The application demonstrates secure authentication, RESTful API development, database persistence, and containerized deployment architecture.



## Overview

The Employee Management System provides a web-based interface for managing employee information through a React frontend and an ASP.NET Core REST API. It uses PostgreSQL for persistent storage and JWT-based authentication to protect employee-related operations.

The project follows a layered backend structure that separates API controllers, business logic, data access, and data transfer objects.

## Technology Stack

| Layer | Technologies |
|---|---|
| Frontend | React, TypeScript, Vite |
| Backend | ASP.NET Core Web API, .NET 10, C# |
| Authentication | ASP.NET Core Identity, JWT |
| Database | PostgreSQL, Entity Framework Core, Npgsql |
| API Design | RESTful APIs, DTOs |
| Containerization | Docker, Docker Compose |
| Web Server | Nginx |
| Version Control | Git, GitHub |
| Deployment | Render — planned deployment architecture |

## Key Features

- User registration and login.
- JWT-based authentication and protected API endpoints.
- Employee record management.
- Employee attributes including name, department, salary, and active status.
- RESTful API endpoints for employee operations.
- Entity Framework Core migrations for database schema management.
- PostgreSQL integration and persistent database storage.
- React and TypeScript frontend.
- Nginx configuration for frontend hosting and API reverse proxying.
- Docker Compose orchestration for the frontend, API, and database.
- Environment-based configuration for sensitive credentials.

## Architecture

```text
React + TypeScript Frontend
          |
          | HTTP / REST API
          v
       Nginx Proxy
          |
          v
 ASP.NET Core Web API
          |
     JWT Authentication
          |
          v
 Entity Framework Core
          |
          v
       PostgreSQL
```

For a public deployment, the frontend and API can run as separate services, with a managed PostgreSQL database. The frontend communicates with the public API URL, and the API restricts cross-origin requests to the configured frontend origin.

## Project Structure

```text
employee-management/
├── EmployeeManagement.Api/
│   ├── Controllers/
│   ├── Data/
│   ├── DTOs/
│   ├── Models/
│   ├── Services/
│   ├── Migrations/
│   ├── Dockerfile
│   ├── Program.cs
│   └── appsettings.json
├── EmployeeManagement.Tests/
├── employee-management-ui/
│   ├── src/
│   ├── Dockerfile
│   ├── nginx.conf
│   └── package.json
├── docker-compose.yml
├── .gitignore
└── README.md
```

## API Endpoints

The API exposes authentication and employee management endpoints.

| HTTP Method | Endpoint | Description |
|---|---|---|
| POST | `/api/auth/register` | Register a user |
| POST | `/api/auth/login` | Authenticate a user |
| GET | `/api/employees` | Retrieve employee records |
| POST | `/api/employees` | Create an employee record |

Employee endpoints require a valid JWT access token.

```http
Authorization: Bearer <access_token>
```

## Local Setup

### Prerequisites

- .NET SDK 10
- Node.js and npm
- Docker Desktop
- Git

### Clone the repository

```bash
git clone https://github.com/PriyaMano19/employee-management.git
cd employee-management
```

### Configure environment variables

Create a `.env` file in the repository root:

```env
POSTGRES_PASSWORD=your_strong_database_password
JWT_KEY=your_long_random_jwt_signing_key
```

Do not commit this file or expose production secrets.

### Run with Docker Compose

```bash
docker compose up -d --build
```

Check service status:

```bash
docker compose ps
```

Open the frontend at:

http://localhost:8080

The application uses PostgreSQL in Docker, and the database schema must be initialized using the existing Entity Framework Core migrations.

### Run the backend independently

```bash
cd EmployeeManagement.Api
dotnet restore
dotnet build
dotnet run
```

Provide a valid PostgreSQL connection string and JWT configuration before starting the API.

### Run the frontend independently

```bash
cd employee-management-ui
npm ci
npm run dev
```

Configure `VITE_API_URL` to point to the running API. The API's CORS configuration must allow the frontend's origin.

## Database Management

The database schema is managed with Entity Framework Core migrations. PostgreSQL stores employee information and ASP.NET Core Identity data.

For production, use a managed database and securely configure the database connection string through the deployment platform's environment variables.

## Deployment Configuration

The intended cloud architecture uses:

- **Frontend:** Render Static Site.
- **Backend:** Render Web Service using the API Dockerfile.
- **Database:** Render managed PostgreSQL.

Required production configuration includes:

- `ConnectionStrings__DefaultConnection`
- `Jwt__Key`
- `Jwt__Issuer`
- `Jwt__Audience`
- `VITE_API_URL` for the frontend build.
- An API CORS policy allowing the deployed frontend origin.

Production deployment also requires applying database migrations, validating authentication, and verifying that the deployed frontend can communicate with the API over HTTPS.

## Security Considerations

- JWT-based authentication for protected endpoints.
- Secrets supplied through environment configuration rather than committed source files.
- CORS restricted to trusted frontend origins.
- HTTPS for public application traffic.
- Database credentials kept separate from application source code.

