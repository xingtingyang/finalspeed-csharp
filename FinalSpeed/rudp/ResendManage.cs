using FinalSpeed.utils;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace FinalSpeed.rudp
{
    public class ResendManage
    {
        	bool haveTask=false;
	Object signalOb=new Object();
	Thread mainThread;
	long vTime=0;
	long lastReSendTime;
	
	ConcurrentQueue<ResendItem> taskList=new ConcurrentQueue<ResendItem>();
	
	public ResendManage(){
		Route.es.execute(this);
	}
	
	public void addTask(ConnectionUDP conn,int sequence)
    {
		ResendItem ri=new ResendItem(conn, sequence);
		ri.setResendTime(getNewResendTime(conn));
		taskList.Enqueue(ri);
	}
	
	long getNewResendTime(ConnectionUDP conn){
		int delayAdd=conn.clientControl.pingDelay+(int) ((float)conn.clientControl.pingDelay*RUDPConfig.reSendDelay);
		if(delayAdd<RUDPConfig.reSendDelay_min){
			delayAdd=RUDPConfig.reSendDelay_min;
		}
		long time=(long) (DateTime.Now.Millisecond+delayAdd);
		return time;
	}
	
	public void run() {
		while(true){
			try {
				ResendItem ri;
                taskList.TryDequeue(out ri);
				if(ri.conn.isConnected()){
					long sleepTime=ri.getResendTime()-DateTime.Now.Millisecond;
					if(sleepTime>0){
						Thread.Sleep(sleepTime);
					}
					ri.addCount();
					
					if(ri.conn.sender.getDataMessage(ri.sequence)!=null){

						if(!ri.conn.stopnow){
							//多线程重发容易内存溢出
//							Route.es.execute(new Runnable() {
//								
//								@Override
//								public void run() {
//									ri.conn.sender.reSend(ri.sequence,ri.getCount());
//								}
//								
//							});
							ri.conn.sender.reSend(ri.sequence,ri.getCount());
						}
					
					}
					if(ri.getCount()<RUDPConfig.reSendTryTimes){
						ri.setResendTime(getNewResendTime(ri.conn));
						taskList.Enqueue(ri);
					}
				}
				if(ri.conn.clientControl.closed){
					break;
				}
			} catch (Exception e) {
                MLog.info(e.Message);
			}
		}
	}
    }
}
