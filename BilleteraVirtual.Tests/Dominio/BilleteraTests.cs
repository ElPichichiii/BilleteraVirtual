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


    // Caso Normal: Asignación automática de CVU y Alias al instanciar la billetera
    [Fact]
    public void Constructor_AsignaCvuYAliasPorDefecto()
    {
        var billetera = new Billetera();

        Assert.Equal(22, billetera.Cvu.Length);
        Assert.NotNull(billetera.Alias);
    }

    // Caso Normal: Cambiar el Alias por uno válido actualiza la propiedad
    [Fact]
    public void CambiarAlias_AliasValido_ActualizaAlias()
    {
        var billetera = new Billetera();

        billetera.CambiarAlias("alias.valido.mp");

        Assert.Equal("alias.valido.mp", billetera.Alias);
    }

    // Caso de Error: Intentar asignar un Alias con formato inválido lanza ArgumentException
    [Theory]
    [InlineData("")]
    [InlineData("abc")] // Menos de 6 caracteres
    [InlineData(".alias.incorrecto")]
    [InlineData("alias..incorrecto")]
    public void CambiarAlias_AliasInvalido_LanzaArgumentException(string aliasInvalido)
    {
        var billetera = new Billetera();

        Assert.Throws<ArgumentException>(() => billetera.CambiarAlias(aliasInvalido));
    }


    // Caso Normal: Transferir por Alias o CVU a una cuenta existente
    [Fact]
    public void TransferirPorIdentificador_CuentaExistente_RealizaTransferencia()
    {
        var repo = new RepositorioBilleteras();
        var origen = new Billetera();
        var destino = new Billetera();
        
        repo.Agregar(origen);
        repo.Agregar(destino);
        origen.CargarDinero(2000m);

        origen.TransferirPorIdentificador(destino.Alias, 500m, repo);

        Assert.Equal(1500m, origen.ConsultarSaldo());
        Assert.Equal(500m, destino.ConsultarSaldo());
    }

    // Caso de Error: Buscar o transferir a un Alias/CVU inexistente lanza InvalidOperationException y no altera saldos
    [Fact]
    public void TransferirPorIdentificador_CuentaInexistente_LanzaInvalidOperationExceptionYNoAlteraSaldo()
    {
        var repo = new RepositorioBilleteras();
        var origen = new Billetera();
        repo.Agregar(origen);
        origen.CargarDinero(1000m);

        Assert.Throws<InvalidOperationException>(() => 
            origen.TransferirPorIdentificador("alias.inexistente.mp", 300m, repo));

        Assert.Equal(1000m, origen.ConsultarSaldo());
    }

    // Caso de Error: Identificador vacío o nulo lanza ArgumentException
    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void BuscarPorIdentificador_IdentificadorVacio_LanzaArgumentException(string identificadorInvalido)
    {
        var repo = new RepositorioBilleteras();

        Assert.Throws<ArgumentException>(() => repo.BuscarPorIdentificador(identificadorInvalido));
    }
}