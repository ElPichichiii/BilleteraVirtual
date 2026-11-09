using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using BilleteraVirtual.Dominio;

namespace BilleteraVirtual.Tests.Dominio;

public class RepositorioBilleterasTests
{
    [Fact]
    public void Agregar_BilleteraConAliasDuplicado_LanzaInvalidOperationException()
    {
        var repositorio = new RepositorioBilleteras();

        var billetera1 = new Billetera();
        billetera1.CambiarAlias("mi.alias.comun");

        var billetera2 = new Billetera();
        billetera2.CambiarAlias("mi.alias.comun"); // Mismo alias

        repositorio.Agregar(billetera1);

        // Debe lanzar excepción al intentar agregar otra billetera con el mismo alias
        Assert.Throws<InvalidOperationException>(() => repositorio.Agregar(billetera2));
    }

    [Fact]
    public void CambiarAlias_AUnAliasOcupadoPorOtraCuenta_LanzaInvalidOperationException()
    {
        var repositorio = new RepositorioBilleteras();

        var billetera1 = new Billetera();
        billetera1.CambiarAlias("alias.de.juan");

        var billetera2 = new Billetera();
        billetera2.CambiarAlias("alias.de.maria");

        repositorio.Agregar(billetera1);
        repositorio.Agregar(billetera2);

        // Intentamos cambiar el alias de María por el que ya usa Juan
        Assert.Throws<InvalidOperationException>(() =>
            repositorio.CambiarAlias(billetera2, "alias.de.juan")
        );
    }

    [Fact]
    public void CambiarAlias_AUnAliasLibre_SeActualizaCorrectamente()
    {
        var repositorio = new RepositorioBilleteras();

        var billetera = new Billetera();
        billetera.CambiarAlias("alias.inicial");

        repositorio.Agregar(billetera);

        // Cambiamos a uno que no está ocupado
        repositorio.CambiarAlias(billetera, "alias.nuevo.libre");

        Assert.Equal("alias.nuevo.libre", billetera.Alias);
    }
}
