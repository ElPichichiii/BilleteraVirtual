namespace BilleteraVirtual.Dominio;

public class RegistroClientes
{
    private List<Cliente> clientes;

    public RegistroClientes()
    {
        this.clientes = new List<Cliente>();
    }

    public Cliente RegistrarCliente(string nombre, string dni, string email)
    {
        if (this.ExisteDni(dni))
        {
            throw new InvalidOperationException($"Ya existe un cliente registrado con el DNI '{dni}'.");
        }

        int id = this.clientes.Count + 1;
        var nuevoCliente = new Cliente(id, nombre, dni, email);
        this.clientes.Add(nuevoCliente);

        return nuevoCliente;
    }

    public Cliente BuscarPorDni(string dni)
    {
        if (string.IsNullOrWhiteSpace(dni))
        {
            throw new ArgumentException("El DNI de búsqueda no puede estar vacío.", nameof(dni));
        }

        var clienteEncontrado = this.clientes.FirstOrDefault(
            cliente => cliente.Dni.Equals(dni, StringComparison.OrdinalIgnoreCase));

        if (clienteEncontrado is null)
        {
            throw new InvalidOperationException($"No se encontró ningún cliente registrado con el DNI '{dni}'.");
        }

        return clienteEncontrado;
    }

    public bool ExisteDni(string dni)
    {
        if (string.IsNullOrWhiteSpace(dni))
        {
            return false;
        }

        return this.clientes.Any(
            cliente => cliente.Dni.Equals(dni, StringComparison.OrdinalIgnoreCase));
    }
}
