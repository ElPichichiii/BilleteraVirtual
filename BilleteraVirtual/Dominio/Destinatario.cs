using System;
using System.Collections.Generic;
using System.Text;

namespace BilleteraVirtual.Dominio;

public class Destinatario
{
    private string alias;
    private Cliente cliente;

    public Destinatario(string alias, Cliente cliente)
    {
        if (string.IsNullOrWhiteSpace(alias))
        {
            throw new ArgumentException("El alias no puede estar vacío.", nameof(alias));
        }

        this.alias = alias;
        this.cliente = cliente ?? throw new ArgumentNullException(nameof(cliente), "El cliente asociado no puede ser nulo.");
    }

    public string ObtenerAlias()
    {
        return this.alias;
    }

    public string ObtenerNombre()
    {
        return this.cliente.Nombre;
    }

    public Billetera ObtenerBilletera()
    {
        return this.cliente.ObtenerBilletera();
    }
}