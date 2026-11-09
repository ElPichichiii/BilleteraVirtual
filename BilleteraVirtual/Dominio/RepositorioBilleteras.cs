using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BilleteraVirtual.Dominio;

public class RepositorioBilleteras
{
    private readonly List<Billetera> billeteras = new();

    public void Agregar(Billetera billetera)
    {
        if (billetera == null)
        {
            throw new ArgumentNullException(nameof(billetera), "La billetera no puede ser nula.");
        }

        
        if (this.billeteras.Any(b => b.Cvu.Equals(billetera.Cvu, StringComparison.OrdinalIgnoreCase)))
        {
            throw new InvalidOperationException($"Ya existe una billetera registrada con el CVU '{billetera.Cvu}'.");
        }

        if (this.billeteras.Any(b => b.Alias.Equals(billetera.Alias, StringComparison.OrdinalIgnoreCase)))
        {
            throw new InvalidOperationException($"Ya existe una billetera registrada con el Alias '{billetera.Alias}'.");
        }

        this.billeteras.Add(billetera);
    }

    public void CambiarAlias(Billetera billetera, string nuevoAlias)
    {
        if (billetera == null)
        {
            throw new ArgumentNullException(nameof(billetera), "La billetera no puede ser nula.");
        }

        
        bool aliasOcupado = this.billeteras
            .Where(b => b != billetera) 
            .Any(b => b.Alias.Equals(nuevoAlias, StringComparison.OrdinalIgnoreCase));

        if (aliasOcupado)
        {
            throw new InvalidOperationException($"El Alias '{nuevoAlias}' ya se encuentra registrado por otra cuenta.");
        }

       
        billetera.CambiarAlias(nuevoAlias);
    }


    public Billetera BuscarPorIdentificador(string aliasOCvu)
    {
        if (string.IsNullOrWhiteSpace(aliasOCvu))
        {
            throw new ArgumentException("El Alias o CVU no puede estar vacío.", nameof(aliasOCvu));
        }

        var cuentaEncontrada = billeteras.FirstOrDefault(b =>
            b.Cvu.Equals(aliasOCvu, StringComparison.OrdinalIgnoreCase) ||
            b.Alias.Equals(aliasOCvu, StringComparison.OrdinalIgnoreCase));

        if (cuentaEncontrada == null)
        {
            throw new InvalidOperationException($"No existe ninguna cuenta asociada al identificador '{aliasOCvu}'.");
        }

        return cuentaEncontrada;
    }
}

