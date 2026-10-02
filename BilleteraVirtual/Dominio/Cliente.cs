using System.Net.Mail;

namespace BilleteraVirtual.Dominio;

public class Cliente
{
    private readonly Billetera billetera;

    public int Id { get; }
    public string Nombre { get; }
    public string Dni { get; }
    public string Email { get; }

    public Cliente(int id, string nombre, string dni, string email)
    {
        ValidarDatoObligatorio(nombre, nameof(nombre));
        ValidarDatoObligatorio(dni, nameof(dni));
        ValidarDatoObligatorio(email, nameof(email));
        ValidarFormatoEmail(email);

        Id = id;
        Nombre = nombre;
        Dni = dni;
        Email = email;
        billetera = new Billetera();
    }

    public Billetera ObtenerBilletera()
    {
        return billetera;
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
        if (!MailAddress.TryCreate(email, out var direccion) ||
            direccion.Address != email)
        {
            throw new ArgumentException("El email tiene un formato inválido.", nameof(email));
        }
    }
}

