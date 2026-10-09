using System;
using BilleteraVirtual.Dominio;
using Xunit;

namespace BilleteraVirtual.Tests.Dominio;

public class TransferenciaTests
{
    // Caso Normal: Transferencia entre dos billeteras distintas
    [Fact]
    public void Ejecutar_DatosValidos_TransfiereFondosYRegistraEnAmbosHistoriales()
    {
        var origen = new Billetera();
        var destino = new Billetera();
        origen.CargarDinero(5000m);

        origen.Transferir(destino, 2000m);

        Assert.Equal(3000m, origen.ConsultarSaldo());
        Assert.Equal(2000m, destino.ConsultarSaldo());
        Assert.Single(origen.ObtenerMovimientos());
        Assert.Single(destino.ObtenerMovimientos());
    }

    // Caso Borde: Transferir hacia la misma billetera
    [Fact]
    public void Ejecutar_MismaBilletera_LanzaInvalidOperationException()
    {
        var billetera = new Billetera();
        billetera.CargarDinero(1000m);

        Assert.Throws<InvalidOperationException>(() => billetera.Transferir(billetera, 500m));
        Assert.Equal(1000m, billetera.ConsultarSaldo());
    }

    // Caso de Error: Transferencia superando el límite permitido
    [Fact]
    public void Ejecutar_MontoSuperaLimite_LanzaInvalidOperationException()
    {
        var origen = new Billetera(1000m);
        var destino = new Billetera();
        origen.CargarDinero(5000m);

        Assert.Throws<InvalidOperationException>(() => origen.Transferir(destino, 2000m));
        Assert.Equal(5000m, origen.ConsultarSaldo());
    }
}