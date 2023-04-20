using NatML.Devices;
using NatML.Devices.Outputs;
using NatSuite.Recorders;
using NatSuite.Recorders.Clocks;
using NatSuite.Recorders.Inputs;
using System.Collections;
using UnityEngine;
using UnityEngine.Android;
using UnityEngine.UI;
using WIGO.Userinterface;

public class TestNatMLCamera : MonoBehaviour
{
    [System.Serializable]
    private enum RecordType
    {
        NatDeviceCam,
        NatDeviceMic,
        CoreWebCam,
        CoreWebMic
    }

    [SerializeField] RawImage _camImage;
    [SerializeField] RecordUIButton _recordButton;
    [SerializeField] AudioSource _audioSource;
    [SerializeField] RecordType _type;

    CameraDevice _currentCamera;
    AudioDevice _currentMicrophone;
    MP4Recorder _recorder;
    Texture2D _streamingTexture;
    WebCamTexture _coreTexture;
    Vector2Int _videoSize;
    int _camIndex = 2;
    bool _recording;

    public void OnRecordClick()
    {
        if (!_recording)
        {
            OnStartRecordVideo();
        }
    }

    public void OnChangeCamera(int index)
    {
        _camIndex = index;
        SetupNatDevice();
    }

    private void Awake()
    {
        var callbacks = new PermissionCallbacks();
        callbacks.PermissionDenied += PermissionCallbackDenied;
        callbacks.PermissionGranted += PermissionCallbackGranted;

        Permission.RequestUserPermission(Permission.Camera, callbacks);
    }

    void PermissionCallbackDenied(string permissionName)
    {
        Debug.Log($"<color=red>{permissionName} Denied</color>");

        var callbacks = new PermissionCallbacks();
        callbacks.PermissionDenied += (name) => Debug.Log($"<color=red>{name} Denied</color>");
        callbacks.PermissionGranted += (name) => Debug.Log($"<color=green>{permissionName} Granted</color>");
        Permission.RequestUserPermission(Permission.Microphone, callbacks);
    }

    void PermissionCallbackGranted(string permissionName)
    {
        Debug.Log($"<color=green>{permissionName} Granted</color>");

        var callbacks = new PermissionCallbacks();
        callbacks.PermissionDenied += (name) => Debug.Log($"<color=red>{name} Denied</color>");
        callbacks.PermissionGranted += (name) => Debug.Log($"<color=green>{permissionName} Granted</color>");
        Permission.RequestUserPermission(Permission.Microphone, callbacks);
    }

    //    async void Start()
    //    {
    //        Permission.RequestUserPermission(Permission.Camera);
    //        // Create a device query for the front camera 
    //        var filter = MediaDeviceCriteria.CameraDevice;
    ////#if UNITY_ANDROID && !UNITY_EDITOR
    ////        filter = MediaDeviceCriteria.FrontCamera;
    ////#endif
    //        var query = new MediaDeviceQuery(filter);
    //        // Create a device query for the microphone 
    //        var micQuery = new MediaDeviceQuery(MediaDeviceCriteria.AudioDevice);

    //        // Get the camera device
    //        _currentCamera = query.current as CameraDevice;
    //        // Get the microphone device
    //        _currentMicrophone = micQuery.current as AudioDevice;

    //        // Start the camera preview
    //        var textureOutput = new TextureOutput(); // stick around for an explainer
    //        _currentCamera.StartRunning(textureOutput);
    //        // Display the preview in our UI
    //        _streamingTexture = await textureOutput;
    //        _camImage.texture = _streamingTexture;
    //        float aspect = (float)_streamingTexture.width / _streamingTexture.height;
    //        _camImage.rectTransform.sizeDelta = new Vector2(_camImage.rectTransform.sizeDelta.x, _camImage.rectTransform.rect.width / aspect);
    //        _camImage.color = Color.white;

    //        _currentMicrophone.StartRunning(audioBuffer => _audioBuffer = audioBuffer);
    //    }

    private void Start()
    {
        switch (_type)
        {
            case RecordType.NatDeviceCam:
                SetupNatDevice();
                break;
            case RecordType.NatDeviceMic:
                SetupNatDeviceMic();
                break;
            case RecordType.CoreWebCam:
                SetupCore();
                break;
            case RecordType.CoreWebMic:
                SetupCoreMic();
                break;
            default:
                break;
        }
    }

