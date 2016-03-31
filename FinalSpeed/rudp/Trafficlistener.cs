using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FinalSpeed.rudp
{
    public interface Trafficlistener
    {

        public void trafficDownload(TrafficEvent tevent);


        public void trafficUpload(TrafficEvent tevent);

    }

}
