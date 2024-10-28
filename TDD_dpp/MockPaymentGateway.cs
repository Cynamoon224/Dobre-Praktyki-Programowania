using System;
using System.Collections.Generic;
using System.Text;

namespace TDD_dpp
{
    public class MockPaymentGateway : IPaymentGateway
    {
        private int _transactionCounter = 0;
        private readonly Dictionary<string, (string userId, double amount)> _transactions = new();

        public TransactionResult Charge(string userId, double amount)
        {
            _transactionCounter++;
            var transactionId = $"txn{_transactionCounter}";
            _transactions[transactionId] = (userId, amount);
            return new TransactionResult(true, transactionId);
        }

        public TransactionResult Refund(string transactionId)
        {
            if (_transactions.Remove(transactionId))
                return new TransactionResult(true, transactionId);
            return new TransactionResult(false, transactionId, "Transaction not found.");
        }

        public TransactionStatus GetStatus(string transactionId)
        {
            return _transactions.ContainsKey(transactionId) ? TransactionStatus.COMPLETED : TransactionStatus.FAILED;
        }
    }

}
