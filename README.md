# Artemis Banking Pro (ABP)

## Descripción

**Artemis Banking Pro (ABP)** es una plataforma de banca digital desarrollada como proyecto académico para el Instituto Tecnológico de las Américas (ITLA).

El proyecto tiene como propósito implementar una solución que permita gestionar de manera integral las principales operaciones y productos financieros de una entidad bancaria. La plataforma contempla la administración de usuarios, cuentas de ahorro, préstamos y tarjetas de crédito, además de operaciones como depósitos, retiros, pagos, transferencias, gestión de beneficiarios y avances de efectivo.

La solución está compuesta por una aplicación web desarrollada con **ASP.NET Core MVC sobre .NET 9** y una **Web API** destinada a exponer funcionalidades administrativas y de comercio, incluyendo el procesamiento de pagos mediante el módulo **Hermes Pay**.

---

## Integrantes

| Nombre | Matrícula |
|:---|:---:|
| **Aliandy Jiménez** | **2025-1018** |
| **Waldin Ceballos** | **2025-1112** |
| **Yailyn Santana** | **2025-1104** |
| **Sky Andujar** | **2025-1063** |

---

## Objetivo

Desarrollar una plataforma de banca digital que permita gestionar de forma organizada y segura las principales operaciones financieras de una entidad bancaria, aplicando principios de arquitectura de software, separación de responsabilidades, autenticación, autorización y buenas prácticas de desarrollo.

El sistema busca garantizar que cada usuario pueda acceder únicamente a las funcionalidades correspondientes a su rol, preservar la integridad de balances, deudas y transacciones, y mantener la trazabilidad de las operaciones financieras realizadas.

---

## Roles del sistema

La plataforma contempla cuatro roles principales:

| Rol | Descripción |
|:---|:---|
| **Administrador** | Responsable de la administración de usuarios y productos financieros, así como de la consulta de indicadores generales del sistema. |
| **Cajero** | Responsable de ejecutar operaciones financieras como depósitos, retiros y pagos. |
| **Cliente** | Usuario que consulta sus productos financieros y realiza las operaciones disponibles para su cuenta. |
| **Comercio** | Rol utilizado exclusivamente para las funcionalidades de la Web API y el procesamiento de pagos mediante Hermes Pay. |

El acceso a las funcionalidades se encuentra restringido de acuerdo con el rol del usuario autenticado.

---

## Funcionalidades principales

### Administrador

El módulo administrativo contempla funcionalidades para la gestión y supervisión de la plataforma:

- Dashboard con indicadores generales.
- Gestión de usuarios.
- Gestión de préstamos.
- Gestión de tarjetas de crédito.
- Gestión de cuentas de ahorro.
- Asignación de productos financieros.
- Consulta de información general del sistema.

### Cliente

El cliente puede consultar sus productos financieros y realizar operaciones relacionadas con sus cuentas:

- Consulta de cuentas de ahorro activas.
- Consulta de préstamos activos.
- Consulta de tarjetas de crédito activas.
- Consulta de movimientos.
- Gestión de beneficiarios.
- Transferencias entre cuentas propias.
- Transferencias a beneficiarios.
- Pagos de tarjetas de crédito.
- Pagos de préstamos.
- Avances de efectivo.
- Transacciones express.

### Cajero

El módulo de cajero permite realizar operaciones financieras de carácter operativo:

- Depósitos.
- Retiros.
- Pagos a tarjetas de crédito.
- Pagos a préstamos.
- Transacciones a cuentas de terceros.

### Comercio y Hermes Pay

El rol Comercio está destinado a las funcionalidades de la Web API y al procesamiento de pagos mediante **Hermes Pay**.

La API contempla módulos relacionados con:

- Gestión de usuarios.
- Gestión de préstamos.
- Gestión de tarjetas de crédito.
- Gestión de cuentas de ahorro.
- Gestión de comercios.
- Procesamiento de pagos.

---

## Arquitectura