    #region NatDeviceWebCam
    async void SetupNatDevice()
    {
        // [TODO]: Stop current camdevice
        System.Predicate<IMediaDevice> cam = null;
        switch (_camIndex)
        {
            case 0:
                cam = MediaDeviceCriteria.CameraDevice;
                break;
            case 1:
                cam = MediaDeviceCriteria.RearCamera;
                break;
            case 2:
                cam = MediaDeviceCriteria.FrontCamera;
                break;
            default:
                break;
        }

        var query = new MediaDeviceQuery(cam);
        CameraDevice camDevice = query.current as CameraDevice;

        if (camDevice == null)
        {
            Debug.LogError("Current camera is null");
            return;
        }

        Debug.LogFormat("Devices: {0}; Front - {1}", query.count, (camDevice.frontFacing ? "yes" : "no"));
        var textureOutput = new TextureOutput();
        camDevice.StartRunning(textureOutput);

        _streamingTexture = await textureOutput;
        SetTexture(_streamingTexture);
    }
    #endregion

    void SetupNatDeviceMic()
    {
        var query = new MediaDeviceQuery(MediaDeviceCriteria.AudioDevice);
        _currentMicrophone = query.current as AudioDevice;
        SetupNatDevice();
    }

    void SetupCore()
    {
        //WebCamDevice[] devices = WebCamTexture.devices;
        _coreTexture = new WebCamTexture();
        _coreTexture.Play();

        SetTexture(_coreTexture);
    }

    void SetupCoreMic()
    {
        SetupCore();
        _audioSource.clip = Microphone.Start(null, true, 10, 44100);
        _audioSource.Play();
    }

    void SetTexture(Texture texture)
    {
        _camImage.texture = texture;
        float aspect = (float)texture.width / texture.height;
        _camImage.rectTransform.sizeDelta = new Vector2(_camImage.rectTransform.sizeDelta.x, _camImage.rectTransform.rect.width / aspect);
        UIGameColors.SetTransparent(_camImage, 1f);
        _videoSize = new Vector2Int(texture.width, texture.height);
    }

    void OnStartRecordVideo()
    {
        _recording = true;
        _recorder = _type == RecordType.NatDeviceMic
            ? new MP4Recorder(_videoSize.x, _videoSize.y, 30, _currentMicrophone.sampleRate, _currentMicrophone.channelCount)
            : new MP4Recorder(_videoSize.x, _videoSize.y, 30);
        _recordButton.StartRecordMode();

        switch (_type)
        {
            case RecordType.NatDeviceCam:
                StartCoroutine(RecordProcess());
                break;
            case RecordType.NatDeviceMic:
                StartCoroutine(RecordProcessMic());
                break;
            case RecordType.CoreWebCam:
                StartCoroutine(RecordProcessCore());
                break;
            case RecordType.CoreWebMic:
                StartCoroutine(RecordProcessCoreMic());
                break;
            default:
                break;
        }
    }

    async void FinishWriting()
    {
        _recording = false;
        _recordButton.StopRecordMode();
        _currentMicrophone?.StopRunning();
        var path = await _recorder.FinishWriting();
        Debug.LogFormat("Record complete: {0}", path);
    }

    IEnumerator RecordProcess()
    {
        var clock = new RealtimeClock();
        for (var i = 0; i < 300; ++i)
        {
            _recorder.CommitFrame(_streamingTexture.GetPixels32(), clock.timestamp);
            yield return null;
        }

        FinishWriting();
    }

    IEnumerator RecordProcessCore()
    {
        var clock = new RealtimeClock();
        for (var i = 0; i < 300; ++i)
        {
            _recorder.CommitFrame(_coreTexture.GetPixels32(), clock.timestamp);
            yield return null;
        }

        FinishWriting();
    }

    IEnumerator RecordProcessCoreMic()
    {
        var clock = new RealtimeClock();
        var audioInput = new AudioInput(_recorder, clock, _audioSource);
        for (var i = 0; i < 300; ++i)
        {
            _recorder.CommitFrame(_coreTexture.GetPixels32(), clock.timestamp);
            yield return null;
        }

        audioInput.Dispose();
        FinishWriting();
    }

    IEnumerator RecordProcessMic()
    {
        var clock = new RealtimeClock();
        _currentMicrophone.StartRunning(audioBuffer => _recorder.CommitSamples(audioBuffer.sampleBuffer.ToArray(), clock.timestamp));

        for (var i = 0; i < 300; ++i)
        {
            _recorder.CommitFrame(_streamingTexture.GetPixels32(), clock.timestamp);
            yield return null;
        }

        FinishWriting();
    }
}
