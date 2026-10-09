


namespace BilleteraVirtual.Dominio;

public class Billetera
{
    private decimal saldo;
    private HistorialMovimientos historial;
    private LimiteTransferencia limite;

    public Billetera(decimal limiteInicial = 500000m)
    {
        this.saldo = 0m;
        this.historial = new HistorialMovimientos();
        this.limite = new LimiteTransferencia(limiteInicial);
    }

    // Consultas públicas 
    public decimal ConsultarSaldo()
    {
        return this.saldo;
    }

    public List<Movimiento> ObtenerMovimientos()
    {
        return this.historial.ObtenerMovimientos();
    }

    public bool TieneSaldoSuficiente(decimal monto)
    {
        return this.saldo >= monto;
    }

    public bool MontoDentroDelLimite(decimal monto)
    {
        return this.limite.EsValido(monto);
    }

    //Operaciones públicas (Delegan en los objetos de Movimiento)
    public void CargarDinero(decimal monto)
    {
        var carga = new Carga(this, monto);
        carga.Ejecutar();
    }

    public void Transferir(Billetera destino, decimal monto)
    {
        var transferencia = new Transferencia(this, destino, monto);
        transferencia.Ejecutar();
    }

    // Métodos internos (Acceso restringido al ensamblado de Dominio) 
    internal void Acreditar(decimal monto)
    {
        this.saldo += monto;
    }

    internal void Debitar(decimal monto)
    {
        this.saldo -= monto;
    }

    internal void RegistrarMovimiento(Movimiento movimiento)
    {
        this.historial.AgregarMovimiento(movimiento);
    }
}