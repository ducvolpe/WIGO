using AOT;
using System.Runtime.InteropServices;
using UnityEngine;

namespace WIGO.Test
{
    public static class MessageIOSHandler
    {
#if UNITY_IOS
#region open controllers
        [DllImport("__Internal")]
        private static extern void startSwiftCameraController();

        [DllImport("__Internal")]
        private static extern void startSwiftMapController();

        [DllImport("__Internal")]
        private static extern void startSwiftTestController();
#endregion

#region delegates
        public delegate void SwiftTestPluginVideoDidSave(string value);
        [DllImport("__Internal")]
        private static extern void setSwiftTestPluginVideoDidSave(SwiftTestPluginVideoDidSave callBack);

        public delegate void SwiftTestPluginLocationDidSend(string value);
        [DllImport("__Internal")]
        private static extern void setSwiftTestPluginLocationDidSend(SwiftTestPluginLocationDidSend callBack);
#endregion

#region delegate handlers
        [MonoPInvokeCallback(typeof(SwiftTestPluginVideoDidSave))]
        public static void setSwiftTestPluginVideoDidSave(string value)
        {
            MessageRouter.RouteMessage(NativeMessageType.Video, value);
        }

        [MonoPInvokeCallback(typeof(SwiftTestPluginLocationDidSend))]
        public static void setSwiftTestPluginLocationDidSend(string value)
        {
            MessageRouter.RouteMessage(NativeMessageType.Location, value);
        }
#endregion

        [RuntimeInitializeOnLoadMethod]
        private static void Initialize()
        {
            setSwiftTestPluginVideoDidSave(setSwiftTestPluginVideoDidSave);
            setSwiftTestPluginLocationDidSend(setSwiftTestPluginLocationDidSend);
        }

        public static void OnPressCameraButton()
        {
            startSwiftCameraController();
        }

        public static void OnPressMapButton()
        {
            startSwiftMapController();
        }

        public static void OnPressTestButton()
        {
            startSwiftTestController();
        }
#endif
    }
}
