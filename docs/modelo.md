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
    -Billetera billetera
    -List~Destinatario~ destinatarios

    +int Id
    +string Nombre
    +string Dni
    +string Email
    +Cliente(int id, string nombre, string dni, string email)
    +Billetera ObtenerBilletera()
    +void AgregarDestinatario(Cliente cliente, string alias)
    +void EliminarDestinatario(string alias)
    +Destinatario BuscarDestinatario(string alias)
}

class Billetera {
    -decimal saldo
    -bool poseeDeudasPendientes
    +string Cvu
    +string Alias

    +Billetera(decimal limiteInicial)
    +void CambiarAlias(string nuevoAlias)
    +decimal ConsultarSaldo()
    +List~Movimiento~ ObtenerMovimientos()
    +bool TieneSaldoSuficiente(decimal monto)
    +bool MontoDentroDelLimite(decimal monto)
    +bool PoseeDeudasPendientes()
    +void EstablecerDeudasPendientes(bool poseeDeudas)
    +void CargarDinero(decimal monto)
    +void Transferir(Billetera destino, decimal monto)
    +void TransferirPorIdentificador(string aliasOCvu, decimal monto, RepositorioBilleteras repositorio)
    +void PagarEnCuotas(decimal montoTotal, int cantidadCuotas)

    ~void Acreditar(decimal monto)
    ~void Debitar(decimal monto)
    ~void RegistrarMovimiento(Movimiento movimiento)
}

class RepositorioBilleteras {
    -List~Billetera~ billeteras
    +void Agregar(Billetera billetera)
    +Billetera BuscarPorIdentificador(string aliasOCvu)
}

class LimiteTransferencia {
    -decimal montoMaximo
    +LimiteTransferencia(decimal montoMaximoInicial)
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
    +int Id
    +decimal Monto
    +DateTime Fecha
    #Movimiento(decimal monto)
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

class PagoEnCuotas {
    -Billetera billetera
    -int cantidadCuotas
    +int CantidadCuotas
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
RepositorioBilleteras "1" o-- "0..*" Billetera : indexa
Billetera ..> RepositorioBilleteras : consulta por alias o CVU
Billetera "1" *-- "1" HistorialMovimientos : contiene
Billetera "1" *-- "1" LimiteTransferencia : utiliza
HistorialMovimientos "1" o-- "0..*" Movimiento : registra
Movimiento <|-- Carga
Movimiento <|-- Transferencia
Movimiento <|-- PagoEnCuotas
Carga --> "1" Billetera : acredita en
Transferencia --> "1" Billetera : origen
Transferencia --> "1" Billetera : destino
PagoEnCuotas --> "1" Billetera : debita de
```

## Justificación del modelo

### Responsabilidades principales

- **RegistroClientes** administra los clientes registrados, asigna su id y evita DNIs duplicados.
- **Cliente** representa a la persona registrada (valida nombre, DNI y formato de email), posee una billetera y administra sus destinatarios.
- **Billetera** administra el saldo, identifica a la cuenta con un **CVU** y un **alias** modificable, y coordina cargas, transferencias, pagos en cuotas e historial. Las operaciones públicas delegan en objetos `Movimiento`.
- **RepositorioBilleteras** permite ubicar una billetera destino a partir de su alias o su CVU, sin necesidad de conocer al cliente.
- **LimiteTransferencia** encapsula la regla de negocio del monto máximo permitido (por defecto 500.000).
- **HistorialMovimientos** registra y consulta las operaciones de una billetera; devuelve copias de la lista para proteger su estado interno.
- **Movimiento** define el flujo común de las operaciones mediante Template Method.
- **Carga**, **Transferencia** y **PagoEnCuotas** implementan comportamientos distintos de validación, aplicación y registro.
- **Destinatario** representa una referencia guardada a otro cliente registrado.

### Agregación y composición

- **Cliente *-- Billetera:** la billetera pertenece al cliente y se crea junto con su registro.
- **Cliente *-- Destinatario:** los destinatarios guardados pertenecen al cliente que los administra.
- **Billetera *-- HistorialMovimientos:** cada billetera contiene su propio historial.
- **Billetera *-- LimiteTransferencia:** la billetera utiliza su propia regla de límite.
- **HistorialMovimientos o-- Movimiento:** se usa agregación porque una misma transferencia puede quedar referenciada en los historiales de origen y destino.
- **RegistroClientes o-- Cliente:** el registro administra una colección de clientes, pero el cliente es una entidad del dominio con identidad propia.
- **RepositorioBilleteras o-- Billetera:** el repositorio solo indexa billeteras; no controla su ciclo de vida, por eso es agregación.
- **Billetera ..> RepositorioBilleteras:** dependencia de uso; el repositorio llega como parámetro de `TransferirPorIdentificador` y la billetera no lo almacena.

### Visibilidad de los métodos de Billetera

`Acreditar`, `Debitar` y `RegistrarMovimiento` son `internal` (`~` en el diagrama): solo los movimientos del dominio pueden modificar el saldo o el historial. Desde afuera, la única forma de alterar una billetera es a través de `CargarDinero`, `Transferir`, `TransferirPorIdentificador` y `PagarEnCuotas`.

### Template Method

La clase abstracta `Movimiento` define el flujo general en `Ejecutar()`, que no es redefinible:

```text
Ejecutar()
  → Validar()
  → Aplicar()
  → Registrar()
```

`Carga`, `Transferencia` y `PagoEnCuotas` redefinen esos pasos porque su comportamiento es diferente:

| Movimiento | Validar | Aplicar | Registrar |
|---|---|---|---|
| **Carga** | monto > 0 | acredita en la billetera | historial de la billetera |
| **Transferencia** | monto > 0, origen ≠ destino, límite permitido, saldo suficiente | debita en origen y acredita en destino | historial de origen y de destino |
| **PagoEnCuotas** | cuotas > 0, monto > 0, sin deudas pendientes, saldo suficiente | debita el monto total | historial de la billetera |

## Diagrama de secuencia - Realizar transferencia

```mermaid
sequenceDiagram
    actor Titular
    participant Origen as origen:Billetera
    participant Repo as repositorio:RepositorioBilleteras
    participant Transf as transferencia:Transferencia
    participant Limite as limite:LimiteTransferencia
    participant Destino as destino:Billetera
    participant HOrigen as historialOrigen:HistorialMovimientos
    participant HDestino as historialDestino:HistorialMovimientos

