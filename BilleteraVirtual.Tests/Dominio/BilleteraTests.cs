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
    public void RetirarSaldo_MontoValidoYSaldoSuficiente_RestaElSaldo()
    {
        var billetera = new Billetera();
        billetera.DepositarSaldo(1000m);

        billetera.RetirarSaldo(400m);

        Assert.Equal(600m, billetera.ConsultarSaldo());
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-50)]
    public void RetirarSaldo_MontoMenorOIgualACero_LanzaArgumentException(decimal montoInvalido)
    {
        var billetera = new Billetera();

        Assert.Throws<ArgumentException>(() => billetera.RetirarSaldo(montoInvalido));
    }

    [Fact]
    public void RetirarSaldo_MontoMayorAlSaldo_LanzaInvalidOperationException()
    {
        var billetera = new Billetera();
        billetera.DepositarSaldo(200m);

        Assert.Throws<InvalidOperationException>(() => billetera.RetirarSaldo(500m));
    }




}

