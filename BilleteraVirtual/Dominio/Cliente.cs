using System.Net.Mail;

namespace BilleteraVirtual.Dominio;

public class Cliente
{
    private int id;
    private string nombre;
    private string dni;
    private string email;
    private Billetera billetera;
    private List<Destinatario> destinatarios;

    public int Id => this.id;
    public string Nombre => this.nombre;
    public string Dni => this.dni;
    public string Email => this.email;

    public Cliente(int id, string nombre, string dni, string email)
    {
        ValidarDatoObligatorio(nombre, nameof(nombre));
        ValidarDatoObligatorio(dni, nameof(dni));
        ValidarDatoObligatorio(email, nameof(email));
        ValidarFormatoEmail(email);

        this.id = id;
        this.nombre = nombre;
        this.dni = dni;
        this.email = email;
        this.billetera = new Billetera();
        this.destinatarios = new List<Destinatario>();
    }

    public Billetera ObtenerBilletera()
    {
        return this.billetera;
    }

    public void AgregarDestinatario(Cliente cliente, string alias)
    {
        if (cliente is null)
        {
            throw new ArgumentNullException(nameof(cliente), "El cliente a agregar no puede ser nulo.");
        }

        if (cliente == this)
        {
            throw new InvalidOperationException("No podés agregarte a vos mismo como destinatario.");
        }

        if (string.IsNullOrWhiteSpace(alias))
        {
            throw new ArgumentException("El alias no puede estar vacío.", nameof(alias));
        }

        if (this.destinatarios.Any(d => d.ObtenerAlias().Equals(alias, StringComparison.OrdinalIgnoreCase)))
        {
            throw new InvalidOperationException("Ya existe un destinatario guardado con ese alias.");
        }

        this.destinatarios.Add(new Destinatario(alias, cliente));
    }

    public void EliminarDestinatario(string alias)
    {
        var destinatario = this.BuscarDestinatario(alias);
        this.destinatarios.Remove(destinatario);
    }

    public Destinatario BuscarDestinatario(string alias)
    {
        if (string.IsNullOrWhiteSpace(alias))
        {
            throw new ArgumentException("El alias de búsqueda no puede estar vacío.", nameof(alias));
        }

        var destinatario = this.destinatarios.FirstOrDefault(
            d => d.ObtenerAlias().Equals(alias, StringComparison.OrdinalIgnoreCase));

        if (destinatario is null)
        {
            throw new InvalidOperationException($"No se encontró ningún destinatario con el alias '{alias}'.");
        }

        return destinatario;
    }

    private static void ValidarDatoObligatorio(string valor, string nombreParametro)
    {
        if (string.IsNullOrWhiteSpace(valor))
        {
            throw new ArgumentException("El dato es obligatorio.", nombreParametro);
        }
    }

    private static void ValidarFormatoEmail(string email)
    {
        if (!MailAddress.TryCreate(email, out var direccion) || direccion.Address != email || !direccion.Host.Contains("."))
        {
            throw new ArgumentException("El email tiene un formato inválido.", nameof(email));
        }
    }
}