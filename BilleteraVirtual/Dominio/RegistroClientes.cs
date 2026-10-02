namespace BilleteraVirtual.Dominio;

public class RegistroClientes
{
    private readonly List<Cliente> clientes = new();

    public Cliente RegistrarCliente(string nombre, string dni, string email)
    {
        if (ExisteDni(dni))
        {
            throw new InvalidOperationException("Ya existe un cliente registrado con ese DNI.");
        }

        int id = clientes.Count + 1;
        var cliente = new Cliente(id, nombre, dni, email);
        clientes.Add(cliente);

        return cliente;
    }

    public Cliente BuscarPorDni(string dni)
    {
        var clienteEncontrado = clientes.FirstOrDefault(cliente => cliente.Dni == dni);

        if (clienteEncontrado is null)
        {
            throw new InvalidOperationException("No existe un cliente con ese DNI.");
        }

        return clienteEncontrado;
    }

    public bool ExisteDni(string dni)
    {
        return clientes.Any(cliente => cliente.Dni == dni);
    }
}
