using BilleteraVirtual.Dominio;

namespace BilleteraVirtual.Tests.Dominio;

public class RegistroClientesTests
{
    [Fact]
    public void ExisteDni_EnRegistroNuevo_DevuelveFalse()
    {
        var registro = new RegistroClientes();

        Assert.False(registro.ExisteDni("12345678"));
    }

    [Fact]
    public void RegistrarCliente_ConDatosValidos_PermiteBuscarloPorDni()
    {
        var registro = new RegistroClientes();

        var cliente = registro.RegistrarCliente(
            "Ana Pérez", "12345678", "ana@email.com");

        Assert.True(registro.ExisteDni("12345678"));
        Assert.Same(cliente, registro.BuscarPorDni("12345678"));
    }

    [Fact]
    public void RegistrarCliente_ConDatosValidos_CreaBilleteraConSaldoCero()
    {
        var registro = new RegistroClientes();

        var cliente = registro.RegistrarCliente(
            "Ana Pérez", "12345678", "ana@email.com");
        var billetera = cliente.ObtenerBilletera();

        Assert.NotNull(billetera);
        Assert.Equal(0m, billetera.ConsultarSaldo());
    }

    [Fact]
    public void RegistrarCliente_ConDniExistente_RechazaRegistroOriginalSeConserva()
    {
        var registro = new RegistroClientes();
        var clienteExistente = registro.RegistrarCliente(
            "Ana Pérez", "12345678", "ana@email.com");

        Assert.Throws<InvalidOperationException>(
            () => registro.RegistrarCliente(
                "Juan López", "12345678", "juan@email.com"));

        Assert.Same(clienteExistente, registro.BuscarPorDni("12345678"));
    }

    [Fact]
    public void BuscarPorDni_ConDniInexistente_LanzaInvalidOperationException()
    {
        var registro = new RegistroClientes();

        Assert.Throws<InvalidOperationException>(
            () => registro.BuscarPorDni("12345678"));
    }

    [Theory]
    [InlineData(null, "12345678", "ana@email.com")]
    [InlineData("", "12345678", "ana@email.com")]
    [InlineData("   ", "12345678", "ana@email.com")]
    [InlineData("Ana Pérez", null, "ana@email.com")]
    [InlineData("Ana Pérez", "", "ana@email.com")]
    [InlineData("Ana Pérez", "   ", "ana@email.com")]
    [InlineData("Ana Pérez", "12345678", null)]
    [InlineData("Ana Pérez", "12345678", "")]
    [InlineData("Ana Pérez", "12345678", "   ")]
    public void RegistrarCliente_SinDatoObligatorio_RechazaRegistro(
        string? nombre,
        string? dni,
        string? email)
    {
        var registro = new RegistroClientes();

        Assert.Throws<ArgumentException>(
            () => registro.RegistrarCliente(nombre!, dni!, email!));

        Assert.False(registro.ExisteDni("12345678"));
    }
}
