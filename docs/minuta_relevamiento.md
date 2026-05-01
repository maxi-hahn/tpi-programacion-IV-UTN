Minuta de relevamiento - Proyecto TPI

1. Tema

Aplicación de gestión de clases (gimnasio/centro deportivo) para administrar usuarios, planes, sedes, clases y horarios. Permite a los clientes inscribirse en clases, a los administradores gestionar la oferta y ver reportes básicos.

2. Objetivo

Desarrollar una aplicación web que permita:
- Gestionar usuarios (clientes, administradores, sysadmin).
- Definir planes con límites de clases y asignarlos a usuarios.
- Administrar sedes y aulas donde se dictan las clases.
- Crear y programar clases con cupo y horarios.
- Permitir inscripciones de usuarios a clases (control de cupo).

3. Funcionalidades principales

- Autenticación y autorización de usuarios por tipo (cliente, admin, sysadmin).
- CRUD para Planes, Sedes, Clases y Horarios.
- Gestión de inscripciones: crear y cancelar inscripciones, verificar cupo y reglas del plan.
- Visualización de horarios por sede y por clase.
- Reportes básicos: ocupación de clases, inscripciones por usuario, ingresos por planes.

4. Requisitos no funcionales

- Persistencia en base de datos MySQL.
- API RESTful con .NET 10 (ASP.NET Core).
- Documentación mínima con diagramas y README en la carpeta docs.
- Permisos mínimos: no usar root para la app — crear usuario de BD dedicado.

5. Entidades principales (resumen)

- Usuario: id_usuario, nombre, email, tipo_usuario, estado, password, plan_id, dni.
- Plan: id_plan, nombre, valor, clases_max.
- Sede: id_sede, nombre, direccion.
- Clase: id_clase, nombre, cupo_maximo, id_sede.
- HorariosClase: id, dia, hora_inicio, hora_fin, id_clase.
- Inscripcion: id_inscripcion, cliente_id, clase_id, fecha_inscripcion.

6. Observaciones

- Se recomienda usar un usuario de base de datos específico para la app y no root.
- El diagrama de clases se encuentra en docs/diagrama_clases.mmd en formato Mermaid para fácil visualización.

7. Entrega

- Link al repositorio: (poner link del repositorio en la entrega final).



