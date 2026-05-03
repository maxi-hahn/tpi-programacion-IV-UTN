# Diagrama de Clases - Sistema de Gestión de Gimnasio

```mermaid
classDiagram

    class Plan {
        +int id_plan
        +String nombre
        +float valor
        +int clases_max
    }

    class Usuario {
        +int id_usuario
        +String nombre
        +String email
        +String tipo_usuario
        +String estado
        +String password
        +String dni
        +int id_plan FK
    }

    class Clase {
        +int id_clase
        +String nombre
        +int cupo_maximo
        +int id_sede FK
    }

    class HorariosClase {
        +int id
        +String dia
        +Time hora_inicio
        +Time hora_fin
        +int id_clase FK
    }

    class Inscripcion {
        +int id_inscripcion
        +int id_cliente FK
        +int id_clase FK
    }

    class Sede {
        +int id_sede
        +String nombre
        +String direccion
    }

    Plan "1" --> "N" Usuario : tiene
    Usuario "N" --> "M" Clase : inscripto en
    Inscripcion ..> Usuario : referencia
    Inscripcion ..> Clase : referencia
    Clase "N" --> "1" Sede : pertenece a
    Clase "1" --> "N" HorariosClase : tiene
```
