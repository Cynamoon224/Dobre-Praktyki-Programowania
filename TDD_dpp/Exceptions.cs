using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TDD_dpp
{

    public class NetworkException : Exception
    {
        public NetworkException(string message) : base(message) { }
    }

    public class PaymentException : Exception
    {
        public PaymentException(string message) : base(message) { }
    }

    public class RefundException : Exception
    {
        public RefundException(string message) : base(message) { }
    }

}
