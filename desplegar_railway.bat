@echo off
chcp 65001 >nul 2>&1
title NavegaStudio - Deploy a Railway

echo ============================================
echo   NavegaStudio - Despliegue en Railway
echo ============================================
echo.

:: Verificar que railway CLI esta instalado
where railway >nul 2>&1
if %errorlevel% neq 0 (
    echo [ERROR] Railway CLI no esta instalado.
    echo Instalar con: npm install -g @railway/cli
    echo O descarga desde: https://railway.app/cli
    echo.
    pause
    exit /b 1
)

:: Verificar que estamos en el directorio correcto
if not exist "NavegaStudio.csproj" (
    echo [ERROR] No se encuentra NavegaStudio.csproj
    echo Asegurate de ejecutar este script desde la raiz del proyecto.
    echo.
    pause
    exit /b 1
)

:: Verificar que existe el Dockerfile
if not exist "Dockerfile" (
    echo [ERROR] No se encuentra Dockerfile
    echo El Dockerfile es necesario para el build en Railway.
    echo.
    pause
    exit /b 1
)

:: Mostrar estado actual
echo [1/3] Verificando proyecto...
echo   - Directorio: %cd%
echo   - Proyecto: NavegaStudio.csproj
echo   - Dockerfile: OK
echo.

:: Build local opcional para verificar errores antes de subir
echo [2/3] Verificando build local...
dotnet build --nologo -v q 2>nul
if %errorlevel% neq 0 (
    echo.
    echo [ERROR] El build local fallo. Corrige los errores antes de desplegar.
    echo Ejecuta 'dotnet build' para ver los detalles.
    echo.
    pause
    exit /b 1
)
echo   - Build local: OK
echo.

:: Desplegar a Railway
echo [3/3] Desplegando a Railway...
echo   - Servicio: navegastudio
echo   - Esto puede tardar ~1-2 minutos...
echo.
railway up --service navegastudio
if %errorlevel% neq 0 (
    echo.
    echo [ERROR] El despliegue fallo.
    echo.
    echo Posibles causas:
    echo   - No estas autenticado: ejecuta 'railway login --browserless'
    echo   - No estas vinculado al proyecto: ejecuta 'railway link'
    echo   - Problema de red: verifica tu conexion a internet
    echo.
    pause
    exit /b 1
)

echo.
echo ============================================
echo   Despliegue completado con exito!
echo ============================================
echo.
echo   URL: https://navegastudio-production.up.railway.app
echo.
echo   Para ver logs: railway logs --service navegastudio
echo.
pause
