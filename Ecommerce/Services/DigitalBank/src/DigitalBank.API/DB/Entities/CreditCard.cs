using DigitalBank.API.Enums;

namespace DigitalBank.API.DB.Entities;

public class CreditCard
{
    public int Id { get; init; }

    public string Number { get; private set; }

    public string ExpirationDate { get; private set; }

    public string CVV { get; private set; }

    public virtual ICollection<Balance> Balances { get; private set; } = [];

    public Balance GetBalance(CurrencyType currency)
    {
        return Balances?.FirstOrDefault(b => b.Currency == currency);
    }
}
