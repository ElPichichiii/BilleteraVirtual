namespace BilleteraVirtual.Dominio;

public class Billetera
{
    private decimal saldo;

    public Billetera()
    {
        saldo = 0m;
    }

    public decimal ConsultarSaldo()
    {
        return saldo;
    }

    public void DepositarSaldo(decimal monto)
    {
        if (monto <= 0)

        {
            throw new ArgumentException("El monto a depositar debe ser mayor a 0");
        }

        saldo += monto;
    }

    public void RetirarSaldo(decimal monto)
    {
        if (monto <= 0) 
        {
            throw new ArgumentException("El monto a retirar debe ser mayor a 0");
        }

        if (monto > saldo)
        {
            throw new InvalidOperationException("El saldo es insuficiente para realizar el retiro");
        }
        saldo -= monto;
    }
}
