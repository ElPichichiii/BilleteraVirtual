using System;
using BilleteraVirtual.Dominio;
using Xunit;

namespace BilleteraVirtual.Tests.Dominio;

public class HistorialMovimientosTests
{
    // Caso Normal: Agregar y consultar movimientos
    [Fact]
    public void AgregarMovimiento_MovimientoValido_LoGuardaEnElHistorial()
    {
        var historial = new HistorialMovimientos();
        var billetera = new Billetera();
        var carga = new Carga(billetera, 1000m);

        historial.AgregarMovimiento(carga);

        var movimientos = historial.ObtenerMovimientos();
        Assert.Single(movimientos);
        Assert.Same(carga, movimientos[0]);
    }

    // Caso Borde: Ordenar historial sin movimientos devuelve lista vacía
    [Fact]
    public void OrdenarPorFechaDescendente_HistorialVacio_DevuelveListaVacia()
    {
        var historial = new HistorialMovimientos();

        var ordenados = historial.OrdenarPorFechaDescendente();

        Assert.Empty(ordenados);
    }

    // Caso de Error: Intentar agregar un movimiento nulo
    [Fact]
    public void AgregarMovimiento_Nulo_LanzaArgumentNullException()
    {
        var historial = new HistorialMovimientos();

        Assert.Throws<ArgumentNullException>(() => historial.AgregarMovimiento(null!));
    }
}