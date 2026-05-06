# Task Management System - Prueba Técnica Fullstack - Nivel Semisenior

---

## 🌐 Live Demo
La aplicación se encuentra desplegada y operativa en la siguiente dirección:
👉 **[http://100.30.192.189/](http://100.30.192.189/)**

*Desplegado en **AWS EC2** utilizando **Docker** y **NGINX**.

---

Solución completa y profesional desarrollada con **.NET 8 Web API**, **Angular 18** y **SQL Server**.

## Arquitectura y Tecnologías
- **Backend**: Clean Architecture (Domain Driven Design). C# .NET 8, Entity Framework Core 8, Result Pattern, Unit of Work, Middleware global de errores.
- **Frontend**: Angular 18 (Standalone Components, Signals, RxJS). Estructura por Feature Modules con Lazy Loading.
- **Base de Datos**: SQL Server. Constraint `ISJSON` y operaciones avanzadas con Entity Framework Core (`FromSqlRaw`, LINQ).


### Pasos para Ejecutar el Proyecto

### 1. Base de Datos (SQL Server)
Ejecuta los scripts que están en la carpeta `Database/` en el siguiente orden estricto:
1. **`init-db.sql`**: **(PASO PRIMORDIAL)** Crea la base de datos `TaskManagementDB` y prepara el entorno.
2. **`task-management-DB/TaskManagementDB.sql`**: Crea todas las tablas, índices, constraints JSON y las vistas de la aplicación.
3. **`SeedData.sql`**: Inserta los usuarios y tareas iniciales para pruebas.

*(Nota: Asegúrate de que la cadena de conexión en `TaskManagement.API/appsettings.json` apunte a tu servidor local).*

### 2. Backend (.NET 8)
Abre una terminal en la ruta raíz del proyecto o directamente en `TaskManagement.API`:
```bash
cd TaskManagement.API
dotnet run
```
El servidor levantará en `http://localhost:5000`.
Accede a **Swagger** para ver y probar la documentación de la API:
`http://localhost:5000/swagger`

### 3. Frontend (Angular 18)
Abre otra terminal en la carpeta `task-management-frontend`:
```bash
cd task-management-frontend
npm install
npm run start
```
La aplicación web se ejecutará en `http://localhost:4200` (o el puerto configurado en angular.json).

## Decisiones Técnicas Destacadas

### Backend
1. **Result Pattern**: En lugar de lanzar excepciones (lo cual es costoso a nivel de CPU), los servicios de negocio retornan un objeto `Result<T>` que encapsula el éxito/error y el Status Code HTTP adecuado. El controlador solo debe hacer un match.
2. **Entity Framework Core**: Todo el acceso de escritura es por el ORM usando el patrón genérico Repository. Las consultas de solo lectura usan `.AsNoTracking()` para mejorar drásticamente el performance ya que no se rastrean los cambios en memoria.
3. **Reglas de Negocio en el Dominio**: La lógica "No se permite pasar de Pending a Done" vive dentro de la propia entidad `Task`, asegurando el principio de encapsulamiento del DDD.

### Frontend
1. **Angular Signals + RxJS**: Toda la conexión HTTP hacia la API es Pura-Reactiva (RxJS con observables). Una vez los datos llegan, se bajan a Signals (`set()`, `update()`) en el Servicio (que actúa como Store) para que Angular detecte cambios en UI con precisión quirúrgica, sin requerir Zone.js en esos nodos.
2. **Stand-alone Components y Lazy Loading**: No existe `app.module.ts`. Toda la app está construida sobre módulos funcionales que se cargan diferidos, optimizando el bundle size.
3. **Control de Errores Global**: A través de `error.interceptor.ts`, capturamos los códigos HTTP (400, 404, 409, 422) lanzados por el Result Pattern en el Backend y los procesamos en la capa de UI de forma amigable.
