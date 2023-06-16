using TMPro;
using UnityEngine;

namespace WIGO.Test
{
    public class UITestIOSController : MonoBehaviour
    {
        [SerializeField] GameObject _startWindow;
        [SerializeField] UITestVideoWindow _resultWindow;
        [SerializeField] GameObject _locationWindow;
        [SerializeField] TMP_Text _locationResultLabel;
        [SerializeField] string _videoPath;

        public void OnTestClick()
        {
#if UNITY_EDITOR
            Debug.Log("Editor version: OK");
#elif UNITY_IOS
            MessageIOSHandler.OnPressTestButton();
#endif
        }

        public void OnCameraClick()
        {
#if UNITY_EDITOR
            string path = System.IO.Path.Combine(Application.streamingAssetsPath, _videoPath);
            OnRecordComplete(path);
#elif UNITY_IOS
            MessageIOSHandler.OnPressCameraButton();
#endif
        }

        public void OnLocationClick()
        {
#if UNITY_EDITOR
            _locationResultLabel.SetText("My location: -5421.67; 268.1");
            OnDisplayLocation("-5421.67; 268.1");
#elif UNITY_IOS
            MessageIOSHandler.OnPressMapButton();
#endif
        }

        public void OnLocationBackClick()
        {
            _startWindow.SetActive(true);
            _locationWindow.SetActive(false);
        }

        private void Awake()
        {
//#if UNITY_IOS
//            MessageIOSHandler.Initialize();
//#endif

            MessageRouter.onMessageReceive += OnReceiveMessage;
            _resultWindow.Init(OnBackClick);
        }

        private void OnApplicationQuit()
        {
            MessageRouter.onMessageReceive -= OnReceiveMessage;
        }

        void OnBackClick()
        {
            _startWindow.SetActive(true);
            _resultWindow.OnClose();
        }

        void OnRecordComplete(string path)
        {
            Debug.LogFormat("Path: {0}", path);
            _startWindow.SetActive(false);
            _resultWindow.OnOpen(path);
        }

        void OnDisplayLocation(string location)
        {
            _locationResultLabel.SetText($"My location: {location}");
            _startWindow.SetActive(false);
            _locationWindow.SetActive(true);
        }

        void OnReceiveMessage(NativeMessageType type, string message)
        {
            switch (type)
            {
                case NativeMessageType.Video:
                    OnRecordComplete(message);
                    break;
                case NativeMessageType.Location:
                    OnDisplayLocation(message);
                    break;
                case NativeMessageType.Other:
                    Debug.LogFormat("Message: {0}", message);
                    break;
                default:
                    break;
            }
        }
    }
}
