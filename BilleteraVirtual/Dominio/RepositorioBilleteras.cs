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
        if (billetera == null) throw new ArgumentNullException(nameof(billetera));
        billeteras.Add(billetera);
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

