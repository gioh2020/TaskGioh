USE TaskManagementDB;
GO

-- Usuarios
INSERT INTO Users (Id, Name, Email, CreatedAt) VALUES
    ('11111111-1111-1111-1111-111111111111', 'Ana Martínez', 'ana.martinez@empresa.com', '2025-01-10 08:00:00'),
    ('22222222-2222-2222-2222-222222222222', 'Carlos López', 'carlos.lopez@empresa.com', '2025-01-11 09:30:00'),
    ('33333333-3333-3333-3333-333333333333', 'María González', 'maria.gonzalez@empresa.com', '2025-01-12 10:00:00');

-- Tareas
INSERT INTO Tasks (Id, Title, Description, Status, AssignedUserId, CreatedAt, AdditionalInfo) VALUES
    ('aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa', 'Diseñar base de datos', 'Modelo relacional', 0, '11111111-1111-1111-1111-111111111111', '2025-01-15 08:00:00', '{"priority":"high","estimatedEndDate":"2025-02-28","tags":["database"]}'),
    ('bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb', 'Implementar API REST', 'Endpoints .NET 8', 1, '22222222-2222-2222-2222-222222222222', '2025-01-16 09:00:00', '{"priority":"medium","estimatedEndDate":"2025-03-15","tags":["backend"]}'),
    ('cccccccc-cccc-cccc-cccc-cccccccccccc', 'Configurar Git', 'Estructura de ramas', 2, '11111111-1111-1111-1111-111111111111', '2025-01-13 07:00:00', '{"priority":"low","estimatedEndDate":"2025-01-14","tags":["git"]}'),
    ('dddddddd-dddd-dddd-dddd-dddddddddddd', 'Desarrollar frontend', 'Componentes Angular', 0, '33333333-3333-3333-3333-333333333333', '2025-01-17 10:00:00', '{"priority":"high","estimatedEndDate":"2025-04-01","tags":["frontend"]}'),
    ('eeeeeeee-eeee-eeee-eeee-eeeeeeeeeeee', 'Pruebas unitarias', 'Tests de dominio', 1, '22222222-2222-2222-2222-222222222222', '2025-01-18 11:00:00', NULL);
GO
