using DigitalBank.API.DB.Entities;
using DigitalBank.API.DTOs;
using DigitalBank.API.Repositories;
using DigitalBank.API.Requests.CreditCard;
using DigitalBank.API.Requests.Transaction;

namespace DigitalBank.API.Services;

public class CreditCardService(
    CreditCardRepository creditCardRepository,
    BalanceRepository balanceRepository,
    TransactionRepository transactionRepository)
{
    public async Task<bool> ExistsAsync(ExistsRequest request, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);

        return await creditCardRepository.ExistsAsync(
            request.CreditCardNumber,
            request.ExpirationDate,
            request.CVV,
            cancellationToken);
    }

    public async Task<BalanceDto> GetBalanceAsync(GetBalanceRequest request, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);

        var creditCardInfo = await creditCardRepository.GetAsync(
                                 request.CreditCardNumber,
                                 request.ExpirationDate,
                                 request.CVV,
                                 cancellationToken)
                             ?? throw new ArgumentNullException("Credit card not found");

        var balance = creditCardInfo.Balances?.FirstOrDefault(b => b.Currency == request.Currency)
                      ?? throw new ArgumentNullException("Balance not found");

        return new BalanceDto(balance.Amount);
    }

    public async Task<Guid> PayAsync(PayRequest request, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);

        var creditCardInfo = await creditCardRepository.GetAsync(
                                 request.CreditCardNumber,
                                 request.ExpirationDate,
                                 request.CVV,
                                 cancellationToken)
                             ?? throw new ArgumentNullException("Credit card not found");

        var balance = creditCardInfo.GetBalance(request.Currency)
                      ?? throw new ArgumentNullException("Balance not found");

        if (!balance.IsEnough(request.Amount))
        {
            throw new ArgumentNullException("Insufficient funds");
        }

        balance.DecreaseAmount(request.Amount);
        await balanceRepository.UpdateAsync(balance, cancellationToken);

        var transaction = Transaction.Create(request.Amount, balance.Id);
        await transactionRepository.AddAsync(transaction, cancellationToken);

        return transaction.Id;
    }

    public async Task<Guid> RefundAsync(RefundRequest request, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);

        var previousTransaction = await transactionRepository.FindAsync(request.TransactionId, cancellationToken)
                                  ?? throw new ArgumentNullException("Transaction not found");

        var balance = await balanceRepository.FindAsync<Balance>(previousTransaction.BalanceId, cancellationToken)
                      ?? throw new ArgumentNullException("Balance not found");

        balance.AddAmount(previousTransaction.Amount);
        await balanceRepository.UpdateAsync(balance, cancellationToken);

        previousTransaction.Rollback();
        await transactionRepository.UpdateAsync(previousTransaction, cancellationToken);

        var newTransaction = Transaction.Create(previousTransaction.Amount, balance.Id, previousTransaction.Id);
        await transactionRepository.AddAsync(newTransaction, cancellationToken);

        return newTransaction.Id;
    }
}
