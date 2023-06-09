using System;

namespace WIGO.Test
{
    public enum NativeMessageType
    {
        Video,
        Location,
        Other
    }

    public static class MessageRouter
    {
        public static Action<NativeMessageType, string> onMessageReceive;

        public static void RouteMessage(NativeMessageType type, string message)
        {
            onMessageReceive?.Invoke(type, message);
        }
    }
}
