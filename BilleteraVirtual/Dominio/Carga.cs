using System;
using System.Collections.Generic;
namespace BilleteraVirtual.Dominio;

public class Carga : Movimiento
{
    private Billetera billetera;

    public Carga(Billetera billetera, decimal monto) : base(monto)
    {
        this.billetera = billetera ?? throw new ArgumentNullException(nameof(billetera), "La billetera no puede ser nula.");
    }

    protected override void Validar()
    {
        if (this.Monto <= 0)
        {
            throw new ArgumentException("El monto a cargar debe ser mayor a cero.", nameof(this.Monto));
        }
    }

    protected override void Aplicar()
    {
        // Utiliza el método interno (~) de Billetera para modificar el saldo
        this.billetera.Acreditar(this.Monto);
    }

    protected override void Registrar()
    {
        // Utiliza el método interno (~) de Billetera para guardar en el HistorialMovimientos
        this.billetera.RegistrarMovimiento(this);
    }

    public override string ObtenerTipo() => "Carga";
}