    Titular->>Origen: TransferirPorIdentificador(aliasOCvu, monto, repositorio)
    Origen->>Repo: BuscarPorIdentificador(aliasOCvu)
    Repo-->>Origen: destino

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

    else Identificador inexistente
        Repo-->>Origen: InvalidOperationException
        Origen-->>Titular: transferencia rechazada

    else Monto invalido o destino igual al origen
        Transf-->>Titular: excepcion, transferencia rechazada

    else Supera el limite permitido
        Transf-->>Titular: excepcion, transferencia rechazada

    else Saldo insuficiente
        Transf-->>Titular: excepcion, transferencia rechazada
    end
```

## Diagrama de secuencia - Pagar en cuotas

```mermaid
sequenceDiagram
    actor Titular
    participant Bill as billetera:Billetera
    participant Pago as pago:PagoEnCuotas
    participant Hist as historial:HistorialMovimientos

    Titular->>Bill: PagarEnCuotas(montoTotal, cantidadCuotas)
    Bill->>Pago: Crear PagoEnCuotas(billetera, montoTotal, cantidadCuotas)
    Bill->>Pago: Ejecutar()

    Pago->>Pago: Validar()
    Note right of Pago: cuotas > 0, monto > 0, sin deudas pendientes y saldo suficiente

    Pago->>Bill: PoseeDeudasPendientes()
    Bill-->>Pago: poseeDeudas
    Pago->>Bill: TieneSaldoSuficiente(montoTotal)
    Bill-->>Pago: saldoSuficiente

    alt Validaciones correctas
        Pago->>Pago: Aplicar()
        Pago->>Bill: Debitar(montoTotal)

        Pago->>Pago: Registrar()
        Pago->>Bill: RegistrarMovimiento(pago)
        Bill->>Hist: AgregarMovimiento(pago)

        Bill-->>Titular: pago registrado
    else Cuotas o monto invalidos
        Pago-->>Titular: ArgumentException, pago rechazado
    else Posee deudas pendientes
        Pago-->>Titular: InvalidOperationException, pago rechazado
    else Saldo insuficiente
        Pago-->>Titular: InvalidOperationException, pago rechazado
    end
```

## Decisiones de diseño del flujo

- El diagrama de transferencia comienza con la búsqueda de la billetera destino por alias o CVU. `Transferir(destino, monto)` es el mismo flujo, pero con el destino ya resuelto.
- La validación se concentra en `Transferencia.Validar()`.
- Antes de modificar los saldos se verifica el monto, que origen y destino sean distintos, el límite permitido y el saldo disponible.
- `Transferencia.Aplicar()` realiza el débito en origen y la acreditación en destino.
- `Transferencia.Registrar()` registra la misma transferencia en ambos historiales a través de las billeteras.
- El acceso al historial queda encapsulado mediante `Billetera.RegistrarMovimiento()`.
- Las validaciones fallidas se informan con excepciones (`ArgumentException` para datos inválidos e `InvalidOperationException` para reglas de negocio); como `Validar()` corre antes de `Aplicar()`, un rechazo nunca deja saldos ni historiales modificados.
- `PagoEnCuotas` sigue el mismo flujo del Template Method: se debita el monto total en una sola operación y se guarda la cantidad de cuotas en el movimiento.


    
