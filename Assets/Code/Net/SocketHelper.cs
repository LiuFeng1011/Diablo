﻿using System;
using System.Collections;
using System.Collections.Generic;  
using System.Linq;  
using System.Text;  
using System.Net;  
using System.Net.Sockets;  
using System.Threading;  
using UnityEngine; 

public class DataHolder
{
	public byte[] mRecvDataCache;//use array as buffer for efficiency consideration
	public byte[] mRecvData;

	private int mTail = -1;
	private int packLen;
	public void PushData(byte[] data, int length)
	{
		if (mRecvDataCache == null)
			mRecvDataCache = new byte[length];

		if (this.Count + length >  this.Capacity)//current capacity is not enough, enlarge the cache
		{
			byte[] newArr = new byte[this.Count + length];
			mRecvDataCache.CopyTo(newArr, 0);
			mRecvDataCache = newArr;
		}

		Array.Copy(data, 0, mRecvDataCache, mTail + 1, length);
		mTail += length;
	}

	public bool IsFinished()
	{
		if (this.Count == 0)
		{
			//skip if no data is currently in the cache
			return false;
		}

		if (this.Count >= 4)
		{
			DataStream reader = new DataStream(mRecvDataCache, true);
			packLen = (int)reader.ReadInt32();
			if (packLen > 0)
			{
				if (this.Count - 4 >= packLen)
				{
					mRecvData = new byte[packLen];
					Array.Copy(mRecvDataCache, 4, mRecvData,0,packLen);
					return true;
				}

				return false;
			}
			return false;
		}

		return false;
	}

	public void Reset()
	{
		mTail = -1;
	}

	public void RemoveFromHead()
	{
		int countToRemove = packLen + 4;
		if (countToRemove > 0 && this.Count - countToRemove > 0)
		{
			Array.Copy(mRecvDataCache, countToRemove, mRecvDataCache, 0, this.Count - countToRemove);
		}
		mTail -= countToRemove;
	}

	//cache capacity
	public int Capacity
	{
		get
		{
			return mRecvDataCache != null ? mRecvDataCache.Length : 0;
		}
	}

	//indicate how much data is currently in cache in bytes
	public int Count
	{
		get
		{
			return mTail + 1;
		}
	}
}

public class SocketHelper : MonoBehaviour{

	private DataHolder mDataHolder = new DataHolder();
	private static SocketHelper socketHelper;  


	public delegate void ConnectCallback();

	ConnectCallback connectDelegate = null;
	ConnectCallback connectFailedDelegate = null;

	Queue<byte[]> dataQueue = new Queue<byte[]>();

	Queue<Request> sendDataQueue = new Queue<Request>();

	List<int> maskList = new List<int>();

	private Socket socket;  
	bool isStopReceive = true;

	public static SocketHelper GetInstance()  
	{  
		return socketHelper;  
	}  

	void Awake(){
		socketHelper = this;
	}

	public void Connect(ConnectCallback connectCallback,ConnectCallback connectFailedCallback){
		connectDelegate = connectCallback;
		connectFailedDelegate = connectFailedCallback;

		//采用TCP方式连接  
		socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);  

		//服务器IP地址  
		IPAddress address = IPAddress.Parse(NetProtocols.SERVER_ADDRESS);  

		//服务器端口  
		IPEndPoint endpoint = new IPEndPoint(address,NetProtocols.SERVER_PORT);  

		//异步连接,连接成功调用connectCallback方法  
		IAsyncResult result = socket.BeginConnect(endpoint, new AsyncCallback(ConnectedCallback), socket);  

		//这里做一个超时的监测，当连接超过5秒还没成功表示超时  
		bool success = result.AsyncWaitHandle.WaitOne(5000, true);  
		if (!success)  
		{  
			//超时  
			Closed();  
			NetTools.Log("connect Time Out");  
			if(connectFailedDelegate != null){
				connectFailedDelegate();
			}
		}  
		else  
		{  
			//与socket建立连接成功，开启线程接受服务端数据。  
			isStopReceive = false;
			Thread thread = new Thread(new ThreadStart(ReceiveSorket));  
			thread.IsBackground = true;  
			thread.Start();  
		}  

