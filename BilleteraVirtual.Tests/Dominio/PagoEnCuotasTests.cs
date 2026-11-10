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

        Assert.Equal(5000.00m - montoTotalEsperado, billetera.ConsultarSaldo());
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
        billetera.CargarDinero(5000.00m);
        billetera.EstablecerDeudasPendientes(true);

        Assert.Throws<InvalidOperationException>(() => billetera.PagarEnCuotas(2000.00m, 3));
        Assert.Equal(5000.00m, billetera.ConsultarSaldo());
    }
}