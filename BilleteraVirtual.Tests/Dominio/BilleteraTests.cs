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
}

