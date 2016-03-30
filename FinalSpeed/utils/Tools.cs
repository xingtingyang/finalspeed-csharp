using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.IO;
using System.Net.Cache;
using System.Security.Cryptography;

namespace FinalSpeed.utils
{
    public class Tools
    {
        //   public static HttpURLConnection getConnection(String urlString)
        //   {
        //    URL url = new URL(urlString);
        //    HttpURLConnection conn = null;
        //    if(urlString.startsWith("http://")){
        //        conn = (HttpURLConnection) url.openConnection();
        //    }else if(urlString.startsWith("https://")){
        //        HttpsURLConnection conns=(HttpsURLConnection)url.openConnection();
        //        conns.setHostnameVerifier(new HostnameVerifier() {
        //            public boolean verify(String hostname, SSLSession session) {
        //                return true;
        //            }
        //        });
        //        conn=conns;
        //    }
        //    if(conn!=null){
        //        conn.setConnectTimeout(10*1000);
        //        conn.setReadTimeout(10*1000);
        //        conn.setRequestMethod("POST");
        //        conn.setDoInput(true);
        //        conn.setDoOutput(true);
        //        conn.setUseCaches(false);
        //    }
        //    return conn;
        //}
        public static HttpWebRequest getConnection(string urlString)
        {
            HttpWebRequest request = null;
            if (urlString.StartsWith("https", StringComparison.OrdinalIgnoreCase))
            {
                ServicePointManager.ServerCertificateValidationCallback += (se, cert, chain, sslerror) =>
                {
                    return true;
                };
                request = WebRequest.Create(urlString) as HttpWebRequest;
                request.ProtocolVersion = HttpVersion.Version10;

            }
            else
            {
                request = WebRequest.Create(urlString) as HttpWebRequest;
            }
            if (request != null)
            {
                request.Timeout = 10 * 1000;
                request.ReadWriteTimeout = 10 * 1000;
                request.Method = "POST";
                request.CachePolicy = new RequestCachePolicy(RequestCacheLevel.NoCacheNoStore);
            }

            return request;

        }

        public static string getMD5(string strPwd)
        {
            return MD5Encrypt(strPwd);
        }

        public static string MD5Encrypt(string strPwd)
        {
            MD5 md5 = new MD5CryptoServiceProvider();
            byte[] data = System.Text.Encoding.UTF8.GetBytes(strPwd);//将字符编码为一个字节序列 
            byte[] md5data = md5.ComputeHash(data);//计算data字节数组的哈希值 
            md5.Clear();
            string str = "";
            for (int i = 0; i < md5data.Length; i++)
            {
                str += md5data[i].ToString("X2");
            }
            return str;
        }


        public static string MD5Encrypt(Stream inStream)
        {
            using (System.Security.Cryptography.MD5CryptoServiceProvider get_md5 = new System.Security.Cryptography.MD5CryptoServiceProvider())
            {
                inStream.Seek(0, SeekOrigin.Begin);
                byte[] hash_byte = get_md5.ComputeHash(inStream);
                string resule = System.BitConverter.ToString(hash_byte);
                resule = resule.Replace("-", "").ToLower();
                return resule;
            }
        }

        public static string getSizeStringKB(long size)
        {
            int gb = (int)(size / (1024 * 1024 * 1024));
            int gbl = (int)(size % (1024 * 1024 * 1024));
            int mb = gbl / (1024 * 1024);
            int mbl = gbl % (1024 * 1024);
            int kb = mbl / (1024);
            string ls = "";
            if (gb > 0)
            {
                ls += gb + ",";
            }
            if (mb > 0)
            {
                string mbs = "";
                if (gb > 0)
                {
                    if (mb < 10)
                    {
                        mbs += "00";
                    }
                    else if (mb < 100)
                    {
                        mbs += "0";
                    }
                }
                mbs += mb;
                ls += mbs + ",";
            }
            else
            {
                if (gb > 0)
                {
                    ls += "000,";
                }
            }

            if (kb > 0)
            {
                string kbs = "";
                if (gb > 0 | mb > 0)
                {
                    if (kb < 10)
                    {
                        kbs += "00";
                    }
                    else if (kb < 100)
                    {
                        kbs += "0";
                    }
                }
                kbs += kb;
                ls += kbs + " KB";
            }
            else
            {
                if (mb > 0 | gb > 0)
                {
                    ls += "000 KB";
                }
            }
            if (size == 0)
            {
                ls += 0 + " KB";
            }
            if (size < 1024)
            {
                //ls=size+" b";
                ls = 0 + " KB";
            }
            return ls;
        }
    }
}
