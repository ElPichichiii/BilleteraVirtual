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

        Assert.Equal(9000.00m, billetera.ConsultarSaldo()); // Solo se debita la primera cuota (3000 / 3)
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
        billetera.CargarDinero(100.00m); // Saldo menor a la primera cuota (2500 / 6 = 416,66)

        Assert.Throws<InvalidOperationException>(() =>
            billetera.PagarEnCuotas(2500.00m, 6)
        );

        Assert.Equal(100.00m, billetera.ConsultarSaldo());
        Assert.Single(billetera.ObtenerMovimientos());
    }

    [Fact]
    public void PagarEnCuotas_CondicionesValidas_DebitaSoloLaPrimeraCuotaYQuedaConDeuda()
    {
        var billetera = new Billetera();
        billetera.CargarDinero(10000.00m);

        billetera.PagarEnCuotas(3000.00m, 3);

        Assert.Equal(9000.00m, billetera.ConsultarSaldo());
        Assert.True(billetera.PoseeDeudasPendientes());
        Assert.Equal(2, billetera.CuotasPendientes());
    }

    [Fact]
    public void PagarCuota_HastaCompletarElPlan_DebitaTodoElMontoYCancelaLaDeuda()
    {
        var billetera = new Billetera();
        billetera.CargarDinero(10000.00m);
        billetera.PagarEnCuotas(3000.00m, 3);

        billetera.PagarCuota();
        billetera.PagarCuota();

        Assert.Equal(7000.00m, billetera.ConsultarSaldo());
        Assert.False(billetera.PoseeDeudasPendientes());
        Assert.Equal(0, billetera.CuotasPendientes());
    }

    [Fact]
    public void PagarCuota_MontoNoDivisible_LaUltimaCuotaAbsorbeLaDiferencia()
    {
        var billetera = new Billetera();
        billetera.CargarDinero(1000.00m);

        billetera.PagarEnCuotas(100.00m, 3); // 33,33 + 33,33 + 33,34
        billetera.PagarCuota();
        billetera.PagarCuota();

        Assert.Equal(900.00m, billetera.ConsultarSaldo());
        Assert.False(billetera.PoseeDeudasPendientes());
    }

    [Fact]
    public void PagarEnCuotas_UnaSolaCuota_DebitaTodoYNoDejaDeuda()
    {
        var billetera = new Billetera();
        billetera.CargarDinero(5000.00m);

        billetera.PagarEnCuotas(2000.00m, 1);

        Assert.Equal(3000.00m, billetera.ConsultarSaldo());
        Assert.False(billetera.PoseeDeudasPendientes());
        Assert.Equal(0, billetera.CuotasPendientes());
    }

    [Fact]
    public void PagarCuota_SinPlanPendiente_LanzaInvalidOperationException()
    {
        var billetera = new Billetera();
        billetera.CargarDinero(1000.00m);

        Assert.Throws<InvalidOperationException>(() => billetera.PagarCuota());
        Assert.Equal(1000.00m, billetera.ConsultarSaldo());
    }

    [Fact]
    public void PagarCuota_SaldoInsuficiente_LanzaInvalidOperationExceptionYMantieneLaDeuda()
    {
        var billetera = new Billetera();
        billetera.CargarDinero(1000.00m);
        billetera.PagarEnCuotas(3000.00m, 3); // Debita 1000 y deja el saldo en 0

        Assert.Throws<InvalidOperationException>(() => billetera.PagarCuota());

        Assert.Equal(0m, billetera.ConsultarSaldo());
        Assert.True(billetera.PoseeDeudasPendientes());
        Assert.Equal(2, billetera.CuotasPendientes());
    }

    [Fact]
    public void PagarEnCuotas_ConPlanEnCurso_LanzaInvalidOperationException()
    {
        var billetera = new Billetera();
        billetera.CargarDinero(10000.00m);
        billetera.PagarEnCuotas(3000.00m, 3);

        Assert.Throws<InvalidOperationException>(() => billetera.PagarEnCuotas(500.00m, 2));
        Assert.Equal(9000.00m, billetera.ConsultarSaldo());
    }
}