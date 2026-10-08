namespace BilleteraVirtual.Dominio;

public class Billetera
{
    private decimal saldo;
    private readonly HistorialMovimientos historial;

    public Billetera()
    {
        saldo = 0m;
        historial = new HistorialMovimientos();
    }

    public decimal ConsultarSaldo()
    {
        return saldo;
    }

    public void CargarDinero(decimal monto)
    {
        int id = historial.ObtenerMovimientos().Count + 1;
        var carga = new Carga(id, monto, this);

        carga.Ejecutar();
    }

    public List<Movimiento> ObtenerMovimientos()
    {
        return historial.ObtenerMovimientos();
    }

    internal void Acreditar(decimal monto)
    {
        saldo += monto;
    }

    internal void RegistrarMovimiento(Movimiento movimiento)
    {
        historial.AgregarMovimiento(movimiento);
    }
}
