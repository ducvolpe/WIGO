using System;
using System.IO;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using WIGO.Core;

namespace WIGO.Userinterface
{
    public abstract class EventInfoWindow : UIWindow
    {
        [SerializeField] RectTransform _previewMask;
        [SerializeField] RawImage _preview;
        [SerializeField] protected TMP_InputField _descIF;
        [SerializeField] TMP_Text _counterDescLabel;
        [SerializeField] TMP_Text _sendButton;
        [SerializeField] Image _overlay;
        [SerializeField] GameObject _loader;
        [SerializeField] GameObject _doneElement;
        [SerializeField] protected WindowAnimator _animator;
        [SerializeField] Texture2D _tempPreview;

        string _videoPath;

        const float MAX_PREVIEW_HEIGHT = 164f;

        public override void OnOpen(WindowId previous)
        {
            _animator.OnOpen();
        }

        public override void OnBack(WindowId previous, Action callback = null)
        {
            ClearWindow();
            callback?.Invoke();
        }

        public virtual void Setup(string path)
        {
            if (!string.IsNullOrEmpty(path))
            {
                _videoPath = path;
#if UNITY_EDITOR
                var preview = _tempPreview;
#elif UNITY_ANDROID || UNITY_IOS
                var preview = NativeCamera.GetVideoThumbnail(path);
#endif
                float aspect = (float)preview.width / preview.height;
                float previewHeight = _previewMask.rect.width / aspect;
                float height = Mathf.Min(previewHeight, MAX_PREVIEW_HEIGHT);
                _previewMask.sizeDelta = new Vector2(_previewMask.sizeDelta.x, height);
                _preview.texture = preview;
                _preview.rectTransform.sizeDelta = new Vector2(_preview.rectTransform.sizeDelta.x, previewHeight);
            }

            _descIF.SetTextWithoutNotify(string.Empty);
            UIGameColors.SetTransparent(_sendButton, 0.4f);
            _counterDescLabel.text = $"{_descIF.characterLimit}/{_descIF.characterLimit}";
        }

        public void OnBackButtonClick()
        {
            ServiceLocator.Get<UIManager>().CloseCurrent();
        }

        public virtual void OnEditDescText(string text)
        {
            int count = text.Length;
            int remaining = _descIF.characterLimit - count;
            _counterDescLabel.text = $"{remaining}/{_descIF.characterLimit}";
        }

        public virtual async void OnPublishClick()
        {
            if (IsAvailable())
            {
                await CreateEventOrResponse();
                await Task.Delay(1200);
                ServiceLocator.Get<UIManager>().SwitchTo(WindowId.FEED_SCREEN);
            }
        }

        protected async Task<EventCard> CreateEventOrResponse()
        {
            Debug.Log("Send new event");
            UIGameColors.SetTransparent(_overlay, 0.8f);
            _overlay.gameObject.SetActive(true);
            _loader.SetActive(true);

            await Task.Delay(2000);

            if (true)
            {
                UIGameColors.SetTransparent(_overlay, 1f);
                _loader.SetActive(false);
                _doneElement.SetActive(true);
                if (File.Exists(_videoPath))
                    File.Delete(_videoPath);

                EventCard card = EventCard.CreateEmpty();
                return card;
            }
            //else
            //{
            //    _overlay.gameObject.SetActive(false);
            //    _loader.SetActive(false);
            //}
        }

        protected virtual void ClearWindow()
        {
#if !UNITY_EDITOR
            Destroy(_preview.texture);
#endif
            _videoPath = null;
            _overlay.gameObject.SetActive(false);
            _doneElement.SetActive(false);
        }

        protected virtual bool IsAvailable()
        {
            return false;
        }

        protected void CheckIfAvailable()
        {
            float alpha = IsAvailable() ? 1f : 0.4f;
            UIGameColors.SetTransparent(_sendButton, alpha);
        }
    }
}
