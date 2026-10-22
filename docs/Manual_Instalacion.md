# UNIVERSIDAD DON BOSCO
## FACULTAD DE ESTUDIOS TECNOLÓGICOS — CICLO 02/2026

---

# MANUAL DE INSTALACIÓN Y CONFIGURACIÓN
### PROYECTO CÁTEDRA FASE II: SISTEMA WEB DE INVENTARIO Y TIENDA INTELIGENTE "GLOW & GLAMOUR"

---

## 1. INTRODUCCIÓN Y REQUISITOS PREVIOS DEL SISTEMA

El presente manual describe paso a paso el procedimiento formal para la clonación, configuración de dependencias, preparación de la base de datos y ejecución del sistema tanto en entorno de desarrollo local como mediante contenedores **Docker**.

### 1.1 Requisitos de Hardware Recomendados
* **Procesador:** x64 / ARM64 multinúcleo a 2.0 GHz o superior.
* **Memoria RAM:** Mínimo 8 GB (Recomendado 16 GB si se ejecuta Docker Desktop con WSL 2).
* **Almacenamiento:** Mínimo 5 GB de espacio libre en disco.

### 1.2 Requisitos de Software
1. **.NET SDK:** Versión 10.0 (o superior).
2. **Motor de Base de Datos:**
   * Microsoft SQL Server 2019 / 2022 o SQL Server LocalDB (`(localdb)\MSSQLLocalDB`).
3. **Herramientas de Contenerización (Opcional para modo Docker):**
   * Docker Desktop con motor WSL 2 activo.
4. **Entorno de Desarrollo Integrado (IDE):**
   * Visual Studio 2022 / 2026 (con carga de trabajo "Desarrollo web y de ASP.NET") o Visual Studio Code con C# Dev Kit.

---

## 2. OBTENCIÓN DEL CÓDIGO FUENTE (CLONACIÓN DEL REPOSITORIO)

Para clonar el repositorio institucional, abra una terminal (PowerShell, Command Prompt o Git Bash) y ejecute:

```bash
git clone https://github.com/usuario-udb/proyecto-catedra-fase-2.git
cd "PROYECTO CATEDRA FASE II"
```

Verifique que la estructura de carpetas cumpla con los estándares requeridos:
* `/src/MaquillajeInventory`: Solución y proyecto ASP.NET Core.
* `/database`: Scripts SQL de creación y datos semilla.
* `/docker`: Dockerfile y docker-compose.yml.
* `/docs`: Manuales técnicos, de usuario, instalación y evidencias.

---

## 3. CONFIGURACIÓN DE BASE DE DATOS LOCAL (MICROSOFT SQL SERVER)

El sistema cuenta con un inicializador automático (`DbInitializer.cs`) que garantiza la creación del esquema y la inserción de datos semilla en el primer arranque. No obstante, si se desea aprovisionar la base de datos de manera manual mediante Transact-SQL, proceda como se indica a continuación:

### Opción A: Ejecución mediante línea de comandos (`sqlcmd`)
```bash
sqlcmd -S "(localdb)\MSSQLLocalDB" -E -i "database\MakeupInventoryDB.sql"
```
*(Nota: Si utiliza una instancia completa de SQL Server, sustituya `(localdb)\MSSQLLocalDB` por `.` o `localhost`).*

### Opción B: Ejecución mediante SQL Server Management Studio (SSMS)
1. Abra **SQL Server Management Studio**.
2. Conéctese a su servidor local (`.` o `(localdb)\MSSQLLocalDB`).
3. Abra el archivo `database/MakeupInventoryDB.sql`.
4. Haga clic en **Execute (F5)**.

---

## 4. CONFIGURACIÓN DE VARIABLES DE ENTORNO Y CADENAS DE CONEXIÓN

En el proyecto ubicado en `src/MaquillajeInventory/appsettings.json`, configure la cadena de conexión correspondiente a su motor local:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=(localdb)\\MSSQLLocalDB;Database=MakeupInventoryDB;Trusted_Connection=True;MultipleActiveResultSets=true;TrustServerCertificate=True"
  },
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  },
  "AllowedHosts": "*"
}
```

---

## 5. INSTALACIÓN DE DEPENDENCIAS Y COMPILACIÓN LOCAL

1. Navegue al directorio de código fuente:
   ```bash
   cd src/MaquillajeInventory
   ```

2. Restaure los paquetes NuGet del proyecto:
   ```bash
   dotnet restore
   ```

3. Compile la solución verificando cero advertencias y cero errores:
   ```bash
   dotnet build
   ```

4. Ejecute la aplicación en modo desarrollo:
   ```bash
   dotnet run
   ```

5. Abra su navegador web en la dirección indicada por la consola (generalmente `http://localhost:5000` o `https://localhost:7000`). La aplicación abrirá de forma automática el **Catálogo Cosmético para el Cliente** y sembrará los registros base si la base de datos está vacía.

---

## 6. EJECUCIÓN MEDIANTE CONTENEDORES DOCKER & DOCKER COMPOSE

El proyecto incluye soporte nativo para despliegue automatizado en entornos multi-contenedor, encapsulando la aplicación web y una instancia dedicada de Microsoft SQL Server 2022 en una red privada virtual.

### 6.1 Preparación de Archivo `.env`
Copie el archivo de ejemplo ubicado en la raíz o en `/docker`:
```bash
copy .env.example .env
```

### 6.2 Construcción y Despliegue con Docker Compose
Desde la raíz del repositorio, ejecute:
```bash
docker compose up -d --build
```

### 6.3 Verificación de Contenedores Activos
Compruebe que ambos servicios (`makeup_sqlserver_db` y `makeup_web_app`) se encuentren en estado `healthy` / `running`:
```bash
docker ps
```

### 6.4 Acceso a la Aplicación Contenerizada
Abra el navegador en:
```
http://localhost:8080
```

### 6.5 Detención del Entorno Docker
Para apagar los servicios preservando el volumen de datos persistente:
```bash
docker compose down
```
*(Si desea reiniciar limpiando la base de datos, use `docker compose down -v`).*

---

## 7. RESOLUCIÓN DE INCIDENCIAS FRECUENTES (TROUBLESHOOTING)

| Síntoma | Posible Causa | Solución |
| :--- | :--- | :--- |
| `Cannot open database "MakeupInventoryDB"` | Instancia SQL Server detenida o LocalDB no iniciado. | Iniciar servicio con `sqllocaldb start MSSQLLocalDB` o `net start MSSQLSERVER`. |
| Error de certificado SSL en conexión SQL | Parámetro `TrustServerCertificate` en `false`. | Asegurar `TrustServerCertificate=True` en la cadena de conexión de `appsettings.json`. |
| Conflicto de puertos en Docker (1433 u 8080 en uso) | Otro servicio local está ocupando el puerto. | Modificar los puertos en `docker-compose.yml` (ej. `14333:1433` y `8085:8080`). |
