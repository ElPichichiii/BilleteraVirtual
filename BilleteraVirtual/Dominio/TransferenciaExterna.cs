using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
    

namespace BilleteraVirtual.Dominio;

public class TransferenciaExterna : Movimiento
{
    private Billetera billetera;
    private string cbuDestino;

    public string CbuDestino => this.cbuDestino;

    public TransferenciaExterna(Billetera billetera, decimal monto, string cbuDestino) : base(monto)
    {
        this.billetera = billetera ?? throw new ArgumentNullException(nameof(billetera), "La billetera no puede ser nula.");

        if (string.IsNullOrWhiteSpace(cbuDestino))
        {
            throw new ArgumentException("El CBU de destino no puede estar vacío.", nameof(cbuDestino));
        }

        this.cbuDestino = cbuDestino;
    }

    protected override void Validar()
    {
        if (this.Monto <= 0)
        {
            throw new ArgumentException("El monto a transferir debe ser mayor a cero.", nameof(this.Monto));
        }

        if (!this.billetera.TieneSaldoSuficiente(this.Monto))
        {
            throw new InvalidOperationException("La billetera no posee saldo suficiente para realizar la transferencia externa.");
        }
    }

    protected override void Aplicar()
    {
        // Debita de la billetera origen de forma segura
        this.billetera.Debitar(this.Monto);
    }

    protected override void Registrar()
    {
        // Registra el movimiento en el historial de la billetera
        this.billetera.RegistrarMovimiento(this);
    }

    public override string ObtenerTipo() => "Transferencia Externa";
}