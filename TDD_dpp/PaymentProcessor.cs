using System;
using System.Collections.Generic;
using System.Text;

namespace TDD_dpp
{

    public class PaymentProcessor
    {
        private readonly IPaymentGateway _paymentGateway;

        public PaymentProcessor(IPaymentGateway paymentGateway)
        {
            _paymentGateway = paymentGateway;
        }

        public TransactionResult ProcessPayment(string userId, double amount)
        {
            if (amount <= 0)
                throw new ArgumentException("Amount must be positive.", nameof(amount));
            if (string.IsNullOrEmpty(userId))
                throw new ArgumentException("User ID cannot be empty.", nameof(userId));

            try
            {
                var result = _paymentGateway.Charge(userId, amount);
                return result;
            }
            catch (Exception ex)
            {
                return new TransactionResult(false, "", ex.Message);
            }
        }

        public TransactionResult RefundPayment(string transactionId)
        {
            if (string.IsNullOrEmpty(transactionId))
                throw new ArgumentException("Transaction ID cannot be empty.", nameof(transactionId));

            try
            {
                var result = _paymentGateway.Refund(transactionId);
                return result;
            }
            catch (Exception ex)
            {
                return new TransactionResult(false, "", ex.Message);
            }
        }

        public TransactionStatus GetPaymentStatus(string transactionId)
        {
            if (string.IsNullOrEmpty(transactionId))
                throw new ArgumentException("Transaction ID cannot be empty.", nameof(transactionId));

            return _paymentGateway.GetStatus(transactionId);
        }
    }

}
