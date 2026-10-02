using BilleteraVirtual.Dominio;

namespace BilleteraVirtual.Tests.Dominio;

public class ClienteTests
{
    [Fact]
    public void CrearCliente_ConDatosValidos_AsignaSusDatos()
    {
        var cliente = new Cliente(1, "Ana Pérez", "12345678", "ana@email.com");

        Assert.Equal(1, cliente.Id);
        Assert.Equal("Ana Pérez", cliente.Nombre);
        Assert.Equal("12345678", cliente.Dni);
        Assert.Equal("ana@email.com", cliente.Email);
    }

    [Fact]
    public void CrearCliente_ConDatosValidos_AsociaBilleteraConSaldoCero()
    {
        var cliente = new Cliente(1, "Ana Pérez", "12345678", "ana@email.com");

        var billetera = cliente.ObtenerBilletera();

        Assert.NotNull(billetera);
        Assert.Same(billetera, cliente.ObtenerBilletera());
        Assert.Equal(0m, billetera.ConsultarSaldo());
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void CrearCliente_SinNombre_LanzaArgumentException(string? nombre)
    {
        var excepcion = Assert.Throws<ArgumentException>(
            () => new Cliente(1, nombre!, "12345678", "ana@email.com"));

        Assert.Equal("nombre", excepcion.ParamName);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void CrearCliente_SinDni_LanzaArgumentException(string? dni)
    {
        var excepcion = Assert.Throws<ArgumentException>(
            () => new Cliente(1, "Ana Pérez", dni!, "ana@email.com"));

        Assert.Equal("dni", excepcion.ParamName);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void CrearCliente_SinEmail_LanzaArgumentException(string? email)
    {
        var excepcion = Assert.Throws<ArgumentException>(
            () => new Cliente(1, "Ana Pérez", "12345678", email!));

        Assert.Equal("email", excepcion.ParamName);
    }
}
