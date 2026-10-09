using System;
using BilleteraVirtual.Dominio;
using Xunit;

namespace BilleteraVirtual.Tests.Dominio;

public class DestinatarioTests
{
    // Caso Normal: Retorna el alias, nombre del cliente y su billetera
    [Fact]
    public void Propiedades_DestinatarioValido_DevuelveValoresCorrectos()
    {
        var clienteDestino = new Cliente(1, "Carlos Gómez", "87654321", "carlos@email.com");
        var destinatario = new Destinatario("Carlos Amigo", clienteDestino);

        Assert.Equal("Carlos Amigo", destinatario.ObtenerAlias());
        Assert.Equal("Carlos Gómez", destinatario.ObtenerNombre());
        Assert.Same(clienteDestino.ObtenerBilletera(), destinatario.ObtenerBilletera());
    }

    // Caso Borde / Error: Alias vacío o cliente nulo lanza excepción
    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void CrearDestinatario_AliasInvalido_LanzaArgumentException(string? aliasInvalido)
    {
        var cliente = new Cliente(1, "Pedro", "11223344", "pedro@email.com");

        Assert.Throws<ArgumentException>(() => new Destinatario(aliasInvalido!, cliente));
    }

    [Fact]
    public void CrearDestinatario_ClienteNulo_LanzaArgumentNullException()
    {
        Assert.Throws<ArgumentNullException>(() => new Destinatario("Alias", null!));
    }
}