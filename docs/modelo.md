# Modelo de Dominio - Billetera Virtual

```mermaid
classDiagram
    class BilleteraVirtual {
        -usuarios : List~Usuario~
        +ObtenerSaldo() : decimal
        +RegistrarUsuario(usuario: Usuario) : void
        +ObtenerUsuario(dni: int) : Usuario
        +ObtenerUsuarios() : List~Usuario~
    }

    class Usuario {
        -dni : int
        -Nombre : string
        -Apellido : string
        -Email : string
        -contraseña : string
    }

    class Cuenta {
        -numeroCuenta : string
        -activa : bool
        -saldo : decimal
        -movimientos : List~movimiento~
        +activar() : void
        +bloquear() : void
        +ConsultarEstado() : string
    }

    class movimiento {
        -monto : decimal
        -fecha : Datetime
        -descripcion : string
        +MostrarMovimiento() : void
    }

    class Estado {
        +pendiente() : bool
        +exitoso() : bool
        +fallido() : bool
    }

    class AbstracBilletera {
        <<abstract>>
        -monto : decimal
        -fecha : Datetime
        -movimiento : List~movimiento~
        +Ejecutar()*
        +Validar()* : bool
    }

    class Transferencias {
        -monto : decimal
        -fecha : Datetime
        -origen : Cuenta
        -destino : Cuenta
        +Ejecutar() : void
        +validar() : bool
    }

    class Deposito {
        -monto : decimal
        -fecha : datetime
        -Cuenta : Cuenta
        +Ejecutar() : void
        +Validar() : bool
    }

    class Retiro {
        -monto : decimal
        -fecha : DateTime
        -Cuenta : Cuenta
        -Estado : bool
        +Ejecutar() : void
        +Validar() : bool
    }

    class GestorTransferencias {
        -Transferencias : list~transferencias~
        +RealizarTransferencia(origen: Cuenta, Destino: Cuenta, monto: decimal) : bool
        +ConsultarTransferencias() : List~Transferencia~
    }

    %% Relaciones
    BilleteraVirtual "1" o-- "*" Usuario : contiene
    Usuario "1" -- "1" Cuenta : posee
    Cuenta "1" *-- "*" movimiento : registra
    movimiento "1" --> "1" Estado : tiene

    AbstracBilletera <|-- Transferencias : hereda
    AbstracBilletera <|-- Deposito : hereda
    AbstracBilletera <|-- Retiro : hereda

    GestorTransferencias "1" o-- "*" Transferencias : gestiona
    Transferencias "1" --> "2" Cuenta : involucra origen/destino
    Deposito "1" --> "1" Cuenta : acredita
    Retiro "1" --> "1" Cuenta : debita
```

---

## Justificación del Modelo

### 1. Agregación vs. Composición
* **Composición (`Cuenta *-- movimiento`):** Los movimientos son parte fundamental del historial de una cuenta. Si la cuenta desaparece, sus movimientos pierden sentido y existencia en el sistema.
* **Agregación (`BilleteraVirtual o-- Usuario`):** Los usuarios existen por sí mismos dentro del dominio del sistema independientemente de la instancia del gestor o la billetera.
* **Agregación (`GestorTransferencias o-- Transferencias`):** El gestor administra la lista de transferencias, pero no destruye el registro histórico si el gestor deja de existir.

### 2. Justificación de la Jerarquía
La abstracción `AbstracBilletera` define la interfaz de ejecución de operaciones transaccionales (`Ejecutar()` y `Validar()`). Cada subclase cambia su **comportamiento dinámico**:
* **`Transferencias`**: Valida saldo en la cuenta origen y acredita en una cuenta destino distinta de forma atómica.
* **`Deposito`**: Valida credenciales e interactúa con medios de ingreso externos (tarjeta/banco) para incrementar el saldo de una sola cuenta.
* **`Retiro`**: Valida saldo disponible y límites de extracción para egresar dinero a un canal externo.
