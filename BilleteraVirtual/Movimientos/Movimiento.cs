namespace BilleteraVirtual.Dominio;

public abstract class Movimiento
{
    public int Id { get; }
    public decimal Monto { get; }
    public DateTime Fecha { get; }

    protected Movimiento(int id, decimal monto)
    {
        Id = id;
        Monto = monto;
        Fecha = DateTime.Now;
    }

    public void Ejecutar()
    {
        Validar();
        Aplicar();
        Registrar();
    }

    protected abstract void Validar();

    protected abstract void Aplicar();

    protected abstract void Registrar();

    public abstract string ObtenerTipo();
}
