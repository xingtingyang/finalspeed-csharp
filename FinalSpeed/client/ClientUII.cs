using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FinalSpeed.client
{
    public interface ClientUII
    {
        void setMessage(String message);

        void updateUISpeed(int connNum, int downSpeed, int upSpeed);

        bool login();

        bool updateNode(bool testSpeed);

        bool isOsx_fw_pf();

        bool isOsx_fw_ipfw();
    }
}
