using System;
using System.Linq;
using BilleteraVirtual.Dominio;
using Xunit;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;


namespace BilleteraVirtual.Tests.Dominio;

public class PagoEnCuotasTests
{
    [Fact]
    public void PagarEnCuotas_CondicionesValidas_DescuentaSaldoYRegistraMovimiento()
    {
        var billetera = new Billetera();
        billetera.CargarDinero(10000.00m); // Saldo inicial

        billetera.PagarEnCuotas(3000.00m, 3); // 3 cuotas válidas (10% de interés: total 3300)

        Assert.Equal(8900.00m, billetera.ConsultarSaldo()); // Solo se debita la primera cuota (3300 / 3 = 1100)
        var movimientos = billetera.ObtenerMovimientos();
        Assert.Equal(2, movimientos.Count); // Carga + Pago en Cuotas
        Assert.Equal("Pago en Cuotas", movimientos.Last().ObtenerTipo());
        Assert.Equal(3300.00m, movimientos.Last().Monto); // El movimiento guarda el total con interés
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-2)]
    [InlineData(5)] // plan inexistente: solo se permiten 1, 2, 3, 6 o 12
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
        billetera.CargarDinero(100.00m); // Saldo menor a la primera cuota (2500 + 25% = 3125; 3125 / 6 = 520,83)

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

        billetera.PagarEnCuotas(3000.00m, 3); // Total 3300, cuota 1100

        Assert.Equal(8900.00m, billetera.ConsultarSaldo());
        Assert.True(billetera.PoseeDeudasPendientes());
        Assert.Equal(2, billetera.CuotasPendientes());
    }

    [Fact]
    public void PagarCuota_HastaCompletarElPlan_DebitaTodoElMontoYCancelaLaDeuda()
    {
        var billetera = new Billetera();
        billetera.CargarDinero(10000.00m);
        billetera.PagarEnCuotas(3000.00m, 3); // Total 3300

        billetera.PagarCuota();
        billetera.PagarCuota();

        Assert.Equal(6700.00m, billetera.ConsultarSaldo()); // 10000 - 3300
        Assert.False(billetera.PoseeDeudasPendientes());
        Assert.Equal(0, billetera.CuotasPendientes());
    }

    [Fact]
    public void PagarCuota_MontoNoDivisible_LaUltimaCuotaAbsorbeLaDiferencia()
    {
        var billetera = new Billetera();
        billetera.CargarDinero(1000.00m);

        billetera.PagarEnCuotas(100.00m, 3); // Total 110: 36,66 + 36,66 + 36,68
        billetera.PagarCuota();
        billetera.PagarCuota();

        Assert.Equal(890.00m, billetera.ConsultarSaldo()); // 1000 - 110
        Assert.False(billetera.PoseeDeudasPendientes());
    }

    [Fact]
    public void PagarEnCuotas_UnaSolaCuota_DebitaTodoYNoDejaDeuda()
    {
        var billetera = new Billetera();
        billetera.CargarDinero(5000.00m);

        billetera.PagarEnCuotas(2000.00m, 1); // Una cuota: sin interés

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
        billetera.CargarDinero(1100.00m);
        billetera.PagarEnCuotas(3000.00m, 3); // Total 3300: debita 1100 y deja el saldo en 0

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
        Assert.Equal(8900.00m, billetera.ConsultarSaldo());
    }

    // Test nuevo: el interés depende del plan elegido (sobre un monto base de 1000)
    [Theory]
    [InlineData(2, 1050.00)]   // +5%
    [InlineData(3, 1100.00)]   // +10%
    [InlineData(6, 1250.00)]   // +25%
    [InlineData(12, 1500.00)]  // +50%
    public void PagarEnCuotas_AplicaElInteresSegunElPlan(int cuotas, double totalEsperado)
    {
        var billetera = new Billetera();
        billetera.CargarDinero(10000.00m);

        billetera.PagarEnCuotas(1000.00m, cuotas);

        Assert.Equal((decimal)totalEsperado, billetera.ObtenerMovimientos().Last().Monto);
    }
}
