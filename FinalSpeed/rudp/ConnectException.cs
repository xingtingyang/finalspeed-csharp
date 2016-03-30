using FinalSpeed.utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FinalSpeed.rudp
{
    public class ConnectException : Exception
    {
        private const long serialVersionUID = 8735513900170495107L;
        public new string Message;
        private string p;
        public ConnectException(string message)
        {
            this.Message = message;
        }
    }
}
