using System;
using System.Collections.Generic;
using System.Text;

namespace BilleteraVirtual.Dominio;

public class LimiteTransferencia
{
    private decimal montoMaximo;

    public LimiteTransferencia(decimal montoMaximoInicial = 500000m)
    {
        if (montoMaximoInicial <= 0)
        {
            throw new ArgumentException("El monto máximo inicial debe ser mayor a cero.", nameof(montoMaximoInicial));
        }

        this.montoMaximo = montoMaximoInicial;
    }

    public bool EsValido(decimal monto)
    {
        return monto > 0 && monto <= this.montoMaximo;
    }

    public void ActualizarLimite(decimal nuevoLimite)
    {
        if (nuevoLimite <= 0)
        {
            throw new ArgumentException("El nuevo límite debe ser mayor a cero.", nameof(nuevoLimite));
        }

        this.montoMaximo = nuevoLimite;
    }
}