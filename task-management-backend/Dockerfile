# ============================================================
# Dockerfile – TaskManagement API  (.NET 8)
# Multi-stage build: restore → build → publish → runtime
# ============================================================

# ---------- Stage 1: build (SDK para restore + publish) ----------
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# 1. Copiar solo los .csproj para cachear el restore de NuGet
COPY TaskManagement.Domain/TaskManagement.Domain.csproj             TaskManagement.Domain/
COPY TaskManagement.Application/TaskManagement.Application.csproj   TaskManagement.Application/
COPY TaskManagement.Infrastructure/TaskManagement.Infrastructure.csproj TaskManagement.Infrastructure/
COPY TaskManagement.API/TaskManagement.API.csproj                   TaskManagement.API/

# Restore desde el proyecto raíz (API) que arrastra las dependencias
RUN dotnet restore TaskManagement.API/TaskManagement.API.csproj

# 2. Copiar todo el código fuente y publicar en Release
COPY . .
RUN dotnet publish TaskManagement.API/TaskManagement.API.csproj \
    -c Release \
    -o /app/publish

# ---------- Stage 2: runtime (imagen liviana) ----------
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app

# Crear usuario no-root para mayor seguridad
RUN adduser --disabled-password --gecos "" appuser

COPY --from=build /app/publish .

# Entorno Development para habilitar Swagger en local
ENV ASPNETCORE_ENVIRONMENT=Development
# Puerto por defecto de ASP.NET Core
ENV ASPNETCORE_URLS=http://+:8080
EXPOSE 8080

# Cambiar al usuario no-root
USER appuser

ENTRYPOINT ["dotnet", "TaskManagement.API.dll"]
