using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Net.Sockets;
using FinalSpeed.utils;
using System.Runtime.Serialization;

namespace FinalSpeed.client
{
   public class PortMapManager
    {
       	MapClient mapClient;
	
	List<MapRule> mapList=new List<MapRule>();
	
	Dictionary<int, MapRule> mapRuleTable=new Dictionary<int, MapRule>();
	
	string configFilePath="port_map.json";
	
public	PortMapManager(MapClient mapClient){
		this.mapClient=mapClient;
		//listenPort();
		loadMapRule();
	}
	
	void addMapRule(MapRule mapRule){
		if(getMapRule(mapRule.name)!=null){
			throw new Exception("映射 "+mapRule.name+" 已存在,请修改名称!");
		}
		Socket serverSocket=null;
		try {
			serverSocket=new Socket(AddressFamily.InterNetwork,SocketType.Stream,ProtocolType.Tcp);//serverSocket = new Socket(mapRule.getListen_port());c#不这样创建socket
            serverSocket.Connect(IPAddress.Parse("127.0.0.1"),mapRule.getListen_port());
			listen(serverSocket);
			mapList.Add(mapRule);
			mapRuleTable.Add(mapRule.listen_port, mapRule);
			saveMapRule();
		} catch (Exception e2) {
			//e2.printStackTrace();
			throw new Exception("端口 "+mapRule.listen_port+" 已经被占用!");
		}finally{
//			if(serverSocket!=null){
//				serverSocket.close();
//			}
		}
	}
	
	void removeMapRule(String name){
		MapRule mapRule=getMapRule(name);
		if(mapRule!=null){
			mapList.Remove(mapRule);
			mapRuleTable.Remove(mapRule.listen_port);
			if(mapRule.serverSocket!=null){
				try {
					mapRule.serverSocket.Close();
				} catch (Exception e) {
					MLog.info(e.Message);
				}
			}
			try {
				saveMapRule();
			} catch (Exception e) {
				MLog.info(e.Message);
			}
		}
	}
	
	void updateMapRule(MapRule mapRule_origin,MapRule mapRule_new){
		if(getMapRule(mapRule_new.name)!=null&&!mapRule_origin.name.Equals(mapRule_new.name)){
			throw new Exception("映射 "+mapRule_new.name+" 已存在,请修改名称!");
		}
		Socket serverSocket=null;
		if(mapRule_origin.listen_port!=mapRule_new.listen_port){
			try {
				//serverSocket = new ServerSocket(mapRule_new.getListen_port());
                serverSocket=new Socket(AddressFamily.InterNetwork,SocketType.Stream,ProtocolType.Tcp);
                serverSocket.Connect(IPAddress.Parse("127.0.0.1"),mapRule_new.getListen_port());
				listen(serverSocket);
				mapRule_origin.isUsing=false;
				if(mapRule_origin.serverSocket!=null){
					mapRule_origin.serverSocket.Close();
				}
				mapRule_origin.serverSocket=serverSocket;
				mapRuleTable.Remove(mapRule_origin.listen_port);
				mapRuleTable.Add(mapRule_new.listen_port, mapRule_new);
			} catch (Exception e2) {
				//e2.printStackTrace();
				throw new Exception("端口 "+mapRule_new.getListen_port()+" 已经被占用!");
			}finally{
//				if(serverSocket!=null){
//					serverSocket.close();
//				}
			}
		}
		mapRule_origin.name=mapRule_new.name;
		mapRule_origin.listen_port=mapRule_new.listen_port;
		mapRule_origin.dst_port=mapRule_new.dst_port;
		saveMapRule();
		
	}
	
