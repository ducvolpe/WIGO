using UnityEngine;
using WIGO.Core;
using TMPro;
using System;

namespace WIGO.Userinterface
{
    public class ProfileWindow : UIWindow
    {
        [SerializeField] ProfilePhotoScroll _profilePhotoElement;
        [Space]
        [SerializeField] TMP_Text _displayNameLabel;
        [SerializeField] TMP_Text _usernameLabel;
        [SerializeField] TMP_Text _raitingLabel;
        [SerializeField] TMP_Text _aboutLabel;

        public override void OnClose(WindowId next, Action callback = null)
        {
            _profilePhotoElement.Clear();
            base.OnClose(next, callback);
        }

        public void Setup(UserProfile profile)
        {
            _profilePhotoElement.Setup(profile.GetPhotos());

            _displayNameLabel.text = profile.GetDisplayName();
            _usernameLabel.text = $"@{profile.GetUsername()}";
            _raitingLabel.text = string.Format("{0:0.0}", profile.GetRaiting());
            _aboutLabel.text = profile.GetAboutDesc();
        }

        public void OnBackButtonClick()
        {
            ServiceLocator.Get<UIManager>().CloseCurrent();
        }

        public void OnMainFeedClick()
        {
            ServiceLocator.Get<UIManager>().SwitchTo(WindowId.FEED_SCREEN);
        }

        public void OnSettingsClick()
        {
            ServiceLocator.Get<UIManager>().Open<SettingsWindow>(WindowId.SETTINGS_SCREEN);
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.LeftArrow))
            {
                _profilePhotoElement.MoveTo(-1);
            }

            if (Input.GetKeyDown(KeyCode.RightArrow))
            {
                _profilePhotoElement.MoveTo(1);
            }
        }
    }
}
