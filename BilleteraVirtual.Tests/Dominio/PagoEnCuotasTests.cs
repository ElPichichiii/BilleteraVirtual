using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BilleteraVirtual.Dominio;

namespace BilleteraVirtual.Tests.Dominio;

public class PagoEnCuotasTests
{
    [Fact]
    public void PagarEnCuotas_CondicionesValidas_DescuentaSaldoYRegistraMovimiento()
    {
        var billetera = new Billetera();
        billetera.CargarDinero(10000.00m); // Saldo inicial

        billetera.PagarEnCuotas(3000.00m, 3); // 3 cuotas válidas

        Assert.Equal(7000.00m, billetera.ConsultarSaldo());
        var movimientos = billetera.ObtenerMovimientos();
        Assert.Equal(2, movimientos.Count); // Carga + Pago en Cuotas
        Assert.Equal("Pago en Cuotas", movimientos.Last().ObtenerTipo());
        Assert.Equal(3000.00m, movimientos.Last().Monto);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-2)]
    public void PagarEnCuotas_CuotasInvalidas_LanzaArgumentException(int cuotasInvalidas)
    {
        var billetera = new Billetera();
        billetera.CargarDinero(5000.00m);

        Assert.Throws<ArgumentException>(() =>
            billetera.PagarEnCuotas(1500.00m, cuotasInvalidas)
        );
        Assert.Equal(5000.00m, billetera.ConsultarSaldo());
    }

    [Fact]
    public void PagarEnCuotas_PoseeDeudasPendientes_LanzaInvalidOperationExceptionYMantieneSaldoIntacto()
    {
        var billetera = new Billetera();
        billetera.CargarDinero(5000.00m);
        billetera.EstablecerDeudasPendientes(true); // Simulamos deuda activa

        Assert.Throws<InvalidOperationException>(() =>
            billetera.PagarEnCuotas(2000.00m, 3)
        );

        Assert.Equal(5000.00m, billetera.ConsultarSaldo());
        Assert.Single(billetera.ObtenerMovimientos());
    }

    [Fact]
    public void PagarEnCuotas_SaldoInsuficiente_LanzaInvalidOperationExceptionYMantieneSaldoIntacto()
    {
        var billetera = new Billetera();
        billetera.CargarDinero(1000.00m); // Saldo menor al pago

        Assert.Throws<InvalidOperationException>(() =>
            billetera.PagarEnCuotas(2500.00m, 6)
        );

        Assert.Equal(1000.00m, billetera.ConsultarSaldo());
        Assert.Single(billetera.ObtenerMovimientos());
    }
}