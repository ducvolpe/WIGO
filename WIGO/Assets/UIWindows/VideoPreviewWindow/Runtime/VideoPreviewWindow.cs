using System.Collections;
using System;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

namespace WIGO.Userinterface
{
    public class VideoPreviewWindow : UIWindow
    {
        [SerializeField] VideoPlayer _videoPlayer;
        [SerializeField] RectTransform _maskBounds;
        [SerializeField] RawImage _preview;
        [SerializeField] GameObject _playButton;

        RenderTexture _videoTexture;
        string _videoPath;
        bool _isPlaying;
        bool _isResponse;

        const float UPPER_DEFAULT_PADDING = 56f;
        const float BOTTOM_DEFAULT_PADDING = 116f;

        public override void OnBack(WindowId previous, Action callback = null)
        {
            ClearData();
            callback?.Invoke();
        }

        public override void CloseUnactive()
        {
            ClearData();
        }

        public void Setup(string path, Vector2Int videoSize, bool response)
        {
            if (string.IsNullOrEmpty(path) || !File.Exists(path))
            {
                Debug.LogError("Can't find video");
                return;
            }

            _isResponse = response;
            _videoPath = path;
            SetupCardTextureSize(videoSize.x, videoSize.y);
            _videoTexture = new RenderTexture(videoSize.x, videoSize.y, 32);
            _preview.texture = _videoTexture;

            _videoPlayer.targetTexture = _videoTexture;
            _videoPlayer.url = path;
            _videoPlayer.loopPointReached += (player) => 
            { 
                player.Stop();
                _isPlaying = false;
                _playButton.SetActive(true);
            };
            _videoPlayer.Play();
            StartCoroutine(StopVideoAfterFrame());
        }

        public void OnBackButtonClick()
        {
            ServiceLocator.Get<UIManager>().CloseCurrent();
        }

        public void OnContinueClick()
        {
            _videoPlayer.Stop();
            _isPlaying = false;
            _playButton.SetActive(true);
            if (_isResponse)
                ServiceLocator.Get<UIManager>().Open<SaveEventWindow>(WindowId.SAVE_EVENT_SCREEN, (window) => window.Setup(_videoPath));
            else
                ServiceLocator.Get<UIManager>().Open<CreateEventWindow>(WindowId.CREATE_EVENT_SCREEN, (window) => window.Setup(_videoPath));
        }

        public void OnPlayControlClick()
        {
            _playButton.SetActive(_isPlaying);
            if (_isPlaying)
            {
                _videoPlayer.Pause();
            }
            else
            {
                _videoPlayer.Play();
            }

            _isPlaying = !_isPlaying;
        }

        void SetupCardTextureSize(int width, int height)
        {
            float aspect = (float)width / height;
            
            float cardWidth = _preview.rectTransform.rect.width;
            float cardHeight = cardWidth / aspect;

            _preview.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, cardHeight);
            float screenHeight = ServiceLocator.Get<UIManager>().GetCanvasSize().y;
            float maskHeight = _maskBounds.rect.height > cardHeight 
                ? cardHeight 
                : Mathf.Min(screenHeight - UPPER_DEFAULT_PADDING - BOTTOM_DEFAULT_PADDING, cardHeight);
            _maskBounds.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, maskHeight);
        }

        void ClearData()
        {
            if (_videoTexture != null)
            {
                _videoPlayer.Stop();
                _videoTexture.Release();
                Destroy(_videoTexture);
                _videoTexture = null;
                _videoPath = null;
                _isPlaying = false;
                _playButton.SetActive(true);
            }
        }

        IEnumerator StopVideoAfterFrame()
        {
            yield return new WaitForEndOfFrame();
            _videoPlayer.Pause();
        }
    }
}
