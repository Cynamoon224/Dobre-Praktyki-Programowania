using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using Moq;

namespace TDD_dpp
{
    [TestFixture]
    public class PaymentProcessorTests
    {
        private Mock<IPaymentGateway> _mockGateway;
        private PaymentProcessor _processor;

        [SetUp]
        public void SetUp()
        {
            _mockGateway = new Mock<IPaymentGateway>();
            _processor = new PaymentProcessor(_mockGateway.Object);
        }

        [Test]
        public void ProcessPayment_Success_ReturnsTransactionResult()
        {
            var userId = "user123";
            var amount = 100.0;
            var transactionId = "txn1";
            _mockGateway.Setup(gateway => gateway.Charge(userId, amount))
                        .Returns(new TransactionResult(true, transactionId));

            var result = _processor.ProcessPayment(userId, amount);

            Assert.IsTrue(result.Success);
            Assert.AreEqual(transactionId, result.TransactionId);
            _mockGateway.Verify(gateway => gateway.Charge(userId, amount), Times.Once);
        }

        [Test]
        public void ProcessPayment_InsufficientFunds_ReturnsFailure()
        {
            var userId = "user123";
            var amount = 100.0;
            _mockGateway.Setup(gateway => gateway.Charge(userId, amount))
                        .Returns(new TransactionResult(false, "", "Insufficient funds"));

            var result = _processor.ProcessPayment(userId, amount);

            Assert.IsFalse(result.Success);
            Assert.AreEqual("Insufficient funds", result.Message);
        }

        [Test]
        public void ProcessPayment_NegativeAmount_ThrowsArgumentException()
        {
            Assert.Throws<ArgumentException>(() => _processor.ProcessPayment("user123", -50.0));
        }

        [Test]
        public void ProcessPayment_EmptyUserId_ThrowsArgumentException()
        {
            Assert.Throws<ArgumentException>(() => _processor.ProcessPayment("", 100.0));
        }

        [Test]
        public void ProcessPayment_NetworkException_HandlesException()
        {
            var userId = "user123";
            var amount = 100.0;
            _mockGateway.Setup(gateway => gateway.Charge(userId, amount))
                        .Throws(new NetworkException("Network error"));

            var result = _processor.ProcessPayment(userId, amount);

            Assert.IsFalse(result.Success);
            Assert.AreEqual("Network error", result.Message);
        }

        [Test]
        public void RefundPayment_Success_ReturnsTransactionResult()
        {
            var transactionId = "txn1";
            _mockGateway.Setup(gateway => gateway.Refund(transactionId))
                        .Returns(new TransactionResult(true, transactionId));

            var result = _processor.RefundPayment(transactionId);

            Assert.IsTrue(result.Success);
            Assert.AreEqual(transactionId, result.TransactionId);
            _mockGateway.Verify(gateway => gateway.Refund(transactionId), Times.Once);
        }


        [Test]
        public void RefundPayment_NetworkException_HandlesException()
        {
            var transactionId = "txn1";
            _mockGateway.Setup(gateway => gateway.Refund(transactionId))
                        .Throws(new RefundException("Refund error"));

            var result = _processor.RefundPayment(transactionId);

            Assert.IsFalse(result.Success);
            Assert.AreEqual("Refund error", result.Message);
        }

        [Test]
        public void GetPaymentStatus_Success_ReturnsCompleted()
        {
            var transactionId = "txn1";
            _mockGateway.Setup(gateway => gateway.GetStatus(transactionId))
                        .Returns(TransactionStatus.COMPLETED);

            var status = _processor.GetPaymentStatus(transactionId);

            Assert.AreEqual(TransactionStatus.COMPLETED, status);
            _mockGateway.Verify(gateway => gateway.GetStatus(transactionId), Times.Once);
        }


    }


}
