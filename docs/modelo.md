# Modelo de Dominio - Billetera Virtual

## Diagrama de clases

```mermaid
classDiagram

class RegistroClientes {
    -List~Cliente~ clientes
    +Cliente RegistrarCliente(string nombre, string dni, string email)
    +Cliente BuscarPorDni(string dni)
    +bool ExisteDni(string dni)
}

class Cliente {
    -int id
    -string nombre
    -string dni
    -string email
    -List~Destinatario~ destinatarios

    +Billetera ObtenerBilletera()
    +void AgregarDestinatario(Cliente cliente, string alias)
    +void EliminarDestinatario(string alias)
    +Destinatario BuscarDestinatario(string alias)
}

class Billetera {
    -decimal saldo
    -HistorialMovimientos historial

    +decimal ConsultarSaldo()
    +void CargarDinero(decimal monto)
    +List~Movimiento~ ObtenerMovimientos()
    +void Transferir(Billetera destino, decimal monto)
    +bool TieneSaldoSuficiente(decimal monto)
    +bool MontoDentroDelLimite(decimal monto)

    ~void Acreditar(decimal monto)
    ~void Debitar(decimal monto)
    ~void RegistrarMovimiento(Movimiento movimiento)
}

class LimiteTransferencia {
    -decimal montoMaximo
    +bool EsValido(decimal monto)
    +void ActualizarLimite(decimal nuevoLimite)
}

class HistorialMovimientos {
    -List~Movimiento~ movimientos
    +void AgregarMovimiento(Movimiento movimiento)
    +List~Movimiento~ ObtenerMovimientos()
    +List~Movimiento~ OrdenarPorFechaDescendente()
}

class Movimiento {
    <<abstract>>
    -int id
    -decimal monto
    -DateTime fecha
    +void Ejecutar()
    #void Validar()*
    #void Aplicar()*
    #void Registrar()*
    +string ObtenerTipo()*
}

class Carga {
    -Billetera billetera
    #void Validar()
    #void Aplicar()
    #void Registrar()
    +string ObtenerTipo()
}

class Transferencia {
    -Billetera origen
    -Billetera destino
    #void Validar()
    #void Aplicar()
    #void Registrar()
    +string ObtenerTipo()
}

class Destinatario {
    -string alias
    -Cliente cliente
    +string ObtenerAlias()
    +string ObtenerNombre()
    +Billetera ObtenerBilletera()
}

RegistroClientes "1" o-- "0..*" Cliente : registra
Cliente "1" *-- "1" Billetera : posee
Cliente "1" *-- "0..*" Destinatario : guarda
Destinatario --> "1" Cliente : referencia
Billetera "1" *-- "1" HistorialMovimientos : contiene
Billetera "1" *-- "1" LimiteTransferencia : utiliza
HistorialMovimientos "1" o-- "0..*" Movimiento : registra
Movimiento <|-- Carga
Movimiento <|-- Transferencia
Carga --> "1" Billetera : acredita en
Transferencia --> "1" Billetera : origen
Transferencia --> "1" Billetera : destino
```

## Justificación del modelo

### Responsabilidades principales

- **RegistroClientes** administra los clientes registrados y evita DNIs duplicados.
- **Cliente** representa a la persona registrada, posee una billetera y administra sus destinatarios.
- **Billetera** administra el saldo y coordina cargas, transferencias e historial.
- **LimiteTransferencia** encapsula la regla de negocio del monto máximo permitido.
- **HistorialMovimientos** registra y consulta las operaciones de una billetera.
- **Movimiento** define el flujo común de las operaciones mediante Template Method.
- **Carga** y **Transferencia** implementan comportamientos distintos de validación, aplicación y registro.
- **Destinatario** representa una referencia guardada a otro cliente registrado.

### Agregación y composición

- **Cliente *-- Billetera:** la billetera pertenece al cliente y se crea junto con su registro.
- **Cliente *-- Destinatario:** los destinatarios guardados pertenecen al cliente que los administra.
- **Billetera *-- HistorialMovimientos:** cada billetera contiene su propio historial.
- **Billetera *-- LimiteTransferencia:** la billetera utiliza su propia regla de límite.
- **HistorialMovimientos o-- Movimiento:** se usa agregación porque una misma transferencia puede quedar referenciada en los historiales de origen y destino.
- **RegistroClientes o-- Cliente:** el registro administra una colección de clientes, pero el cliente es una entidad del dominio con identidad propia.

### Template Method

La clase abstracta `Movimiento` define el flujo general:

```text
Ejecutar()
  → Validar()
  → Aplicar()
  → Registrar()
```

`Carga` y `Transferencia` redefinen esos pasos porque su comportamiento es diferente.

## Diagrama de secuencia - Realizar transferencia

```mermaid
sequenceDiagram
    actor Titular
    participant Origen as origen:Billetera
    participant Transf as transferencia:Transferencia
    participant Limite as limite:LimiteTransferencia
    participant Destino as destino:Billetera
    participant HOrigen as historialOrigen:HistorialMovimientos
    participant HDestino as historialDestino:HistorialMovimientos

    Titular->>Origen: Transferir(destino, monto)
    Origen->>Transf: Crear Transferencia(origen, destino, monto)
    Origen->>Transf: Ejecutar()

    Transf->>Transf: Validar()
    activate Transf
    Note right of Transf: Dentro de Validar(): monto > 0, origen != destino, limite permitido y saldo suficiente

    Transf->>Origen: MontoDentroDelLimite(monto)
    Origen->>Limite: EsValido(monto)
    Limite-->>Origen: limiteValido
    Origen-->>Transf: limiteValido

    Transf->>Origen: TieneSaldoSuficiente(monto)
    Origen-->>Transf: saldoSuficiente
    deactivate Transf

    alt Validaciones correctas
        Transf->>Transf: Aplicar()
        Transf->>Origen: Debitar(monto)
        Transf->>Destino: Acreditar(monto)

        Transf->>Transf: Registrar()
        Transf->>Origen: RegistrarMovimiento(transferencia)
        Origen->>HOrigen: AgregarMovimiento(transferencia)

        Transf->>Destino: RegistrarMovimiento(transferencia)
        Destino->>HDestino: AgregarMovimiento(transferencia)

        Transf-->>Origen: transferencia completada
        Origen-->>Titular: transferencia exitosa

    else Monto invalido o destino igual al origen
        Origen-->>Titular: transferencia rechazada

    else Supera el limite permitido
        Origen-->>Titular: transferencia rechazada

    else Saldo insuficiente
        Origen-->>Titular: transferencia rechazada
    end
```

## Decisiones de diseño del flujo

- El diagrama comienza cuando la billetera destino ya fue seleccionada.
- La validación se concentra en `Transferencia.Validar()`.
- Antes de modificar los saldos se verifica el monto, que origen y destino sean distintos, el límite permitido y el saldo disponible.
- `Transferencia.Aplicar()` realiza el débito en origen y la acreditación en destino.
- `Transferencia.Registrar()` registra la misma transferencia en ambos historiales a través de las billeteras.
- El acceso al historial queda encapsulado mediante `Billetera.RegistrarMovimiento()`.
