using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Runtime.Serialization;

namespace FinalSpeed.client
{
    [Serializable]
    public class MapRule
    {
        public int listen_port;

        public int dst_port;

        public string name;

        /// <summary>
        /// 原名叫做using，和C#关键词冲突了
        /// </summary>
        public bool isUsing = false;

        /// <summary>
        /// 原JAVA代码是ServerSocket类
        /// </summary>
        public Socket serverSocket;

        /// <summary>
        /// 可以直接用listen_port属性
        /// </summary>
        /// <returns></returns>
        [Obsolete]
        public int getListen_port()
        {
            return listen_port;
        }

        public void setListen_port(int listen_port)
        {
            this.listen_port = listen_port;
        }

        public int getDst_port()
        {
            return dst_port;
        }

        public void setDst_port(int dst_port)
        {
            this.dst_port = dst_port;
        }

        public String getName()
        {
            return name;
        }

        public void setName(String name)
        {
            this.name = name;
        }
    }
}
