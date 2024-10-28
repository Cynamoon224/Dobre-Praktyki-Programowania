using System;

namespace TDD_dpp
{
    class Program
    {
        static void Main(string[] args)
        {
            var mockGateway = new MockPaymentGateway();
            var processor = new PaymentProcessor(mockGateway);

            ProcessPayment(processor, "user123", 150.0);
            ProcessPayment(processor, "user456", 200.0);
            ProcessPaymentWithInvalidData(processor, "user789", -50.0);
            ProcessPaymentWithInvalidData(processor, "", 100.0);
            RefundPayment(processor, "txn1");
            RefundPayment(processor, "nonexistent_txn");
            CheckPaymentStatus(processor, "txn1");
            CheckPaymentStatus(processor, "nonexistent_txn");
        }

        static void ProcessPayment(PaymentProcessor processor, string userId, double amount)
        {
            try
            {
                var paymentResult = processor.ProcessPayment(userId, amount);
                Console.WriteLine($"Payment result: Success={paymentResult.Success}, Transaction ID={paymentResult.TransactionId}, Message={paymentResult.Message}");

                var status = processor.GetPaymentStatus(paymentResult.TransactionId);
                Console.WriteLine($"Payment status: {status}");
            }
            catch (Exception e)
            {
                Console.WriteLine($"An error occurred while processing payment: {e.Message}");
            }
        }

        static void ProcessPaymentWithInvalidData(PaymentProcessor processor, string userId, double amount)
        {
            try
            {
                var paymentResult = processor.ProcessPayment(userId, amount);
                Console.WriteLine($"Payment result: Success={paymentResult.Success}, Transaction ID={paymentResult.TransactionId}, Message={paymentResult.Message}");
            }
            catch (ArgumentException ae)
            {
                Console.WriteLine($"Invalid argument error: {ae.Message}");
            }
            catch (Exception e)
            {
                Console.WriteLine($"An error occurred while processing payment: {e.Message}");
            }
        }

        static void RefundPayment(PaymentProcessor processor, string transactionId)
        {
            try
            {
                var refundResult = processor.RefundPayment(transactionId);
                Console.WriteLine($"Refund result: Success={refundResult.Success}, Transaction ID={refundResult.TransactionId}, Message={refundResult.Message}");
            }
            catch (Exception e)
            {
                Console.WriteLine($"An error occurred while processing refund: {e.Message}");
            }
        }

        static void CheckPaymentStatus(PaymentProcessor processor, string transactionId)
        {
            try
            {
                var status = processor.GetPaymentStatus(transactionId);
                Console.WriteLine($"Payment status for Transaction ID={transactionId}: {status}");
            }
            catch (Exception e)
            {
                Console.WriteLine($"An error occurred while checking payment status: {e.Message}");
            }
        }
    }
}
