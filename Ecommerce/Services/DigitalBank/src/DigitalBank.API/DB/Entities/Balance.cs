using DigitalBank.API.Enums;

namespace DigitalBank.API.DB.Entities;

public class Balance
{
    public int Id { get; init; }

    public CurrencyType Currency { get; private set; }

    public decimal Amount { get; private set; }

    public int CreditCardId { get; private set; }

    public virtual CreditCard CreditCard { get; private set; }

    public bool IsEnough(decimal amount)
    {
        return Amount >= amount;
    }

    public void DecreaseAmount(decimal amount)
    {
        Amount -= amount;
    }

    public void AddAmount(decimal amount)
    {
        Amount += amount;
    }
}
