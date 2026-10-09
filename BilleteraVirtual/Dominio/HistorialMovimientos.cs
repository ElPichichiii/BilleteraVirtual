using System;
using System.Collections.Generic;
using System.Text;

namespace BilleteraVirtual.Dominio;

public class HistorialMovimientos
{
    private List<Movimiento> movimientos;

    public HistorialMovimientos()
    {
        this.movimientos = new List<Movimiento>();
    }

    public void AgregarMovimiento(Movimiento movimiento)
    {
        if (movimiento is null)
        {
            throw new ArgumentNullException(nameof(movimiento), "El movimiento no puede ser nulo.");
        }

        this.movimientos.Add(movimiento);
    }

    public List<Movimiento> ObtenerMovimientos()
    {
        return new List<Movimiento>(this.movimientos);
    }

    public List<Movimiento> OrdenarPorFechaDescendente()
    {
        return this.movimientos.OrderByDescending(m => m.Fecha).ToList();
    }


}