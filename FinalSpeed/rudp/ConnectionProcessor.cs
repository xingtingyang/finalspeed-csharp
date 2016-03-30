using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FinalSpeed.rudp
{
    public interface ConnectionProcessor
    {
        public abstract void process(ConnectionUDP conn);
    }

}
