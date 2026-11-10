using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System;

namespace BilleteraVirtual.Dominio;

public class PagoEnCuotas : Movimiento
{
    private Billetera billetera;
    private int cantidadCuotas;

    public int CantidadCuotas => this.cantidadCuotas;

    public PagoEnCuotas(Billetera billetera, decimal montoTotal, int cantidadCuotas) : base(montoTotal)
    {
        this.billetera = billetera ?? throw new ArgumentNullException(nameof(billetera), "La billetera no puede ser nula.");
        this.cantidadCuotas = cantidadCuotas;
    }

    protected override void Validar()
    {
        // Validar que la cantidad de cuotas sea válida (mayor a 0)
        if (this.cantidadCuotas <= 0)
        {
            throw new ArgumentException("La cantidad de cuotas debe ser mayor a cero.", nameof(this.cantidadCuotas));
        }

        if (this.Monto <= 0)
        {
            throw new ArgumentException("El monto total del pago debe ser mayor a cero.", nameof(this.Monto));
        }

        //Validar que el usuario no posea deudas pendientes activas (lanza InvalidOperationException)
        if (this.billetera.PoseeDeudasPendientes())
        {
            throw new InvalidOperationException("No se puede realizar el pago en cuotas porque posee deudas pendientes activas.");
        }

        //Validar que la billetera cuente con saldo suficiente (lanza InvalidOperationException)
        if (!this.billetera.TieneSaldoSuficiente(this.Monto))
        {
            throw new InvalidOperationException("La billetera no posee saldo suficiente para cubrir la operación.");
        }
    }

    protected override void Aplicar()
    {
        //Debitar el saldo correspondiente
        this.billetera.Debitar(this.Monto);
    }

    protected override void Registrar()
    {
        //Registrar la operación en el HistorialMovimientos con tipo "Pago en Cuotas"
        this.billetera.RegistrarMovimiento(this);
    }

    public override string ObtenerTipo() => "Pago en Cuotas";
}
