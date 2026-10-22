# UNIVERSIDAD DON BOSCO
## FACULTAD DE ESTUDIOS TECNOLÓGICOS — CICLO 02/2026

---

# PROPUESTA DE DESPLIEGUE EN MICROSOFT AZURE
### PROYECTO CÁTEDRA FASE II: SISTEMA WEB "GLOW & GLAMOUR"

---

## 1. INTRODUCCIÓN Y OBJETIVO DE LA PROPUESTA

En cumplimiento con el **Punto 4** de la guía del Proyecto de Cátedra Fase II, el equipo de desarrollo ha formulado una propuesta arquitectónica y financiera para el despliegue del sistema **Glow & Glamour Makeup Studio** en la plataforma de servicios en la nube **Microsoft Azure**.

El objetivo es migrar la solución desde su entorno de desarrollo y contenerización local hacia una infraestructura PaaS (Platform as a Service) altamente disponible, elástica, segura y de bajo mantenimiento operativo.

---

## 2. SERVICIOS DE MICROSOFT AZURE SELECCIONADOS

Para soportar las cargas de trabajo de la aplicación web, la base de datos relacional y los servicios inteligentes, se han seleccionado los siguientes servicios administrados:

```
+-----------------------------------------------------------------------------------+
|                        MICROSOFT AZURE CLOUD ARCHITECTURE                         |
+-----------------------------------------------------------------------------------+
                                          |
                                    [ Azure DNS ]
                                          |
                                          v
                         [ Azure Front Door / Application Gateway ]
                                          |
                                          v
+-----------------------------------------------------------------------------------+
| AZURE APP SERVICE / AZURE CONTAINER APPS (Linux B1 / P1v3)                        |
| - Imagen Docker: Contenedor ASP.NET Core .NET 10                                  |
| - Escalamiento horizontal automático (Autoscaling)                                |
| - Certificados SSL/TLS gestionados gratuitos                                      |
+-----------------------------------------------------------------------------------+
                     |                                           |
                     v                                           v
+-----------------------------------------+   +-------------------------------------+
| AZURE SQL DATABASE (DTU Basic / S0)     |   | AZURE OPENAI SERVICE / COGNITIVE    |
| - Base de datos 'MakeupInventoryDB'     |   | - Modelo GPT-4o-mini / Embeddings   |
| - Copias de seguridad automáticas (PITR)|   | - Soporte para BeautyAdvisor AI     |
| - Encriptación TDE en reposo y tránsito |   | - Inferencia contextual en español  |
+-----------------------------------------+   +-------------------------------------+
                     |
                     v
+-----------------------------------------+
| AZURE KEY VAULT                         |
| - Gestión segura de ConnectionStrings   |
| - Almacenamiento de API Keys y Secretos |
+-----------------------------------------+
```

### 2.1 Justificación de los Servicios Seleccionados

1. **Azure App Service (Plan Linux para Contenedores):**
   * Permite desplegar directamente la imagen Docker construida mediante nuestro `Dockerfile` sin necesidad de administrar máquinas virtuales ni parches de sistema operativo.
   * Soporte integrado de Continuous Deployment (CI/CD) vinculado a GitHub Actions.
   * Certificados SSL/TLS automáticos y gratuitos mediante Azure Managed Certificates.

2. **Azure SQL Database (PaaS):**
   * Motor de base de datos relacional compatible al 100% con Microsoft SQL Server y Entity Framework Core.
   * Ofrece respaldo automático continuo con restauración a un punto en el tiempo (PITR) hasta por 35 días.
   * Cifrado transparente de datos (TDE) y reglas de firewall a nivel de red virtual (VNet).

3. **Azure OpenAI Service / Cognitive Services:**
   * Hospedaje seguro de modelos LLM (como GPT-4o-mini) con aislamiento empresarial y cumplimiento de privacidad de datos.
   * Enrutamiento mediante endpoint privado e integración con nuestro `IAService`.

