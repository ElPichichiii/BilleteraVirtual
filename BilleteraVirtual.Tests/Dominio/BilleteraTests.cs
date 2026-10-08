using BilleteraVirtual.Dominio;

namespace BilleteraVirtual.Tests.Dominio;

public class BilleteraTests
{
    [Fact]
    public void ConsultarSaldo_EnBilleteraNueva_DevuelveCero()
    {
        var billetera = new Billetera();

        Assert.Equal(0m, billetera.ConsultarSaldo());
    }

    [Fact]
    public void ConsultarSaldo_AlConsultarloDosVeces_NoModificaSaldo()
    {
        var billetera = new Billetera();

        var saldoPrimeraConsulta = billetera.ConsultarSaldo();
        var saldoSegundaConsulta = billetera.ConsultarSaldo();

        Assert.Equal(saldoPrimeraConsulta, saldoSegundaConsulta);
    }

    [Fact]
    public void CargarDinero_ConMontoPositivo_AumentaSaldo()
    {
        var billetera = new Billetera();

        billetera.CargarDinero(100m);

        Assert.Equal(100m, billetera.ConsultarSaldo());
    }

    [Fact]
    public void CargarDinero_VariasVeces_AcumulaSaldo()
    {
        var billetera = new Billetera();

        billetera.CargarDinero(100m);
        billetera.CargarDinero(50m);

        Assert.Equal(150m, billetera.ConsultarSaldo());
    }

    [Fact]
    public void CargarDinero_ConMontoPositivo_RegistraLaCarga()
    {
        var billetera = new Billetera();

        billetera.CargarDinero(100m);

        var movimiento = Assert.Single(billetera.ObtenerMovimientos());
        var carga = Assert.IsType<Carga>(movimiento);
        Assert.Equal(1, carga.Id);
        Assert.Equal(100m, carga.Monto);
        Assert.Equal("Carga", carga.ObtenerTipo());
        Assert.NotEqual(default, carga.Fecha);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-10)]
    public void CargarDinero_ConMontoInvalido_NoModificaSaldoNiHistorial(decimal monto)
    {
        var billetera = new Billetera();
        billetera.CargarDinero(100m);

        var excepcion = Assert.Throws<ArgumentException>(
            () => billetera.CargarDinero(monto));

        Assert.Equal("monto", excepcion.ParamName);
        Assert.Equal(100m, billetera.ConsultarSaldo());
        Assert.Single(billetera.ObtenerMovimientos());
    }
}

