using System;

namespace BilleteraVirtual.Dominio;

public class PagoEnCuotas : Movimiento
{
    private Billetera billetera;
    private int cantidadCuotas;
    private int cuotasPagadas;

    public int CantidadCuotas => this.cantidadCuotas;
    public int CuotasPagadas => this.cuotasPagadas;
    public int CuotasRestantes => this.cantidadCuotas - this.cuotasPagadas;

    // Monto original de la compra (sin interés) y porcentaje aplicado según el plan.
    // this.Monto pasa a ser el TOTAL con interés.
    public decimal MontoBase { get; }
    public decimal PorcentajeInteres { get; }

    // Valor de una cuota común: monto total (con interés) / cuotas, truncado a 2 decimales.
    // La última cuota absorbe la diferencia de redondeo (ver ObtenerMontoProximaCuota).
    public decimal ValorCuota => this.cantidadCuotas > 0
        ? Math.Round(this.Monto / this.cantidadCuotas, 2, MidpointRounding.ToZero)
        : 0m;

    public PagoEnCuotas(Billetera billetera, decimal montoBase, int cantidadCuotas)
        : base(CalcularMontoTotal(montoBase, cantidadCuotas))
    {
        this.billetera = billetera ?? throw new ArgumentNullException(nameof(billetera), "La billetera no puede ser nula.");
        this.MontoBase = montoBase;
        this.cantidadCuotas = cantidadCuotas;
        this.cuotasPagadas = 0;
        this.PorcentajeInteres = ObtenerPorcentajeInteres(cantidadCuotas);
    }

    // Interés según el plan elegido. Cualquier otra cantidad de cuotas no es válida.
    private static decimal ObtenerPorcentajeInteres(int cuotas) => cuotas switch
    {
        1 => 0m,      // una sola cuota: sin interés
        2 => 0.05m,   // 5%
        3 => 0.10m,   // 10%
        6 => 0.25m,   // 25%
        12 => 0.50m,  // 50%
        _ => throw new ArgumentException($"El plan de {cuotas} cuotas no es válido. Usar: 1, 2, 3, 6 o 12.", nameof(cuotas))
    };

    // Total a pagar = monto base + interés del plan, redondeado a 2 decimales.
    private static decimal CalcularMontoTotal(decimal montoBase, int cuotas)
    {
        if (montoBase <= 0)
        {
            throw new ArgumentException("El monto debe ser mayor a cero.", nameof(montoBase));
        }

        return Math.Round(montoBase * (1m + ObtenerPorcentajeInteres(cuotas)), 2, MidpointRounding.AwayFromZero);
    }

    // Si es la última, devuelve lo que falta para completar el total exacto.
    internal decimal ObtenerMontoProximaCuota()
    {
        if (this.CuotasRestantes <= 0)
        {
            throw new InvalidOperationException("El pago en cuotas ya está completamente saldado.");
        }

        if (this.CuotasRestantes == 1)
        {
            return this.Monto - (this.ValorCuota * (this.cantidadCuotas - 1));
        }

        return this.ValorCuota;
    }

    internal void RegistrarCuotaPagada()
    {
        this.cuotasPagadas++;
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

        // El monto debe poder dividirse en cuotas de al menos un centavo
        if (this.ValorCuota <= 0)
        {
            throw new ArgumentException("El monto es demasiado chico para dividirlo en esa cantidad de cuotas.", nameof(this.Monto));
        }

        // Validar que el usuario no posea deudas pendientes activas (lanza InvalidOperationException)
        if (this.billetera.PoseeDeudasPendientes())
        {
            throw new InvalidOperationException("No se puede realizar el pago en cuotas porque posee deudas pendientes activas.");
        }

        // Validar que la billetera cuente con saldo suficiente (lanza InvalidOperationException)
        if (!this.billetera.TieneSaldoSuficiente(this.ObtenerMontoProximaCuota()))
        {
            throw new InvalidOperationException("La billetera no posee saldo suficiente para pagar la primera cuota.");
        }
    }

    protected override void Aplicar()
    {
        // Se debita solamente la primera cuota
        this.billetera.Debitar(this.ObtenerMontoProximaCuota());
        this.RegistrarCuotaPagada();

        // Si quedan cuotas por pagar, la billetera pasa a tener una deuda pendiente
        if (this.CuotasRestantes > 0)
        {
            this.billetera.IniciarPlanDeCuotas(this);
        }
    }

    protected override void Registrar()
    {
        // Registrar la operación en el HistorialMovimientos con tipo "Pago en Cuotas"
        this.billetera.RegistrarMovimiento(this);
    }

    public override string ObtenerTipo() => "Pago en Cuotas";
}
