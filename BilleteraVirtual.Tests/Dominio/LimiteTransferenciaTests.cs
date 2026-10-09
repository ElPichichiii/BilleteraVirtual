using System;
using BilleteraVirtual.Dominio;
using Xunit;

namespace BilleteraVirtual.Tests.Dominio;

public class LimiteTransferenciaTests
{
    // Caso Normal: Verificación de límite
    [Fact]
    public void EsValido_MontoMenorOIgualAlLimite_DevuelveTrue()
    {
        var limite = new LimiteTransferencia(100000m);

        Assert.True(limite.EsValido(50000m));
        Assert.True(limite.EsValido(100000m));
    }

    // Caso Borde: Monto por encima del límite
    [Fact]
    public void EsValido_MontoMayorAlLimite_DevuelveFalse()
    {
        var limite = new LimiteTransferencia(100000m);

        Assert.False(limite.EsValido(100001m));
    }

    // Caso de Error: Intentar actualizar límite con un valor menor o igual a cero
    [Theory]
    [InlineData(0)]
    [InlineData(-100)]
    public void ActualizarLimite_NuevoLimiteInvalido_LanzaArgumentException(decimal limiteInvalido)
    {
        var limite = new LimiteTransferencia(100000m);

        Assert.Throws<ArgumentException>(() => limite.ActualizarLimite(limiteInvalido));
    }
}