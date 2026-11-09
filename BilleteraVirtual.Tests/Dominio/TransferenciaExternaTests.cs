using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using BilleteraVirtual.Dominio;

namespace BilleteraVirtual.Tests.Dominio;

public class TransferenciaExternaTests
{
    [Fact]
    public void RealizarTransferenciaExterna_SaldoSuficiente_DescuentaSaldoYRegistraEnHistorial()
    {
        var billetera = new Billetera();
        billetera.CargarDinero(5000.00m); // Carga inicial para tener saldo

        string cbuDestino = "0123456789012345678901";
        billetera.RealizarTransferenciaExterna(2000.00m, cbuDestino);

        // Validar que se haya descontado correctamente el saldo
        Assert.Equal(3000.00m, billetera.ConsultarSaldo());

        // Validar que se haya registrado en el historial
        var movimientos = billetera.ObtenerMovimientos();
        Assert.Equal(2, movimientos.Count); // Carga + Transferencia Externa
        Assert.Equal("Transferencia Externa", movimientos.Last().ObtenerTipo());
        Assert.Equal(2000.00m, movimientos.Last().Monto);
    }

    [Fact]
    public void RealizarTransferenciaExterna_SaldoInsuficiente_LanzaInvalidOperationExceptionYMantieneSaldoIntacto()
    {
        var billetera = new Billetera();
        billetera.CargarDinero(1000.00m);

        string cbuDestino = "0123456789012345678901";

        // Intentamos transferir más de lo disponible
        Assert.Throws<InvalidOperationException>(() =>
            billetera.RealizarTransferenciaExterna(2500.00m, cbuDestino)
        );

        // El saldo debe quedar completamente intacto
        Assert.Equal(1000.00m, billetera.ConsultarSaldo());

        // No debe haberse registrado la transferencia fallida en el historial
        Assert.Single(billetera.ObtenerMovimientos());
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-500.00)]
    public void RealizarTransferenciaExterna_MontoInvalido_LanzaArgumentException(decimal montoInvalido)
    {
        var billetera = new Billetera();
        billetera.CargarDinero(3000.00m);

        string cbuDestino = "0123456789012345678901";

        Assert.Throws<ArgumentException>(() =>
            billetera.RealizarTransferenciaExterna(montoInvalido, cbuDestino)
        );

        // El saldo debe mantenerse intacto ante montos inválidos
        Assert.Equal(3000.00m, billetera.ConsultarSaldo());
    }
}