//////////////////////////////////////////////////////////////////////////////////////////
//// HTTPTool.cs
//// time:2019/3/30 下午4:48:27 				
//// author:BanMing   
//// des:http请求工具
////////////////////////////////////////////////////////////////////////////////////////////
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;

public class HTTPTool
{

    ////////////////////////////////////////////////////////////////////////////////////////////本地////////////////////////////////////////////////////////////////////////////////////////////

    //获得文字
    private static IEnumerator getText(string url, Action<string> callback)
    {
        using (var www = UnityWebRequest.Get(url))
        {
            yield return www.SendWebRequest();
            if (www.isNetworkError || www.isHttpError)
            {
                Debug.LogError("GetText url:" + url + "error:" + www.error);
                callback(null);
                yield return null;
            }
            if (www.isDone)
            {
                callback(www.downloadHandler.text);
            }
            yield return null;
        }
    }

    //获得二进制
    private static IEnumerator getBytes(string url, Action<byte[]> callback)
    {
        using (var www = UnityWebRequest.Get(url))
        {
            yield return www.SendWebRequest();
            if (www.isNetworkError || www.isHttpError)
            {
                Debug.LogError("GetBytes url:" + url + "error:" + www.error);
                callback(null);
                yield return null;
            }
            if (www.isDone)
            {
                callback(www.downloadHandler.data);
            }
            yield return null;
        }
    }

    //获得贴图
    private static IEnumerator getTexture(string url, Action<Texture> callback)
    {
        using (var www = UnityWebRequestTexture.GetTexture(url))
        {
            yield return www.SendWebRequest();
            if (www.isNetworkError || www.isHttpError)
            {
                Debug.LogError("GetTexture url:" + url + "error:" + www.error);
                callback(null);
                yield return null;
            }
            if (www.isDone)
            {
                callback(DownloadHandlerTexture.GetContent(www));
            }
            yield return null;
        }
    }

    // 下载文件
    private static IEnumerator downLoadFile(string url, Action<bool> callback, string path)
    {
        using (var www = UnityWebRequest.Get(url))
        {
            www.downloadHandler = new DownloadHandlerFile(path);
            yield return www.SendWebRequest();
            // UnityEngine.Debug.Log("downloadProgress:" + www.downloadProgress);
            if (www.isNetworkError || www.isHttpError)
            {
                Debug.LogError("DownLoadFile url:" + url + "error:" + www.error);
                callback(false);
                yield return null;
            }
            if (www.isDone)
            {
                callback(true);
            }
            yield return null;
        }
    }

    // 获得一个assetbundle
    private static IEnumerator getAssetBundle(string url, Action<AssetBundle> callback)
    {
        using (var www = UnityWebRequestTexture.GetTexture(url))
        {
            yield return www.SendWebRequest();
            if (www.isNetworkError || www.isHttpError)
            {
                Debug.LogError("GetAssetBundle url:" + url + "error:" + www.error);
                callback(null);
                yield return null;
            }
            if (www.isDone)
            {
                callback(DownloadHandlerAssetBundle.GetContent(www));
            }
            yield return null;
        }
    }

    // 上传文件
    private static IEnumerator uploadFile(string url, UploadHandlerRaw uploadHandler, Action<bool> callback)
    {
        using (var www = UnityWebRequest.Get(url))
        {
            www.uploadHandler = uploadHandler;
            yield return www.SendWebRequest();
            // UnityEngine.Debug.Log("downloadProgress:" + www.downloadProgress);
            if (www.isNetworkError || www.isHttpError)
            {
                Debug.LogError("UploadFile url:" + url + "error:" + www.error);
                callback(false);
                yield return null;
            }
            if (www.isDone)
            {
                callback(true);
            }
            yield return null;
        }
    }

    // post请求
    private static IEnumerator post(string url, WWWForm form, Action<string> callback)
    {
        using (var www = UnityWebRequest.Post(url, form))
        {
            yield return www.SendWebRequest();
            if (www.isNetworkError || www.isHttpError)
            {
                Debug.LogError("Post url:" + url + "error:" + www.error);
                callback(null);
                yield return null;
            }
            if (www.isDone)
            {
                callback(www.downloadHandler.text);
            }
            yield return null;
        }
    }
    ////////////////////////////////////////////////////////////////////////////////////////////公共方法////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    /// 获得文字
    /// </summary>
    public static void GetText(string url, Action<string> callback)
    {
        CoroutineTool.Instance.StartCoroutine(getText(url, callback));
    }

    /// <summary>
    ///获得二进制流
    /// </summary>
    public static void GetBytes(string url, Action<byte[]> callback)
    {
        CoroutineTool.Instance.StartCoroutine(getBytes(url, callback));
    }

    /// <summary>
    /// 获得贴图
    /// </summary>
    public static void GetTexture(string url, Action<Texture> callback)
    {
        CoroutineTool.Instance.StartCoroutine(getTexture(url, callback));
    }

    /// <summary>
    /// 获得一个ui上的sprite
    /// </summary>
    public static void GetSprite(string url, Action<Sprite> callback)
    {
        GetTexture(url, (texture) =>
        {
            Sprite.Create((Texture2D)texture, new Rect(0, 0, texture.width, texture.height), Vector2.one * 0.5f);
        });
    }

    /// <summary>
    /// 下载文件
    /// </summary>
    public static void DownLoadFile(string url, Action<bool> callback, string path)
    {
        CoroutineTool.Instance.StartCoroutine(downLoadFile(url, callback, path));
    }

    /// <summary>
    /// 获得一个AssetBundle
    /// </summary>
    public static void GetAssetBundle(string url, Action<AssetBundle> callback)
    {
        CoroutineTool.Instance.StartCoroutine(getAssetBundle(url, callback));
    }

    /// <summary>
    /// 上传文件
    /// </summary>
    public static void UploadFile(string url, UploadHandlerRaw uploadHandler, Action<bool> callback)
    {
        CoroutineTool.Instance.StartCoroutine(uploadFile(url, uploadHandler, callback));
    }

    /// <summary>
    /// post请求
    /// </summary>
    public static void Post(string url, WWWForm form, Action<string> callback)
    {
        CoroutineTool.Instance.StartCoroutine(post(url, form, callback));
    }
}