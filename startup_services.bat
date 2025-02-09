@echo off
setlocal

echo [STARTING] Initializing Keycloak authentication service...
start "Keycloak" cmd /k "cd keycloak\bin && call kc.bat start-dev"
timeout /t 10 /nobreak

echo [STARTING] Launching ASP.NET Core backend service...
start "Backend" cmd /k "cd backend\Library.Presentation && call dotnet run --launch-profile https"
timeout /t 15 /nobreak

echo [STARTING] Initializing React.js frontend application...
start "Frontend" cmd /k "cd frontend && call npm install && call npm start"

echo [SUCCESS] All services are now starting. Please wait...
echo [INFO] Check individual service windows for detailed logs.

pause
