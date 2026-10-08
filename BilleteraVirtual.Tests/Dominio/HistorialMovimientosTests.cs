using BilleteraVirtual.Dominio;

namespace BilleteraVirtual.Tests.Dominio;

public class HistorialMovimientosTests
{
    [Fact]
    public void ObtenerMovimientos_EnHistorialNuevo_DevuelveListaVacia()
    {
        var historial = new HistorialMovimientos();

        var movimientos = historial.ObtenerMovimientos();

        Assert.Empty(movimientos);
    }

    [Fact]
    public void AgregarMovimiento_AgregaMovimientoAlHistorial()
    {
        var historial = new HistorialMovimientos();
        var movimiento = new Carga(1, 100m, new Billetera());

        historial.AgregarMovimiento(movimiento);

        Assert.Same(movimiento, Assert.Single(historial.ObtenerMovimientos()));
    }

    [Fact]
    public void ObtenerMovimientos_AlModificarLaListaDevuelta_NoModificaHistorial()
    {
        var historial = new HistorialMovimientos();
        var movimiento = new Carga(1, 100m, new Billetera());
        historial.AgregarMovimiento(movimiento);

        var movimientos = historial.ObtenerMovimientos();
        movimientos.Clear();

        Assert.Single(historial.ObtenerMovimientos());
    }
}
