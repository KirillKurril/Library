# Library Management System

## Project Structure
- Frontend: React.js application
- Backend: ASP.NET Web API
- Authentication: Keycloak
- Database: MS SQL Server
- Cache: Redis

## Prerequisites
- Docker and Docker Compose
- Node.js and npm
- .NET SDK 8.0 or later
- Windows 11 OS

## Docker Images
The project uses the following Docker images:
- SQL Server: `dmitrukbohdan/library-mssql` based on `mcr.microsoft.com/mssql/server:2022-latest`
- Redis: `dmitrukbohdan/library-redis` based on `redis:7.4.2`

## Quick Start
1. Clone the repository
```bash
git clone https://github.com/KirillKurril/Library.git
```

2. Go to the root directory of the repository and run docker containers 
```bash
docker compose up -d
```

3. Ensure docker containers are up and running correctly then run services in the same directory 
```bash
startup_services.bat
```

Or you can run the services manually

3.1 Start the backend (in HTTPS profile)
```bash
cd backend\Library.Presentation
dotnet run --launch-profile https
```

3.2 Start the frontend
```bash
cd frontend
npm install
npm start
```

3.3 Start Keycloak in development mode
```bash
cd keycloak\bin
kc.bat start-dev
```

The fronend server is running on "https://localhost:7021"
The backend server is running on "https://localhost:7020"
The keycloak server is running on "https://localhost:8080"


## Development Environment Configuration

### Keycloak Configuration
- URL: http://localhost:8080
- Admin Console: http://localhost:8080/admin
- Default credentials: 
  - Username: Kamidashi
  - Password: 1111

### Redis Configuration
- Host: localhost
- Port: 6379
- Password: Ec_fosJ61EEn78&?

### MS SQL Server Configuration
- Server: localhost
- Port: 1433
- Default credentials:
  - Username: aspnet-api
  - Password: g4q78@Rq1Q]2£%Wa


### Library users credentials 
- Admin:
  - Username: admin
  - Password: 1111
- Regular user
  - Username: reader1
  - Password: 1111  

  - Username: reader2
  - Password: 1111 

  - Username: reader3
  - Password: 1111 

### System requirements
The software was developed and tested on a device with the following specifications:
- Processor:  Intel(R) Core(TM) i5-13500H
- RAM: 16.0 GB
- System type: 64-bit operating system, x64-based processor
- OS: Windows 11 Pro 24H2
