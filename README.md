# Task Management System - Prueba Técnica Fullstack - Nivel Semisenior

---

## 🌐 Live Demo
La aplicación se encuentra desplegada y operativa en la siguiente dirección:
👉 **[http://100.30.192.189/](http://100.30.192.189/)**

*Desplegado en **AWS EC2** utilizando **Docker** y **NGINX**.*

---

Solución completa y profesional desarrollada con **.NET 8 Web API**, **Angular 18** y **SQL Server**.

## Arquitectura y Tecnologías
- **Backend**: Clean Architecture (Domain Driven Design). C# .NET 8, Entity Framework Core 8, Result Pattern, Unit of Work, Middleware global de errores.
- **Frontend**: Angular 18 (Standalone Components, Signals, RxJS). Estructura por Feature Modules con Lazy Loading.
- **Base de Datos**: SQL Server. Constraint `ISJSON` y operaciones avanzadas con Entity Framework Core (`FromSqlRaw`, LINQ).


## Pasos para Ejecutar el Proyecto

### 0. Clonación del Proyecto
Primero, clona el repositorio en tu máquina local:
```bash
git clone https://github.com/gioh2020/TaskGioh.git
cd TaskGioh
```

### 1. Base de Datos (SQL Server)
Ejecuta los scripts que están en la carpeta `Database/` en el siguiente orden estricto:
1. **`init-db.sql`**: **(PASO PRIMORDIAL)** Crea la base de datos `TaskManagementDB` y prepara el entorno.
2. **`task-management-DB/TaskManagementDB.sql`**: Crea todas las tablas, índices, constraints JSON y las vistas de la aplicación.
3. **`SeedData.sql`**: Inserta los usuarios y tareas iniciales para pruebas.

*(Nota: Asegúrate de que la cadena de conexión en `TaskManagement.API/appsettings.json` apunte a tu servidor local).*

### 2. Backend (.NET 8)
Abre una terminal en `TaskManagement.API`:
```bash
dotnet run
```
El servidor levantará en `http://localhost:5000`.
Accede a **Swagger** para ver y probar la documentación de la API:
`http://localhost:5000/swagger`

### 3. Frontend (Angular 18)
Abre una terminal en `task-management-frontend`:
```bash
npm install
npm run start
```
La aplicación web se ejecutará en `http://localhost:4200`.

## Decisiones Técnicas Destacadas

### Backend
1. **Result Pattern**: Los servicios de negocio retornan un objeto `Result<T>` que encapsula el éxito/error, evitando el uso costoso de excepciones para lógica de negocio.
2. **Entity Framework Core**: Consultas de solo lectura optimizadas con `.AsNoTracking()`.
3. **Manejo de JSON**: Uso de columnas `NVARCHAR(MAX)` con validación `ISJSON()` y manipulación de campos específicos mediante `JSON_MODIFY`.

### Frontend
1. **Angular Signals + RxJS**: Gestión de estado reactiva para una detección de cambios ultra-eficiente.
2. **Stand-alone Components y Lazy Loading**: Arquitectura moderna sin `NgModules`, facilitando el Lazy Loading.
3. **Control de Errores Global**: Interceptor centralizado para procesar respuestas de error del servidor de forma consistente.
