# UNIVERSIDAD DON BOSCO
## CICLO 02/2026

# PROYECTO CÁTEDRA FASE II
## "MAQUILLAJE SV"
### Sistema web de inventario y tienda en línea con asesoría de Inteligencia Artificial

---

## Integrantes del equipo

| Integrante | Carnet | Responsabilidad principal |
| :--- | :---: | :--- |
| Manuel Ezequiel Guerrero Granados | GG241501 | Backend y Base de Datos |
| Jorge Alberto Orellana Henríquez | OH240812 | Docker, Despliegue Azure y Repositorio |
| Adriana Eunice Flores Hernández | FH190090 | Seguridad, Pruebas, Documentación y Video |
| Jonathan Enmanuel Galeas Galeas | GG240141 | Frontend y UX/UI |
| Diego Alejandro Cortez Durán | CD231127 | Inteligencia Artificial e Integración |

**Docente:** Ing. René Mauricio Tejada Tobar

---

##  Descripción del proyecto

Las pequeñas empresas de cosméticos enfrentan dos problemas: los clientes en línea abandonan la compra porque no saben qué productos se adaptan a su tipo de piel y a la ocasión, y la vendedora no tiene control en tiempo real del inventario, lo que provoca quiebres de stock.

**Maquillaje sv** resuelve ambos con un solo sistema:

* **Portal del cliente:** catálogo con filtros y búsqueda, carrito de compras, checkout con comprobante digital e historial de pedidos.
* **Asesora Virtual con IA (BeautyAdvisor):** recomienda productos en stock según la consulta, el tipo de piel, la ocasión y el presupuesto.
* **Portal de la vendedora:** dashboard, punto de venta (POS), CRUD de productos y categorías, kárdex de movimientos, reabastecimiento y diagnóstico inteligente de inventario.

---

##  Tecnologías

* ASP.NET Core MVC (.NET 10) y C#
* Entity Framework Core con Microsoft SQL Server 2022 / LocalDB
* Bootstrap 5, Razor y JavaScript
* Docker y Docker Compose
* Arquitectura en capas (Controllers → Services → Repositories → Base de datos)

---

##  Inteligencia Artificial

La IA es un **motor propio de recomendación basado en reglas y puntuación ponderada** (implementado en `Services/IAService.cs`). No depende de servicios externos.

| Función | Entradas | Resultado |
| :--- | :--- | :--- |
| **Asesora Virtual (clientes)** | Consulta en texto, tipo de piel, ocasión, presupuesto y catálogo con stock | Análisis, consejo cosmético y hasta 4 productos recomendados con su motivo |
| **Diagnóstico de inventario (vendedora)** | Productos y ventas registradas | Alertas de stock crítico (≤ 5 unidades), órdenes de reposición sugeridas y productos más vendidos |

Como evolución se propone Azure OpenAI (ver [`docs/Propuesta_Despliegue_Azure.md`](docs/Propuesta_Despliegue_Azure.md)).

---

##  Puesta en marcha rápida

> Guía completa: https://docs.google.com/document/d/1dBVuKxUu3JkkNVuYos52cGMfcRIEhdEy/edit?usp=drive_link&ouid=117926382059529835393&rtpof=true&sd=true

### Opción 1: Ejecución local (.NET SDK 10)

```bash
git clone https://github.com/Adri0205/makeup_inventory.git
cd makeup_inventory/src/MaquillajeInventory
dotnet restore
dotnet build
dotnet run
```

Abra la URL que muestre la consola. La base de datos y los datos de ejemplo se crean en el primer arranque.

### Opción 2: Docker Compose

Los archivos de Docker están en la carpeta `docker/`. Ejecute los comandos desde ahí:

```bash
cd makeup_inventory/docker
cp .env.example .env        # en Windows: copy .env.example .env
# Edite .env y cambie SA_PASSWORD por una contraseña segura
docker compose up -d --build
```

Aplicación disponible en `http://localhost:8080`.

Para detener: `docker compose down` (conserva los datos) o `docker compose down -v` (los elimina).

---

##  Estructura del repositorio

```
makeup_inventory/
├── .gitignore
├── README.md
├── database/
│   └── MakeupInventoryDB.sql        # Script SQL de la base de datos
├── docker/
│   ├── .dockerignore
│   ├── .env.example                 # Variables de entorno (copiar a .env)
│   ├── Dockerfile
│   └── docker-compose.yml
├── docs/
│   ├── Alcance_Entrega.md
│   ├── Evidencias_Pruebas.md
│   ├── Manual_Instalacion.md
│   ├── Manual_Tecnico.md
│   ├── Manual_Usuario.md
│   ├── Propuesta_Despliegue_Azure.md
│   └── img/                         # Capturas de manuales y pruebas
└── src/
    └── MaquillajeInventory/         # Código fuente ASP.NET Core
```

---

##  Estado de los entregables (Fase II)

| N° | Entregable | Estado | Dónde encontrarlo |
| :-: | :--- | :--- | :--- |
| 1 | Aplicación web funcional | Completado | `src/MaquillajeInventory` |
| 2 | Backend en ASP.NET Core | Completado | `Controllers`, `Services`, `Repositories`, `Data` |
| 3 | Integración de IA | Completado (motor basado en reglas) | `Services/IAService.cs` |
| 4 | Docker y propuesta de nube | Completado | `docker/`, `docs/Propuesta_Despliegue_Azure.md` |
| 5 | Base de datos | Completado | `database/MakeupInventoryDB.sql` |
| 6 | Pruebas de funcionamiento | Completado: 25 casos
| 7 | Documentación | Completado | https://docs.google.com/document/d/1N-EdFZNFU6brF89RCi6RqmgIpK-jpugL/edit?usp=sharing&ouid=117926382059529835393&rtpof=true&sd=true · https://docs.google.com/document/d/1dBVuKxUu3JkkNVuYos52cGMfcRIEhdEy/edit?usp=sharing&ouid=117926382059529835393&rtpof=true&sd=true · https://docs.google.com/document/d/1iECDJ79-IVO8ZxZGJ9YQTH-GR7CnKHCo/edit?usp=drive_link&ouid=117926382059529835393&rtpof=true&sd=true · https://docs.google.com/document/d/11pyYItozwNpEqmdJX90ZMDOl7GclZXAy/edit?usp=drive_link&ouid=117926382059529835393&rtpof=true&sd=true |
| 8 | Video de evidencia | Completado | https://drive.google.com/file/d/128xuCMkgB5GcA1dI3z2GO4bUok4JxWDl/view?usp=sharing |
| 9 | Repositorio | Completado | Este repositorio |