4. **Azure Key Vault:**
   * Almacenamiento centralizado y seguro de la cadena de conexión a la base de datos y llaves de acceso, eliminando credenciales expuestas en archivos de texto.

---

## 3. BENEFICIOS TÉCNICOS Y OPERATIVOS

| Área | Beneficio Aportado | Impacto en el Negocio |
| :--- | :--- | :--- |
| **Alta Disponibilidad** | SLA garantizado del 99.95% por Microsoft en App Service y SQL Database. | Operación ininterrumpida de la tienda 24/7 sin caídas durante picos de venta. |
| **Escalabilidad Elástica** | Reglas de escalado automático basadas en consumo de CPU o tráfico HTTP. | Respuesta ágil ante campañas de temporada (ej. Día de las Madres, Black Friday). |
| **Seguridad y Cumplimiento** | Cifrado en reposo y en tránsito (TLS 1.3), Azure DDoS Protection y autenticación por Azure AD. | Protección absoluta de la información de clientes, ventas y kárdex. |
| **Mantenimiento Cero** | Parcheo de seguridad automático del sistema operativo y del motor SQL. | El equipo se enfoca en el desarrollo de funcionalidades de negocio y no en infraestructura. |

---

## 4. ESTIMACIÓN DE COSTOS APROXIMADOS (PRESUPUESTO MENSUAL)

Basado en la calculadora oficial **Azure Pricing Calculator** (Región: *East US 2* / *Central US*), se presenta una estimación orientada a una PyME en fase de crecimiento:

### Opción 1: Nivel de Entrada / Prototipo Universitario Optimizado
| Recurso | Nivel / Capacidad (Tier) | Costo Mensual Aprox. (USD) |
| :--- | :--- | :---: |
| **Azure App Service** | Plan B1 (1 Core, 1.75 GB RAM, Linux) | \$13.00 |
| **Azure SQL Database** | DTU Based - Basic (5 DTU, 2 GB Almacenamiento) | \$4.99 |
| **Azure Key Vault** | Operaciones estándar (< 10,000 transacciones) | \$0.03 |
| **Azure OpenAI Service** | Nivel de consumo (por token utilizado) | \$5.00 |
| **Ancho de Banda / Egress** | Primeros 5 GB gratuitos | \$0.00 |
| **TOTAL MENSUAL ESTIMADO (Prototipo)** | | **\$23.02 USD** |

### Opción 2: Nivel Producción Comercial (Escalable)
| Recurso | Nivel / Capacidad (Tier) | Costo Mensual Aprox. (USD) |
| :--- | :--- | :---: |
| **Azure App Service** | Plan Standard S1 (1 Core, 1.75 GB RAM, Autoscale) | \$73.00 |
| **Azure SQL Database** | DTU Standard S0 (10 DTU, 250 GB) | \$14.72 |
| **Azure OpenAI / AI Foundry** | GPT-4o-mini con inferencia de alto rendimiento | \$15.00 |
| **Azure Key Vault & DNS** | Nivel estándar | \$1.50 |
| **TOTAL MENSUAL ESTIMADO (Producción)** | | **\$104.22 USD** |

---

## 5. ESTRATEGIA DE DESPLIEGUE CONTINUO (CI/CD)

1. **Control de Versiones:** Repositorio en GitHub sincronizado con la rama `main`.
2. **GitHub Actions Workflow:**
   * Disparo automático ante cada `push` a `main`.
   * Ejecución de pruebas unitarias y compilación con `dotnet test` y `dotnet build`.
   * Construcción de la imagen Docker y publicación en **Azure Container Registry (ACR)**.
   * Actualización del webhook en **Azure App Service** para desplegar el contenedor sin tiempo de inactividad (Zero Downtime Deployment).
3. **Migración de Datos:** Ejecución de migraciones automáticas al iniciar el contenedor mediante el método `DbInitializer.Initialize(serviceProvider)`.
