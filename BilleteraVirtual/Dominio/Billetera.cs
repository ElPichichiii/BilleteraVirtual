


namespace BilleteraVirtual.Dominio;

public class Billetera
{
    private decimal saldo;
    private HistorialMovimientos historial;
    private LimiteTransferencia limite;

    public string Cvu { get; }
    public string Alias { get; private set; }

    public Billetera(decimal limiteInicial = 500000m)
    {
        this.saldo = 0m;
        this.historial = new HistorialMovimientos();
        this.limite = new LimiteTransferencia(limiteInicial);

        // Inicialización de CVU y Alias
        this.Cvu = "0000" + Random.Shared.NextInt64(100000000000000000L, 999999999999999999L).ToString();
        this.Alias = $"cuenta.{this.Cvu[..8]}.mp";
    }

    public void CambiarAlias(string nuevoAlias)
    {
        if (string.IsNullOrWhiteSpace(nuevoAlias) || nuevoAlias.Length < 6 || nuevoAlias.Length > 40)
        {
            throw new ArgumentException("El Alias debe tener entre 6 y 40 caracteres.", nameof(nuevoAlias));
        }

        if (nuevoAlias.StartsWith(".") || nuevoAlias.EndsWith(".") || nuevoAlias.Contains(".."))
        {
            throw new ArgumentException("El Alias no puede empezar/terminar con punto ni tener puntos consecutivos.", nameof(nuevoAlias));
        }

        this.Alias = nuevoAlias.ToLower();

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


    public void RealizarTransferenciaExterna(decimal monto, string cbuDestino)
    {
        var transferenciaExterna = new TransferenciaExterna(this, monto, cbuDestino);
        transferenciaExterna.Ejecutar();
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

    public void TransferirPorIdentificador(string aliasOCvu, decimal monto, RepositorioBilleteras repositorio)
    {
        var destino = repositorio.BuscarPorIdentificador(aliasOCvu);
        this.Transferir(destino, monto);
    }
}