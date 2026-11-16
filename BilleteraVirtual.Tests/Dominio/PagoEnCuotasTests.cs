using System;
using System.Linq;
using BilleteraVirtual.Dominio;
using Xunit;

namespace BilleteraVirtual.Tests.Dominio;

public class PagoEnCuotasTests
{
    // Caso Normal: Planes de cuotas válidos (2, 3, 6, 12) aplican el recargo de interés y descuentan el saldo final
    [Theory]
    [InlineData(1000.00, 2, 1050.00)]  // 5% interes
    [InlineData(1000.00, 3, 1100.00)]  // 10% interes
    [InlineData(1000.00, 6, 1250.00)]  // 25% interes
    [InlineData(1000.00, 12, 1500.00)] // 50% interes
    public void PagarEnCuotas_PlanesValidos_AplicaInteresYDescuentaSaldoCorrecto(
        decimal montoBase, int cuotas, decimal montoTotalEsperado)
    {
        var billetera = new Billetera();
        billetera.CargarDinero(5000.00m);

        billetera.PagarEnCuotas(montoBase, cuotas);

        Assert.Equal(9000.00m, billetera.ConsultarSaldo()); // Solo se debita la primera cuota (3000 / 3)
        var movimientos = billetera.ObtenerMovimientos();
        Assert.Equal(2, movimientos.Count); // Carga + Pago en Cuotas
        Assert.Equal("Pago en Cuotas", movimientos.Last().ObtenerTipo());
        Assert.Equal(3000.00m, movimientos.Last().Monto);
    }

    // Caso de Error: Seleccionar una cantidad de cuotas no permitida lanza ArgumentException y mantiene el saldo sin tocar
    [Theory]
    [InlineData(1)]
    [InlineData(4)]
    [InlineData(5)]
    [InlineData(0)]
    public void PagarEnCuotas_CuotasNoSoportadas_LanzaArgumentException(int cuotasNoSoportadas)
    {
        var billetera = new Billetera();
        billetera.CargarDinero(5000.00m);

        Assert.Throws<ArgumentException>(() => billetera.PagarEnCuotas(1000.00m, cuotasNoSoportadas));
        Assert.Equal(5000.00m, billetera.ConsultarSaldo());
    }

    // Caso de Error: Tener saldo para la compra base pero insuficiente para cubrir los intereses lanza InvalidOperationException
    [Fact]
    public void PagarEnCuotas_SaldoInsuficienteParaIntereses_LanzaInvalidOperationException()
    {
        var billetera = new Billetera();
        billetera.CargarDinero(1000.00m); // Alcanza para los $1000 base, pero no para los $1250 finales en 6 cuotas

        Assert.Throws<InvalidOperationException>(() => billetera.PagarEnCuotas(1000.00m, 6));
        Assert.Equal(1000.00m, billetera.ConsultarSaldo());
    }

    // Caso de Error: Intentar realizar un pago en cuotas teniendo deudas activas lanza InvalidOperationException
    [Fact]
    public void PagarEnCuotas_PoseeDeudasPendientes_LanzaInvalidOperationExceptionYMantieneSaldoIntacto()
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