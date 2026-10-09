using System;
using BilleteraVirtual.Dominio;
using Xunit;

namespace BilleteraVirtual.Tests.Dominio;

public class CargaTests
{
    // Caso Normal: Ejecución de carga vía Template Method
    [Fact]
    public void Ejecutar_MontoValido_AcreditaSaldoYRegistraEnHistorial()
    {
        var billetera = new Billetera();
        var carga = new Carga(billetera, 2000m);

        carga.Ejecutar();

        Assert.Equal(2000m, billetera.ConsultarSaldo());
        Assert.Equal("Carga", carga.ObtenerTipo());
        Assert.Single(billetera.ObtenerMovimientos());
    }

    // Caso Borde: Instanciar Carga con Billetera nula
    [Fact]
    public void CrearCarga_BilleteraNula_LanzaArgumentNullException()
    {
        Assert.Throws<ArgumentNullException>(() => new Carga(null!, 500m));
    }

    // Caso de Error: Carga con monto menor o igual a cero
    [Theory]
    [InlineData(0)]
    [InlineData(-100)]
    public void Ejecutar_MontoInvalido_LanzaArgumentException(decimal montoInvalido)
    {
        var billetera = new Billetera();
        var carga = new Carga(billetera, montoInvalido);

        Assert.Throws<ArgumentException>(() => carga.Ejecutar());
        Assert.Equal(0m, billetera.ConsultarSaldo());
    }
}