namespace BilleteraVirtual.Dominio;

public class Carga : Movimiento
{
    private readonly Billetera billetera;

    public Carga(int id, decimal monto, Billetera billetera)
        : base(id, monto)
    {
        this.billetera = billetera;
    }

    protected override void Validar()
    {
        if (Monto <= 0)
        {
            throw new ArgumentException("El monto debe ser mayor a cero.", "monto");
        }
    }

    protected override void Aplicar()
    {
        billetera.Acreditar(Monto);
    }

    protected override void Registrar()
    {
        billetera.RegistrarMovimiento(this);
    }

    public override string ObtenerTipo()
    {
        return "Carga";
    }
}
