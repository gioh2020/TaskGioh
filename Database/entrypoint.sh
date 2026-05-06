#!/bin/bash
# ============================================================
# Entrypoint para el contenedor SQL Server
# 1. Inicia SQL Server en segundo plano
# 2. Espera a que estÃ© listo
# 3. Ejecuta el script de inicializaciÃ³n
# ============================================================

# Iniciar SQL Server en background
/opt/mssql/bin/sqlservr &

# Esperar a que SQL Server estÃ© listo para aceptar conexiones
echo "â ³ Esperando a que SQL Server estÃ© listo..."
for i in {1..60}; do
    /opt/mssql-tools/bin/sqlcmd -S localhost -U sa -P "${MSSQL_SA_PASSWORD}" -C -Q "SELECT 1" > /dev/null 2>&1
    if [ $? -eq 0 ]; then
        echo "âœ… SQL Server estÃ¡ listo!"
        break
    fi
    echo "   Intento $i/60 â€” esperando 2 segundos..."
    sleep 2
done

# Ejecutar script de inicializaciÃ³n
echo "ðŸš€ Ejecutando script de inicializaciÃ³n..."
/opt/mssql-tools/bin/sqlcmd -S localhost -U sa -P "${MSSQL_SA_PASSWORD}" -C -d master -i /docker-entrypoint-initdb/init-db.sql

if [ $? -eq 0 ]; then
    echo "âœ… Base de datos inicializada correctamente."
else
    echo "â Œ Error al inicializar la base de datos."
fi

# Mantener SQL Server en primer plano
wait
