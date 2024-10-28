using System;
using System.Collections.Generic;
using System.Text;

namespace TDD_dpp
{
    public enum TransactionStatus
    {
        PENDING,
        COMPLETED,
        FAILED
    }

    public class TransactionResult
    {
        public bool Success { get; }
        public string TransactionId { get; }
        public string Message { get; }

        public TransactionResult(bool success, string transactionId, string message = "")
        {
            Success = success;
            TransactionId = transactionId;
            Message = message;
        }
    }

    public interface IPaymentGateway
    {
        TransactionResult Charge(string userId, double amount);
        TransactionResult Refund(string transactionId);
        TransactionStatus GetStatus(string transactionId);
    }

}
