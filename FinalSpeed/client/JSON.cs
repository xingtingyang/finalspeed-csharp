using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization.Json;
using System.Runtime.Serialization;
using System.IO;

namespace FinalSpeed.client
{
    /// <summary>
    /// 用于辅助生成JSON和解析JSON的类
    /// </summary>
    public static class JSON
    {
        public static T parse<T>(string jsonString)
        {
            using (var ms = new MemoryStream(Encoding.UTF8.GetBytes(jsonString)))
            {
                return (T)new DataContractJsonSerializer(typeof(T)).ReadObject(ms);
            }
        }

        public static string stringify(object jsonObject)
        {
            using (var ms = new MemoryStream())
            {
                new DataContractJsonSerializer(jsonObject.GetType()).WriteObject(ms, jsonObject);
                return Encoding.UTF8.GetString(ms.ToArray());
            }
        }
    }



    /// <summary>
    /// 辅助生成port_map.json的类
    /// </summary>
    [DataContract]
    public class MapListFile
    {
        [DataMember(Order = 0, IsRequired = true)]
        public List<MapPair> map_list { get; set; }

        public MapListFile()
        {
            map_list = new List<MapPair>();
        }
    }

    /// <summary>
    /// port_map.json里map_list的类
    /// </summary>
    [DataContract]
    public class MapPair
    {
        [DataMember(Order = 0, IsRequired = true)]
        public int dst_port { get; set; }

        [DataMember(Order = 1, IsRequired = true)]
        public int listen_port { get; set; }

        [DataMember(Order = 2, IsRequired = true)]
        public string name { get; set; }
    }
}