		//
		RegisterResp.RegisterAll();
	}

	private void ConnectedCallback(IAsyncResult asyncConnect)  
	{  
		if (!socket.Connected)  {
			if(connectFailedDelegate != null){
				connectFailedDelegate();
			}
			return;
		}

		NetTools.Log("connect success");  

		if(connectDelegate != null){
			connectDelegate();
		}
	}  

	private void ReceiveSorket(){
		mDataHolder.Reset();
		while(!isStopReceive){
			if (!socket.Connected)  
			{  
				//与服务器断开连接跳出循环  
				NetTools.Log("Failed to clientSocket server.");  
				socket.Close();  
				break;  
			}  

			try  
			{  
				//接受数据保存至bytes当中  
				byte[] bytes = new byte[4096];  
				//Receive方法中会一直等待服务端回发消息  
				//如果没有回发会一直在这里等着。  

				int i = socket.Receive(bytes); 

				if (i <= 0)  
				{  
					socket.Close();  
					break;  
				}  
				mDataHolder.PushData(bytes, i);

				while(mDataHolder.IsFinished()){
					dataQueue.Enqueue(mDataHolder.mRecvData);

					mDataHolder.RemoveFromHead();
				}
			}  
			catch (Exception e)  
			{  
				NetTools.Log("Failed to clientSocket error." + e);  
				socket.Close();  
				break;  
			}  
		}
	}

	//接收到数据放入数据队列，按顺序取出
	void Update(){
		if(dataQueue.Count > 0){
			NetTools.Log("dataQueue.Count : " + dataQueue.Count);
			Resp resp = ProtoManager.Instance.TryDeserialize(dataQueue.Dequeue());

			if(maskList.Contains(resp.GetProtocol())){
				maskList.Remove(resp.GetProtocol());
			}
			if(maskList.Count <= 0){

//				FuiController.CloseLoading();
			}
		}

		if(sendDataQueue.Count > 0){
			Send();
		}
	}


	//关闭Socket  
	public void Closed()  
	{  
		isStopReceive = true;

		if (socket != null && socket.Connected)  
		{  
			socket.Shutdown(SocketShutdown.Both);  
			socket.Close();  
		}  
		socket = null;  
	}  

	public bool isConnect(){
		return socket != null && socket.Connected;
	}

	public void SendMessage(Request req)  
	{  
		sendDataQueue.Enqueue(req);
	}  

	private void Send(){
		if(socket == null){
			return;
		}
		if (!socket.Connected)  
		{  
			Closed();
			return;  
		}  
		try  
		{  
			Request req = sendDataQueue.Dequeue();
			NetTools.Log("pto : " + req.GetProtocol());
			maskList.Add(req.GetProtocol());
//			FuiController.Loading();

			DataStream bufferWriter = new DataStream(true);
			req.Serialize(bufferWriter);
			byte[] msg = bufferWriter.ToByteArray();

			byte[] buffer = new byte[msg.Length + 4];
			DataStream writer = new DataStream(buffer, true);

			writer.WriteInt32((uint)msg.Length);//增加数据长度
			writer.WriteRaw(msg);

			byte[] data = writer.ToByteArray();

			IAsyncResult asyncSend = socket.BeginSend(data, 0, data.Length, SocketFlags.None, new AsyncCallback(SendCallback), socket);  
			bool success = asyncSend.AsyncWaitHandle.WaitOne(5000, true);  
			if (!success)  
			{  
				Closed();
				NetTools.Log("Failed to SendMessage server.");  
			}  
		}  
		catch  (Exception e)
		{  
//			FuiController.CloseLoading();
			NetTools.Log("send error : " + e.ToString());
		} 
	}

	private void SendCallback(IAsyncResult asyncConnect)  
	{  
		NetTools.Log("send success");  
	}  

	void OnDestroy(){ 
		Closed();  
	}
}
