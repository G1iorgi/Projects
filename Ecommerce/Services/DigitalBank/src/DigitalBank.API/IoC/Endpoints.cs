using System.ComponentModel.DataAnnotations;
using DigitalBank.API.Requests.CreditCard;
using DigitalBank.API.Requests.Transaction;
using DigitalBank.API.Services;
using Microsoft.AspNetCore.Mvc;
using SharpGrip.FluentValidation.AutoValidation.Endpoints.Extensions;

namespace DigitalBank.API.IoC;

public static class Endpoints
{
    public static void AddCreditCardEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app
            .MapGroup("api/credit-cards")
            .WithTags("Credit Cards")
            .AddFluentValidationAutoValidation();

        group.MapPost(
            "/exists",
            async (
                [Required] [FromBody] ExistsRequest request,
                CreditCardService creditCardService,
                CancellationToken cancellationToken = default) =>
            {
                var result = await creditCardService.ExistsAsync(request, cancellationToken);

                return Results.Ok(result);
            });

        group.MapPost(
            "/get-balance",
            async (
                [Required] [FromBody] GetBalanceRequest request,
                CreditCardService creditCardService,
                CancellationToken cancellationToken = default) =>
            {
                var result = await creditCardService.GetBalanceAsync(request, cancellationToken);

                return Results.Ok(result);
            });

        group.MapPost(
            "/pay",
            async (
                [Required] [FromBody] PayRequest request,
                CreditCardService creditCardService,
                CancellationToken cancellationToken = default) =>
            {
                var response = await creditCardService.PayAsync(request, cancellationToken);

                return Results.Ok(response);
            });
    }

    public static void AddTransactionEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app
            .MapGroup("api/transactions")
            .WithTags("Transactions")
            .AddFluentValidationAutoValidation();

        group.MapGet(
            "{transactionId}",
            async (
                [Required] [FromBody] GetTransactionRequest request,
                TransactionService transactionService,
                CancellationToken cancellationToken = default) =>
            {
                var response = await transactionService.GetAsync(request, cancellationToken);

                return Results.Ok(response);
            });

        group.MapGet(
            "{transactionId}/status",
            async (
                [Required] [FromBody] GetTransactionStatusRequest request,
                TransactionService transactionService,
                CancellationToken cancellationToken = default) =>
            {
                var response = await transactionService.GetStatusAsync(request, cancellationToken);

                return Results.Ok(response);
            });

        group.MapPost(
            "{transactionId}/refund",
            async (
                [Required] [FromBody] RefundRequest request,
                CreditCardService creditCardService,
                CancellationToken cancellationToken = default) =>
            {
                var response = await creditCardService.RefundAsync(request, cancellationToken);

                return Results.Ok(response);
            });
    }
}
