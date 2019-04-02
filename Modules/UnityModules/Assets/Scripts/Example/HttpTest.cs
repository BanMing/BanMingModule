//////////////////////////////////////////////////////////////////////////////////////////
//// HttpTest.cs
//// time:2019/3/30 下午5:17:08 				
//// author:BanMing   
//// des:
////////////////////////////////////////////////////////////////////////////////////////////
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class HttpTest : MonoBehaviour
{
    public async Task<string> GetText(string url)
    {
        // var task=new Task(StartCoroutine (getText (url)));
        // text.text = "2222222222222";
        await Task.Delay(3000);
        // asyncRes = new AsyncResult ();
        // Task.Run ()
        // return await StartCoroutine (getText (url));
        await Task.Run(() =>
        {
            UnityEngine.Debug.Log("ssssssssss");
        });
        return url;
    }
    private IEnumerator getText(string url)
    {
        var req = UnityWebRequest.GetAssetBundle(url);
        req.SendWebRequest();
        // DownloadHandlerAssetBundle.GetContent(req)
        using (UnityWebRequest www = UnityWebRequest.Get(url))
        {
            yield return www.SendWebRequest();
            if (www.isNetworkError || www.isHttpError)
            {
                UnityEngine.Debug.LogError("GetText" + url + " Error:" + www.error);
            }
            else
            {
                yield return www.downloadHandler.text;
            }
        }
    }
    private Text text;

    private void Start()
    {
        text = GameObject.Find("Text").GetComponent<Text>();
    }
    public async void Click()
    {
        // text.text = await GetText("ssssssssssssssssss");
        // text.text = "3";
        // text.text = await GetText("sss2");
        // text.text = "4";
        // HTTPTool.DownLoadFile("http://www.ban-ming.com/serverfile.txt", (isDown) => { }, Application.dataPath+"/test.txt");
        var bytes = File.ReadAllBytes(Application.dataPath + "/test.txt");
        UploadHandlerRaw uploadHandler = new UploadHandlerRaw(bytes);
        HTTPTool.UploadFile("http://www.ban-ming.com/serverfile.txt", uploadHandler, (isOk) => { UnityEngine.Debug.Log("isOk:" + isOk); });
    }
}