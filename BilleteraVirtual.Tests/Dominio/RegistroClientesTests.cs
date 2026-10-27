using System;
using BilleteraVirtual.Dominio;
using Xunit;

namespace BilleteraVirtual.Tests.Dominio;

public class RegistroClientesTests
{
    // Caso Normal: Registrar cliente de forma exitosa
    [Fact]
    public void RegistrarCliente_DatosValidos_GuardaClienteYSePuedeBuscarPorDni()
    {
        var registro = new RegistroClientes();

        var cliente = registro.RegistrarCliente("Juan Pérez", "12345678", "juan@email.com");

        Assert.NotNull(cliente);
        Assert.True(registro.ExisteDni("12345678"));
        Assert.Same(cliente, registro.BuscarPorDni("12345678"));
    }

    // Caso Borde: ExisteDni devuelve false para valores vacíos o no registrados
    [Theory]
    [InlineData("99999999")]
    [InlineData("")]
    [InlineData(null)]
    public void ExisteDni_DniNoRegistrado_DevuelveFalse(string? dniInexistente)
    {
        var registro = new RegistroClientes();
        registro.RegistrarCliente("Ana", "12345678", "ana@email.com");

        Assert.False(registro.ExisteDni(dniInexistente!));
    }

    // Caso de Error: Intento de registrar DNI duplicado
    [Fact]
    public void RegistrarCliente_DniDuplicado_LanzaInvalidOperationException()
    {
        var registro = new RegistroClientes();
        registro.RegistrarCliente("Ana", "12345678", "ana@email.com");

        Assert.Throws<InvalidOperationException>(
            () => registro.RegistrarCliente("Pedro", "12345678", "pedro@email.com"));
    }
}