El proyecto implementa **Onion Architecture**, con una separación de responsabilidades entre las diferentes capas de la solución.

La arquitectura contempla:

- Capa de presentación.
- Capa de aplicación.
- Capa de dominio.
- Capa de infraestructura.
- Web API para servicios externos.

Como parte de los requerimientos técnicos se contempla el uso de repositorios y servicios genéricos para la gestión de datos, manteniendo una estructura organizada y reutilizable.

---

## Tecnologías y herramientas

| Tecnología / Herramienta | Propósito |
|:---|:---|
| **C#** | Lenguaje principal de desarrollo |
| **.NET 9** | Plataforma de desarrollo |
| **ASP.NET Core MVC** | Desarrollo de la aplicación web |
| **ASP.NET Core Web API** | Exposición de servicios externos |
| **Entity Framework Core** | Persistencia y acceso a datos |
| **ASP.NET Identity** | Gestión de usuarios, roles y autenticación |
| **JWT** | Autenticación de la Web API |
| **AutoMapper** | Mapeo entre ViewModels, Entities y DTOs |
| **CQRS** | Separación de Commands y Queries |
| **Mediator** | Implementación del patrón Mediator |
| **FluentValidation** | Validación de Commands y Queries |
| **Swagger** | Documentación de la Web API |
| **xUnit** | Pruebas automatizadas |
| **Serilog** | Logging y auditoría |
| **Bootstrap** | Diseño de la interfaz |

---

## Seguridad

La aplicación implementa autenticación y autorización basada en roles.

Para la aplicación web se utiliza **ASP.NET Identity** con autenticación basada en sesión/cookies, mientras que la Web API utiliza autenticación mediante **JWT**.

Entre los mecanismos de seguridad contemplados se encuentran:

- Gestión de usuarios y roles.
- Activación de cuentas.
- Restablecimiento de contraseñas.
- Restricción de acceso según el rol.
- Tokens de activación y restablecimiento de un solo uso.
- Protección de información sensible.
- Almacenamiento seguro del CVC mediante hashing.
- Control de usuarios activos e inactivos.

Las funcionalidades internas de la aplicación deben permanecer protegidas incluso cuando un usuario intente acceder directamente mediante una URL.

---

## Persistencia de datos

La persistencia de información se realiza mediante **Entity Framework Core** utilizando el enfoque **Code First**.

El proyecto contempla el uso de:

- Entidades de dominio.
- Repositorios genéricos.
- Servicios genéricos.
- DTOs.
- ViewModels.
- AutoMapper para el mapeo entre las diferentes representaciones de los datos.

---

## Patrones y prácticas de desarrollo

La solución incorpora diferentes patrones y prácticas definidos para el proyecto:

### CQRS

La Web API utiliza el patrón **CQRS** para separar las operaciones de lectura y escritura mediante Commands y Queries.

### Mediator

El patrón **Mediator** permite desacoplar los componentes que intervienen en el procesamiento de Commands y Queries.

### FluentValidation

Las validaciones de Commands y Queries se gestionan mediante **Behaviors** utilizando FluentValidation.

### Manejo centralizado de excepciones

Las excepciones se gestionan mediante un **Global Exception Handler**, utilizando respuestas bajo el estándar **Problem Details (RFC 7807)** tanto en la Web API como en la aplicación web.

---

## Web API

La Web API proporciona funcionalidades administrativas y de comercio para servicios externos.

Entre los módulos definidos se encuentran:

- Login y Account Controller.
- Gestión de usuarios.
- Gestión de préstamos.
- Gestión de tarjetas de crédito.
- Gestión de cuentas de ahorro.
- Gestión de comercios.
- Procesador de pagos Hermes Pay.

La API utiliza JWT para la autenticación y **Swagger** para la documentación de sus endpoints.

---

## Logging y auditoría

El sistema utiliza **Serilog** para registrar información relevante para auditoría, diagnóstico y seguimiento de errores.

