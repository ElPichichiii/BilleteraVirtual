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
}
