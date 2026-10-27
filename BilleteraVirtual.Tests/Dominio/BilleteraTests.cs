using System;
using BilleteraVirtual.Dominio;
using Xunit;

namespace BilleteraVirtual.Tests.Dominio;

public class BilleteraTests
{
    // Caso Normal: Cargar dinero incrementa saldo e historial
    [Fact]
    public void CargarDinero_MontoValido_AumentaSaldoYRegistraMovimiento()
    {
        var billetera = new Billetera();

        billetera.CargarDinero(1500m);

        Assert.Equal(1500m, billetera.ConsultarSaldo());
        Assert.Single(billetera.ObtenerMovimientos());
    }

    // Caso Borde: Consultas de validación de saldo y límite
    [Fact]
    public void Consultas_ValidarSaldoYLimite_DevuelveEstadoCorrecto()
    {
        var billetera = new Billetera(100000m);
        billetera.CargarDinero(5000m);

        Assert.True(billetera.TieneSaldoSuficiente(5000m));
        Assert.False(billetera.TieneSaldoSuficiente(5001m));
        Assert.True(billetera.MontoDentroDelLimite(100000m));
        Assert.False(billetera.MontoDentroDelLimite(100001m));
    }

    // Caso de Error: Transferir monto negativo o sin saldo suficiente
    [Fact]
    public void Transferir_SaldoInsuficiente_LanzaInvalidOperationExceptionYNoAlteraSaldo()
    {
        var origen = new Billetera();
        var destino = new Billetera();
        origen.CargarDinero(100m);

        Assert.Throws<InvalidOperationException>(() => origen.Transferir(destino, 500m));
        Assert.Equal(100m, origen.ConsultarSaldo());
        Assert.Equal(0m, destino.ConsultarSaldo());
    }
}
