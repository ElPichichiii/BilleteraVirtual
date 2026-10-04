namespace BilleteraVirtual.Dominio;

public class HistorialMovimientos
{
    private readonly List<Movimiento> movimientos = new();

    public void AgregarMovimiento(Movimiento movimiento)
    {
        movimientos.Add(movimiento);
    }

    public List<Movimiento> ObtenerMovimientos()
    {
        return new List<Movimiento>(movimientos);
    }
}