Los registros pueden incluir:

- Fecha y hora del evento.
- Nivel del log.
- Usuario autenticado.
- Rol del usuario.
- Endpoint o acción ejecutada.
- Identificador de correlación.
- Resultado de la operación.
- Errores o excepciones.

Las operaciones financieras relevantes deben generar registros, incluyendo:

- Depósitos.
- Retiros.
- Transferencias.
- Pagos a tarjetas.
- Pagos a préstamos.
- Avances de efectivo.
- Procesamiento de pagos mediante Hermes Pay.
- Creación de préstamos.
- Asignación de tarjetas.
- Cancelación de productos financieros.

Por razones de seguridad, los logs no deben almacenar información sensible como contraseñas, tokens completos, CVC, números completos de tarjetas, cadenas de conexión o secretos de configuración.

---

## Pruebas

El proyecto contempla la implementación de pruebas automatizadas mediante **xUnit**.

Se consideran:

- Pruebas unitarias para Commands y Queries.
- Pruebas unitarias para los servicios de lógica de negocio.
- Pruebas de integración para los repositorios.
- Validación de las operaciones de persistencia con la base de datos.

---

## Estructura conceptual

```text
ArtemisBankingPro
│
├── WebApp
│   ├── Controllers
│   ├── Views
│   ├── ViewModels
│   └── wwwroot
│
├── WebAPI
│   ├── Controllers
│   ├── Commands
│   ├── Queries
│   ├── Behaviors
│   └── DTOs
│
├── Application
│   ├── Services
│   ├── DTOs
│   └── Validators
│
├── Domain
│   ├── Entities
│   ├── Enums
│   └── Interfaces
│
└── Infrastructure
    ├── Persistence
    ├── Repositories
    ├── Identity
    └── Logging
```

> Esta estructura es una representación conceptual basada en los requerimientos técnicos del proyecto. La estructura física del repositorio puede variar según la implementación realizada.

---

## Requisitos

Para ejecutar el proyecto se requiere un entorno compatible con las tecnologías utilizadas en la solución, incluyendo:

- .NET 9 SDK.
- Visual Studio 2022 o superior.
- SQL Server.
- Entity Framework Core.

Las versiones específicas de los paquetes y dependencias deben consultarse en los archivos de configuración del proyecto.

---

## Instalación y ejecución

### 1. Clonar el repositorio

```bash
git clone <URL_DEL_REPOSITORIO>
```

### 2. Abrir la solución

Abrir el archivo de solución del proyecto en Visual Studio.

### 3. Configurar la base de datos

Configurar la cadena de conexión correspondiente en los archivos de configuración de la aplicación.

### 4. Aplicar las migraciones

Desde la consola del Administrador de paquetes de Visual Studio:

```powershell
Update-Database
```

También puede utilizarse la CLI de .NET:

```bash
dotnet ef database update
```

### 5. Ejecutar el proyecto

Ejecutar la solución desde Visual Studio o mediante:

```bash
dotnet run
```

---

## Documentación

El desarrollo de Artemis Banking Pro se basa en el documento funcional del proyecto, que define los requerimientos funcionales y técnicos de la plataforma, incluyendo los módulos de administración, cliente, cajero, Web API, comercios y Hermes Pay.

---

## Contexto académico

**Proyecto:** Artemis Banking Pro (ABP)  
**Institución:** Instituto Tecnológico de las Américas (ITLA)  
**Año:** 2026  
**Versión:** 1.0

---

## Equipo de desarrollo

| Nombre | Matrícula |
|:---|:---:|
| Aliandy Jiménez | 2025-1018 |
| Waldin Ceballos | 2025-1112 |
| Yailyn Santana | 2025-1104 |
| Sky Andujar | 2025-1063 |

---

## Licencia

Este proyecto fue desarrollado con fines académicos para el Instituto Tecnológico de las Américas (ITLA).

© 2026 Artemis Banking Pro (ABP)
