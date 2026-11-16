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


    // Valor de una cuota común: monto total / cuotas, truncado a 2 decimales.
    // La última cuota absorbe la diferencia de redondeo (ver ObtenerMontoProximaCuota).
    public decimal ValorCuota => this.cantidadCuotas > 0
        ? Math.Round(this.Monto / this.cantidadCuotas, 2, MidpointRounding.ToZero)
        : 0m;
    public PagoEnCuotas(Billetera billetera, decimal montoTotal, int cantidadCuotas) : base(montoTotal)
    {
        this.billetera = billetera ?? throw new ArgumentNullException(nameof(billetera), "La billetera no puede ser nula.");
        this.cantidadCuotas = cantidadCuotas;
        this.cuotasPagadas = 0;
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
        2 => 0.05m,  // 5%
        3 => 0.10m,  // 10%
        6 => 0.25m,  // 25%
        12 => 0.50m, // 50%
        _ => throw new ArgumentException($"El plan de {cuotas} cuotas no es válido. Usar: 2, 3, 6 o 12.", nameof(cuotas))
    };

    private static decimal CalcularMontoTotal(decimal montoBase, int cuotas)
    {
        if (montoBase <= 0)
        {
            throw new ArgumentException("El monto debe ser mayor a cero.", nameof(montoBase));
        }

        // El monto debe poder dividirse en cuotas de al menos un centavo
        if (this.ValorCuota <= 0)
        {
            throw new ArgumentException("El monto es demasiado chico para dividirlo en esa cantidad de cuotas.", nameof(this.Monto));
        }

        //Validar que el usuario no posea deudas pendientes activas (lanza InvalidOperationException)
        if (this.billetera.PoseeDeudasPendientes())
        {
            throw new InvalidOperationException("No se puede pagar en cuotas si posee deudas activas.");
        }

        //Validar que la billetera cuente con saldo suficiente (lanza InvalidOperationException)
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
        this.billetera.RegistrarMovimiento(this);
    }

    public override string ObtenerTipo() => "Pago en Cuotas";
}