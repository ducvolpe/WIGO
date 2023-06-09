using System;
using System.Collections.Generic;
using UnityEngine;

namespace WIGO.Core
{
    [Serializable]
    public class UserProfile
    {
        [SerializeField] string _id;
        [SerializeField] string _displayName;
        [SerializeField] string _username;
        [TextArea]
        [SerializeField] string _aboutDesc;
        [SerializeField] float _raiting;
        [SerializeField] ProfileAvatar _photos;

        public UserProfile()
        {
            _photos = new ProfileAvatar();
        }

        public string GetId() => _id;
        public string GetDisplayName() => _displayName;
        public string GetUsername() => _username;
        public string GetAboutDesc() => _aboutDesc;
        public float GetRaiting() => _raiting;
        public ProfileAvatar GetPhotos() => _photos;

        public void SetId(string id) => _id = id;
        public void SetDisplayName(string displayname) => _displayName = displayname;
        public void SetUsername(string username) => _username = username;
        public void SetAboutDesc(string desc) => _aboutDesc = desc;
        public void SetRaiting(float raiting) => _raiting = raiting;
        public void SetPhotos(List<string> photos, int selected)
        {
            _photos = new ProfileAvatar(photos, selected);
        }

        public void AddPhoto(Texture2D photo)
        {

        }
    }

    [Serializable]
    public class ProfileAvatar
    {
        [SerializeField] List<string> _allPhotos;
        [SerializeField] int _selected;

        Color _avatarColor;

        public ProfileAvatar()
        {
            _allPhotos = new List<string>();
            _selected = -1;
            _avatarColor = GetRandomColor();
        }

        public ProfileAvatar(List<string> photos, int selected)
        {
            if (photos == null || photos.Count == 0)
            {
                _avatarColor = GetRandomColor();
                _selected = 0;
                return;
            }

            _allPhotos = new List<string>(photos);
            _selected = selected;
        }

        public Color GetAvatarColor() => _avatarColor;
        public IReadOnlyList<string> GetAllPhotos() => _allPhotos;
        public int GetSelectedIndex() => _selected;
        public string GetAvatarUrl()
        {
            if (_allPhotos.Count > 0 && _selected >= 0 && _selected < _allPhotos.Count)
            {
                return _allPhotos[_selected];
            }

            return string.Empty;
        }

        Color GetRandomColor()
        {
            System.Random rnd = new System.Random();
            float r = (float)rnd.NextDouble();
            float g = (float)rnd.NextDouble();
            float b = (float)rnd.NextDouble();

            return new Color(r, g, b);
        }
    }
}
