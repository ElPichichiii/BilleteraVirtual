using System;
using System.Collections.Generic;
using System.Text;

namespace BilleteraVirtual.Dominio;

public abstract class Movimiento
{
    public int Id { get; protected set; }
    public decimal Monto { get; protected set; }
    public DateTime Fecha { get; protected set; }

    protected Movimiento(decimal monto)
    {
        this.Monto = monto;
        this.Fecha = DateTime.Now;
    }

    // Template Method
    public void Ejecutar()
    {
        this.Validar();
        this.Aplicar();
        this.Registrar();
    }

    protected abstract void Validar();
    protected abstract void Aplicar();
    protected abstract void Registrar();
    public abstract string ObtenerTipo();
}