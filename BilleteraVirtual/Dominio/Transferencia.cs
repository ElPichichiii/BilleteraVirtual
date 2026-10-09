using System;
using System.Collections.Generic;
using System.Text;

namespace BilleteraVirtual.Dominio;

public class Transferencia : Movimiento
{
    private Billetera origen;
    private Billetera destino;

    public Transferencia(Billetera origen, Billetera destino, decimal monto) : base(monto)
    {
        this.origen = origen ?? throw new ArgumentNullException(nameof(origen), "La billetera de origen no puede ser nula.");
        this.destino = destino ?? throw new ArgumentNullException(nameof(destino), "La billetera de destino no puede ser nula.");
    }

    protected override void Validar()
    {
        if (this.Monto <= 0)
        {
            throw new ArgumentException("El monto a transferir debe ser mayor a cero.", nameof(this.Monto));
        }

        if (this.origen == this.destino)
        {
            throw new InvalidOperationException("La billetera de origen y destino no pueden ser la misma.");
        }

        if (!this.origen.MontoDentroDelLimite(this.Monto))
        {
            throw new InvalidOperationException("El monto a transferir supera el límite permitido.");
        }

        if (!this.origen.TieneSaldoSuficiente(this.Monto))
        {
            throw new InvalidOperationException("La billetera de origen no posee saldo suficiente.");
        }
    }

    protected override void Aplicar()
    {
        // Utiliza los métodos internos de Billetera para debitar y acreditar
        this.origen.Debitar(this.Monto);
        this.destino.Acreditar(this.Monto);
    }

    protected override void Registrar()
    {
        // Se registra en ambas billeteras para que quede el comprobante en los dos historiales
        this.origen.RegistrarMovimiento(this);
        this.destino.RegistrarMovimiento(this);
    }

    public override string ObtenerTipo() => "Transferencia";
}