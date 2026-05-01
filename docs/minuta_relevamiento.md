# Minuta de Relevamiento
## Sistema de Gestión de Gimnasio

---

**Fecha:** Mayo 2026  
**Tipo de documento:** Minuta de Relevamiento  
**Proyecto:** TPI - Aplicación de Gestión de Gimnasio

---

## 1. Descripción de la Temática

La aplicación tiene como objetivo gestionar las operaciones de un gimnasio con múltiples sedes. Permite administrar los usuarios (clientes y administradores), los planes de membresía, las clases ofrecidas, sus horarios y la inscripción de clientes a dichas clases.

El sistema apunta a digitalizar y centralizar la administración del gimnasio, reemplazando el registro manual y facilitando tanto la gestión interna como la experiencia del cliente.

---

## 2. Actores del Sistema

- **Administrador:** gestiona sedes, clases, horarios, usuarios y planes.
- **Cliente (Usuario):** se registra, elige un plan, se inscribe en clases y consulta su actividad.

---

## 3. Funcionalidades Principales

### 3.1 Gestión de Usuarios
- Registro de nuevos usuarios con datos personales (nombre, email, DNI, contraseña).
- Asignación de tipo de usuario: cliente o administrador.
- Activación/desactivación del estado de un usuario.
- Asociación de cada usuario a un plan de membresía.

### 3.2 Gestión de Planes
- Creación y edición de planes (nombre, precio, cantidad máxima de clases permitidas).
- Visualización de planes disponibles.
- Asignación de un plan a un usuario.

### 3.3 Gestión de Sedes
- Alta, baja y modificación de sedes.
- Cada sede posee nombre y dirección.
- Las clases están asociadas a una sede específica.

### 3.4 Gestión de Clases
- Creación de clases con nombre, cupo máximo y sede asignada.
- Definición de horarios por clase (día, hora de inicio y hora de fin).
- Visualización de clases disponibles por sede y horario.

### 3.5 Inscripción a Clases
- El cliente puede inscribirse a una o más clases disponibles según su plan.
- El sistema valida que no se supere el cupo máximo de la clase.
- El sistema valida que el cliente no exceda el límite de clases de su plan.
- Registro de cada inscripción en una tabla auxiliar (Inscripcion) que vincula cliente y clase.

### 3.6 Consultas y Reportes (opcionales / deseables)
- Listado de clases con sus inscriptos.
- Historial de clases de un cliente.
- Ocupación de cada clase en relación al cupo máximo.

---

## 4. Restricciones y Reglas de Negocio

- Un usuario solo puede tener un plan activo a la vez.
- Un cliente no puede inscribirse a más clases de las que permite su plan.
- Cada clase tiene un cupo máximo que no puede superarse.
- Las clases pertenecen a una sola sede.
- Los horarios están vinculados directamente a una clase.

---

## 5. Entidades Identificadas

| Entidad        | Descripción |
|----------------|-------------|
| `Plan`         | Define los tipos de membresía disponibles en el gimnasio. |
| `Usuario`      | Representa a los clientes y administradores del sistema. |
| `Clase`        | Actividades físicas ofrecidas por el gimnasio. |
| `HorariosClase`| Días y horarios en que se dicta cada clase. |
| `Sede`         | Ubicaciones físicas del gimnasio. |
| `Inscripcion`  | Tabla auxiliar que registra la relación N:M entre usuarios y clases. |

---

## 6. Relaciones entre Entidades

- Un **Plan** puede tener muchos **Usuarios** (1:N).
- Un **Usuario** puede inscribirse a muchas **Clases**, y una **Clase** puede tener muchos **Usuarios** (N:M → resuelta con la entidad `Inscripcion`).
- Una **Clase** pertenece a una **Sede** (N:1).
- Una **Clase** puede tener muchos **HorariosClase** (1:N).

---

## 7. Tecnologías Previstas

- **Backend:** A definir por el grupo (ej. Node.js, Spring Boot, etc.)
- **Frontend:** A definir por el grupo (ej. React, Angular, etc.)
- **Base de datos:** Relacional (ej. MySQL, PostgreSQL)
- **Control de versiones:** Git + GitHub/GitLab

---

*Documento elaborado como parte del Trabajo Práctico Integrador (TPI).*
