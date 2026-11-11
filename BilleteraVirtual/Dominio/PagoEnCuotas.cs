using System;

namespace BilleteraVirtual.Dominio;

public class PagoEnCuotas : Movimiento
{
    private readonly Billetera billetera;

    public int CantidadCuotas { get; }
    public decimal MontoBase { get; }
    public decimal PorcentajeInteres { get; }
    public decimal ValorCuota => this.Monto / this.CantidadCuotas;

    public PagoEnCuotas(Billetera billetera, decimal montoBase, int cantidadCuotas)
        : base(CalcularMontoTotal(montoBase, cantidadCuotas))
    {
        this.billetera = billetera ?? throw new ArgumentNullException(nameof(billetera));
        this.MontoBase = montoBase;
        this.CantidadCuotas = cantidadCuotas;
        this.PorcentajeInteres = ObtenerPorcentajeInteres(cantidadCuotas);
    }

    private static decimal ObtenerPorcentajeInteres(int cuotas) => cuotas switch
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

        return montoBase * (1m + ObtenerPorcentajeInteres(cuotas));
    }

    protected override void Validar()
    {
        if (this.billetera.PoseeDeudasPendientes())
        {
            throw new InvalidOperationException("No se puede pagar en cuotas si posee deudas activas.");
        }

        if (!this.billetera.TieneSaldoSuficiente(this.Monto))
        {
            throw new InvalidOperationException("Saldo insuficiente para cubrir el pago total con intereses.");
        }
    }

    protected override void Aplicar()
    {
        this.billetera.Debitar(this.Monto);
    }

    protected override void Registrar()
    {
        this.billetera.RegistrarMovimiento(this);
    }

    public override string ObtenerTipo() => "Pago en Cuotas";
}