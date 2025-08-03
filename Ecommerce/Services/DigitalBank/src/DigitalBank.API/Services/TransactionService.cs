using DigitalBank.API.DTOs;
using DigitalBank.API.Enums;
using DigitalBank.API.Repositories;
using DigitalBank.API.Requests.Transaction;

namespace DigitalBank.API.Services;

public class TransactionService(TransactionRepository transactionRepository)
{
    public async Task<TransactionDto> GetAsync(GetTransactionRequest request, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);

        var transaction = await transactionRepository.FindAsync(request.TransactionId, cancellationToken)
                          ?? throw new ArgumentNullException("Transaction not found");

        return new TransactionDto(
            transaction.Id,
            transaction.Amount,
            transaction.Balance?.CreditCard?.Number,
            transaction.Status,
            transaction.CreateDate);
    }

    public async Task<TransactionStatus> GetStatusAsync(
        GetTransactionStatusRequest request,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);

        var transaction = await transactionRepository.FindAsync(request.TransactionId, cancellationToken)
                          ?? throw new ArgumentNullException("Transaction not found");

        return transaction.Status;
    }
}