	void saveMapRule() {

		JSONObject json=new JSONObject();
		JSONArray json_map_list=new JSONArray();
		json.put("map_list", json_map_list);
		if(mapList.Count==0){

		}
		for(MapRule r:mapList){
			JSONObject json_rule=new JSONObject();
			json_rule.put("name", r.name);
			json_rule.put("listen_port", r.listen_port);
			json_rule.put("dst_port", r.dst_port);
			json_map_list.add(json_rule);
		}
		try {
			saveFile(json.toJSONString().getBytes("utf-8"), configFilePath);
		} catch (UnsupportedEncodingException e) {
			e.printStackTrace();
		} catch (Exception e) {
			e.printStackTrace();
			throw new Exception("保存失败!");
		}
	}
	
	void loadMapRule(){
		String content;
		JSONObject json=null;
		try {
			content = readFileUtf8(configFilePath);
			json=JSONObject.parseObject(content);
		} catch (Exception e) {
			//e.printStackTrace();
		}
		if(json!=null&&json.containsKey("map_list")){
			JSONArray json_map_list=json.getJSONArray("map_list");
			for(int i=0;i<json_map_list.size();i++){
				JSONObject json_rule=(JSONObject) json_map_list.get(i);
				MapRule mapRule=new MapRule();
				mapRule.name=json_rule.getString("name");
				mapRule.listen_port=json_rule.getIntValue("listen_port");
				mapRule.dst_port=json_rule.getIntValue("dst_port");
				mapList.add(mapRule);
				ServerSocket serverSocket;
				try {
					serverSocket = new ServerSocket(mapRule.getListen_port());
					listen(serverSocket);
					mapRule.serverSocket=serverSocket;
				} catch (IOException e) {
					mapRule.using=true;
					e.printStackTrace();
				}
				mapRuleTable.Add(mapRule.listen_port, mapRule);
			}
		}

	}
	
	MapRule getMapRule(string name){
		MapRule rule=null;
		foreach(MapRule r in mapList){
			if(r.getName().Equals(name)){
				rule=r;
				break;
			}
		}
		return rule;
	}
	
	public List<MapRule> getMapList() {
		return mapList;
	}

	public void setMapList(List<MapRule> mapList) {
		this.mapList = mapList;
	}

	void listen(Socket serverSocket){
		Route.es.execute(new Runnable() {

			@Override
			public void run() {
				while(true){
					try {
						final Socket socket=serverSocket.accept();
						Route.es.execute(new Runnable() {
							
							@Override
							public void run() {
								int listenPort=serverSocket.getLocalPort();
								MapRule mapRule=mapRuleTable.get(listenPort);
								if(mapRule!=null){
									Route route=null;
									if(mapClient.isUseTcp()){
										route=mapClient.route_tcp;
									}else {
										route=mapClient.route_udp;
									}
									PortMapProcess process=new PortMapProcess(mapClient,route, socket,mapClient.serverAddress,mapClient.serverPort,null, 
											null,mapRule.dst_port);
								}
							}
							
						});

					} catch (IOException e) {
						e.printStackTrace();
						break;
					}
				}
			}
		});
	}
	
	void saveFile(byte[] data,String path) throws Exception{
		FileOutputStream fos=null;
		try {
			fos=new FileOutputStream(path);
			fos.write(data);
		} catch (Exception e) {
			throw e;
		} finally {
			if(fos!=null){
				fos.close();
			}
		}
	}
	
	public static string readFileUtf8(String path) throws Exception{
		String str=null;
		FileInputStream fis=null;
		DataInputStream dis=null;
		try {
			File file=new File(path);

			int length=(int) file.length();
			byte[] data=new byte[length];

			fis=new FileInputStream(file);
			dis=new DataInputStream(fis);
			dis.readFully(data);
			str=new String(data,"utf-8");

		} catch (Exception e) {
			//e.printStackTrace();
			throw e;
		}finally{
			if(fis!=null){
				try {
					fis.close();
				} catch (IOException e) {
					e.printStackTrace();
				}
			}
			if(dis!=null){
				try {
					dis.close();
				} catch (IOException e) {
					e.printStackTrace();
				}
			}
		}

		return str;
	}
    }
}
