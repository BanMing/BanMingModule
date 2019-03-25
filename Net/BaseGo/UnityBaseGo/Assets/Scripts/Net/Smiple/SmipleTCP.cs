//////////////////////////////////////////////////////////////////////////////////////////
//// SmipleTCP.cs
//// time:2019/3/25 下午2:20:17 				
//// author:BanMing   
//// des:简单的tcp对应simple-tcp-go
////////////////////////////////////////////////////////////////////////////////////////////
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using UnityEngine;

public enum DisType {
    Exception,
    Disconnect,
}

public class SmipleTCP {

    //////////////////////////////////////////////////////////////////////////////Local//////////////////////////////////////////////////////////////////////////////////////////////////////////
    private TcpClient client = null;
    private NetworkStream outStream = null;
    private MemoryStream memoryStream = null;
    private BinaryReader reader;
    private const int MAX_READ = 8192;
    private byte[] byteBuffer = new byte[MAX_READ];

    // Use this for initialization
    public SmipleTCP () { }

    // 连接网络
    private void connectServer (string host, int port) {
        client = null;
        try {
            // 获得ip地址
            var address = Dns.GetHostAddresses (host);
            if (address.Length == 0) {
                UnityEngine.Debug.LogError ("host invalid!");
                return;
            }
            if (address[0].AddressFamily == AddressFamily.InterNetworkV6) {
                client = new TcpClient (AddressFamily.InterNetworkV6);
            } else {
                client = new TcpClient (AddressFamily.InterNetwork);
            }
            client.SendTimeout = 1000;
            client.ReceiveTimeout = 1000;
            client.BeginConnect (host, port, new AsyncCallback (onConnect), null);
        } catch (System.Exception e) {
            UnityEngine.Debug.Log (e.Message);
        }
    }

    // 连接上服务器 
    private void onConnect (IAsyncResult asr) {
        outStream = client.GetStream ();
        client.GetStream ().BeginRead (byteBuffer, 0, MAX_READ, new AsyncCallback (onRead), null);
        // TODO:通知连接成功
        UnityEngine.Debug.Log ("TCP Connect Success!");
    }

    // 读取消息
    private void onRead (IAsyncResult asr) {
        int bytesRead = 0;
        try {
            lock (client.GetStream ()) {
                // 读取字节流到缓冲区
                bytesRead = client.GetStream ().EndRead (asr);
            }
            if (bytesRead < 1) {
                // 包尺寸有问题，断线处理
                onDisconnected (DisType.Exception, "bytesRead<1");
                return;
            }
            // 分发包内容给逻辑层
            onReceive (byteBuffer, bytesRead);
            // 分析完，再次监听服务发过来的新消息
            lock (client.GetStream ()) {
                // 清空数组
                Array.Clear (byteBuffer, 0, byteBuffer.Length);
                client.GetStream ().BeginRead (byteBuffer, 0, MAX_READ, new AsyncCallback (onRead), null);
            }
        } catch (System.Exception e) {
            // printBytes();
            onDisconnected (DisType.Exception, e.Message);
        }
    }

    //丢失链接 
    private void onDisconnected (DisType dis, string msg) {
        // 关闭连接
        Close ();
        UnityEngine.Debug.LogError ("Connection was closed by the server:" + dis);
    }

    //打印字节 
    private void printBytes () {
        string returnStr = string.Empty;
        for (int i = 0; i < byteBuffer.Length; i++) {
            returnStr += byteBuffer[i].ToString ("X2");
        }
        UnityEngine.Debug.Log ("byteBuffer:" + returnStr);
    }

    // 向链接写入数据流
    void onWrite (IAsyncResult r) {
        try {
            outStream.EndWrite (r);
        } catch (Exception ex) {
            Debug.LogError ("OnWrite--->>>" + ex.Message);
        }
    }

    // 接收到消息
    private void onReceive (byte[] bytes, int length) {
        // 覆盖写入
        memoryStream.Seek (0, SeekOrigin.End);
        memoryStream.Write (bytes, 0, length);
        //Reset to beginning
        memoryStream.Seek (0, SeekOrigin.Begin);
        while (remainingBytes () > 2) {
            // 消息的前2位是消息长度（长度0~65,535）
            ushort messageLen = reader.ReadUInt16 ();
            if (remainingBytes () >= messageLen) {
                MemoryStream ms = new MemoryStream ();
                BinaryWriter writer = new BinaryWriter (ms);
                writer.Write (reader.ReadBytes (messageLen));
                ms.Seek (0, SeekOrigin.Begin);
                onReceivedMessage (ms);
            } else {
                //Back up the position two bytes
                memoryStream.Position = memoryStream.Position - 2;
                break;
            }
        }
        //Create a new stream with any leftover bytes
        byte[] leftover = reader.ReadBytes ((int) remainingBytes ());
        memoryStream.SetLength (0); //Clear
        memoryStream.Write (leftover, 0, leftover.Length);
    }

    // 写数据
    private void writeMessage (byte[] message) {
        MemoryStream ms = null;
        using (ms = new MemoryStream ()) {
            ms.Position = 0;
            BinaryWriter writer = new BinaryWriter (ms);
            ushort msglen = (ushort) message.Length;
            writer.Write (msglen);
            writer.Write (message);
            writer.Flush ();
            if (client != null && client.Connected) {
                //NetworkStream stream = client.GetStream();
                byte[] payload = ms.ToArray ();
                outStream.BeginWrite (payload, 0, payload.Length, new AsyncCallback (onWrite), null);
            } else {
                Debug.LogError ("client.connected----->>false");
            }
        }
    }

    // 剩余的字节
    private long remainingBytes () {
        return memoryStream.Length - memoryStream.Position;
    }

    // 接收到消息
    void onReceivedMessage (MemoryStream ms) {
        BinaryReader r = new BinaryReader (ms);
        // test
        UnityEngine.Debug.Log ("onReceivedMessage:" + r.ReadString ());
        // byte[] message = r.ReadBytes ((int) (ms.Length - ms.Position));
        //int msglen = message.Length;

        // ByteBuffer buffer = new ByteBuffer (message);
        // int mainId = buffer.ReadShort ();
        // NetworkManager.AddEvent (mainId, buffer);
    }

    // 会话发送
    void sessionSend (byte[] bytes) {
        writeMessage (bytes);
    }

    //////////////////////////////////////////////////////////////////////////////Public//////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    /// 注册代理
    /// </summary>
    public void OnRegister () {
        memoryStream = new MemoryStream ();
        reader = new BinaryReader (memoryStream);
    }

    /// <summary>
    /// 移除代理
    /// </summary>
    public void OnRemove () {
        this.Close ();
        reader.Close ();
        memoryStream.Close ();
    }

    /// <summary>
    /// 关闭连接
    /// </summary>
    public void Close () {
        if (client != null) {
            if (client.Connected) {
                client.Close ();
            }
        }
    }

    /// <summary>
    /// 发送连接请求
    /// </summary>
    public void SendConnect () {
        connectServer (AppConst.SocketAddress, AppConst.SocketPort);
    }

    /// <summary>
    /// 发送消息
    /// </summary>
    public void SendMessage (byte[] bytes) {
        sessionSend (bytes);
    }
}