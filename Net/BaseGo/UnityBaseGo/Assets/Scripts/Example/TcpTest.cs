//////////////////////////////////////////////////////////////////////////////////////////
//// TcpTest.cs
//// time:2019/3/25 下午4:24:30 				
//// author:BanMing   
//// des:网络连接测试
////////////////////////////////////////////////////////////////////////////////////////////
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class TcpTest : MonoBehaviour {

    private InputField input;
    private SampleTCP smipleTCP;
    private void Awake () {
        smipleTCP = new SampleTCP ();
        smipleTCP.OnRegister ();
        input = GameObject.Find ("InputField").GetComponent<InputField> ();
    }

    public void Connect () {
        smipleTCP.SendConnect (AppConst.SocketAddress, AppConst.SocketPort);
    }
    private void OnDestroy () {
        smipleTCP.OnRemove ();
    }
    public void Send () {
        smipleTCP.SendMessage (System.Text.Encoding.Default.GetBytes (input.text));
        input.text = "";
    }

}