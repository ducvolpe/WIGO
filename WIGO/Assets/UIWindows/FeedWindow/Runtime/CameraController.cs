using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.InteropServices;
using AOT;

public class CameraController : MonoBehaviour
{
    [DllImport("__Internal")]
    private static extern void startSwiftCameraController();
    
    public delegate void SwiftTestPluginVideoDidSave(string value);
    [DllImport("__Internal")]
    private static extern void setSwiftTestPluginVideoDidSave(SwiftTestPluginVideoDidSave callBack);

    
    [MonoPInvokeCallback(typeof(SwiftTestPluginVideoDidSave))]
    public static void setSwiftTestPluginVideoDidSave(string value)
    {
        Debug.Log("SwiftCodeKit value did change. Value: " + value);
    }
    
    public void OpenNativeCamera()
    {
        startSwiftCameraController();
    }
}
