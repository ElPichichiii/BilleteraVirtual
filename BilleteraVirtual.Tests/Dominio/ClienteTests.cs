using System;
using BilleteraVirtual.Dominio;
using Xunit;

namespace BilleteraVirtual.Tests.Dominio;

public class ClienteTests
{
    // Caso Normal: Creación válida de cliente y billetera asociada en $0
    [Fact]
    public void CrearCliente_ConDatosValidos_InicializaConBilleteraEnCero()
    {
        var cliente = new Cliente(1, "Ana Pérez", "12345678", "ana@email.com");

        Assert.Equal(1, cliente.Id);
        Assert.Equal("Ana Pérez", cliente.Nombre);
        Assert.Equal("12345678", cliente.Dni);
        Assert.Equal("ana@email.com", cliente.Email);
        Assert.NotNull(cliente.ObtenerBilletera());
        Assert.Equal(0m, cliente.ObtenerBilletera().ConsultarSaldo());
    }

    // Caso Borde: Agregar, buscar por alias (insensible a mayúsculas) y eliminar destinatario
    [Fact]
    public void GestionarDestinatarios_FlujoValido_AgregaBuscaYEliminaCorrectamente()
    {
        var clienteOrigen = new Cliente(1, "Ana", "12345678", "ana@email.com");
        var clienteDestino = new Cliente(2, "Carlos", "87654321", "carlos@email.com");

        clienteOrigen.AgregarDestinatario(clienteDestino, "Amigo");
        var destinatario = clienteOrigen.BuscarDestinatario("amigo");

        Assert.Equal("Amigo", destinatario.ObtenerAlias());
        Assert.Equal("Carlos", destinatario.ObtenerNombre());

        clienteOrigen.EliminarDestinatario("AMIGO");
        Assert.Throws<InvalidOperationException>(() => clienteOrigen.BuscarDestinatario("Amigo"));
    }

    // Caso de Error: Email con formato inválido o datos vacíos
    [Theory]
    [InlineData("email-invalido")]
    [InlineData("usuario@")]
    [InlineData("usuario@domain")]
    public void CrearCliente_EmailInvalido_LanzaArgumentException(string emailInvalido)
    {
        Assert.Throws<ArgumentException>(() => new Cliente(1, "Juan", "12345678", emailInvalido));
    }
}