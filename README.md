# Employee Management System

A full-stack Employee Management System built with ASP.NET Core Web API, React, TypeScript, PostgreSQL, and Docker. The application provides employee management functionality with JWT-based authentication and a layered backend architecture.

## Tech Stack

### Backend
- ASP.NET Core Web API (.NET 10)
- Entity Framework Core
- PostgreSQL
- ASP.NET Core Identity
- JWT authentication and authorization
- RESTful API architecture

### Frontend
- React
- TypeScript
- Vite

### Database and Deployment
- PostgreSQL 16
- Docker and Docker Compose
- Nginx
- Git and GitHub
- Render deployment preparation

## Features

- User registration and login
- JWT-based authentication
- Protected employee API endpoints
- Create and manage employee records
- Employee information including name, department, salary, and active status
- Entity Framework Core database migrations
- PostgreSQL database persistence
- Containerized application services
- React frontend served through Nginx
- API reverse proxy configuration for local Docker deployment

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

## Prerequisites

Install the following tools:

- [.NET SDK](https://dotnet.microsoft.com/download)
- [Node.js and npm](https://nodejs.org/)
- [Docker Desktop](https://www.docker.com/products/docker-desktop/)
- [Git](https://git-scm.com/)

Docker Desktop must be running before starting the containerized application.

## Getting Started

### 1. Clone the repository

```bash
git clone https://github.com/PriyaMano19/employee-management.git
cd employee-management
```

### 2. Configure environment variables

Create a `.env` file in the project root:

```env
POSTGRES_PASSWORD=your_strong_database_password
JWT_KEY=your_long_random_jwt_signing_key
```

Use strong, unique values. Keep `.env` private and never commit it to GitHub.

The repository's `.gitignore` is configured to exclude `.env` files.

### 3. Start the application with Docker

Build and start the services:

```bash
docker compose up -d --build
```

Check the service status:

```bash
docker compose ps
```

The application consists of three services:

| Service | Purpose |
|---|---|
| `frontend` | React application served by Nginx |
| `api` | ASP.NET Core Web API |
| `db` | PostgreSQL database |

### 4. Apply database migrations

The database schema must be created before using the application. The project uses Entity Framework Core migrations.

For the Docker environment, apply the existing migrations using the configured PostgreSQL connection and the project's EF Core tooling. The required migrations include the initial schema and Identity user tables.

Verify that the database contains the expected tables:

```bash
docker compose exec db \
  psql -U employee_app -d employee_management -c '\dt'
```

### 5. Open the application

Once the services are running, open:

**Frontend:** http://localhost:8080

The Nginx configuration forwards `/api/` requests to the API container.

## API Endpoints

The API base URL in the local Docker deployment is accessed through:

```text
http://localhost:8080/api
```

| Method | Endpoint | Purpose |
|---|---|---|
| POST | `/api/auth/register` | Register a user |
| POST | `/api/auth/login` | Authenticate a user |
| GET | `/api/employees` | Retrieve employee records |
| POST | `/api/employees` | Create an employee record |

The employee endpoints require JWT authentication. Send the access token in the HTTP `Authorization` header:

```http
Authorization: Bearer <your_access_token>
```

The exact request and response fields are defined by the API DTOs and controller implementations.

## Database

The application uses PostgreSQL with Entity Framework Core and Npgsql.

The database schema is managed through migrations, including:

- Employee records
- ASP.NET Core Identity users and related authentication tables
- EF Core migration history

Docker Compose uses a named volume to persist PostgreSQL data across container restarts.

## Frontend Configuration

The React application uses the `VITE_API_URL` environment variable to configure its API base URL.

For the Docker deployment, the frontend is built with:

```text
VITE_API_URL=/api
```

This allows Nginx to proxy API requests to the backend container.

For a separate frontend deployment, configure `VITE_API_URL` to the public API base URL, ending in `/api`. The backend CORS policy must also allow the deployed frontend's origin.

Vite environment variables are embedded at build time, so update the variable and rebuild or redeploy the frontend when its value changes.

## Running Without Docker

### Backend

From the repository root:

```bash
cd EmployeeManagement.Api
dotnet restore
dotnet build
dotnet run
```

Configure a valid PostgreSQL connection string and JWT settings through local configuration or environment variables before running the API.

### Frontend

Open a separate terminal:

```bash
cd employee-management-ui
npm ci
npm run dev
```

Configure `VITE_API_URL` to point to the running API. The API CORS policy must allow the frontend development origin.

## Testing

The repository contains an `EmployeeManagement.Tests` project.

Run the test suite from the repository root:

```bash
dotnet test
```

## Deployment

The application is being prepared for deployment using Render.

The intended deployment architecture is:

- **PostgreSQL:** Render managed PostgreSQL database
- **Backend:** Render Web Service using the API Dockerfile
- **Frontend:** Render Static Site using the React/Vite build

Production deployment requires configuring the database connection string, JWT signing key, API port, frontend API URL, and CORS allowed origins through the deployment platform's environment settings.

Do not commit production credentials or connection strings to the repository.

## Security Notes

- Keep database credentials and JWT signing keys out of source control.
- Use HTTPS for public deployments.
- Restrict CORS to trusted frontend origins.
- Protect employee endpoints with authentication and authorization.
- Use strong, unique secrets in each environment.
- Review logging and data protection configuration before production use.

## License

No license has been specified for this repository yet.
