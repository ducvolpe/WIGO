using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Threading.Tasks;
using System.Linq;
using WIGO.RecyclableScroll;
using System;
using WIGO.Core;

namespace WIGO.Userinterface
{
    public class UIChatInfo : ContactInfo
    {
        public Action onInfoLoaded;

        ChatData _chatData;
        UserProfile _profile;
        Action<ChatData, UserProfile> _onSelectChat;

        bool _loaded;

        public UIChatInfo(ChatData data, Action<ChatData, UserProfile> onSelectChat)
        {
            _chatData = data;
            _onSelectChat = onSelectChat;
            SetupUser(data.GetUserId());
        }

        public ChatData GetData() => _chatData;
        public UserProfile GetProfile() => _profile;
        public bool IsLoaded() => _loaded;

        public async Task SetChatMuted()
        {
            bool status = !_chatData.IsMuted();
            
            await Task.Delay(200);
            _chatData.ChangeMuteStatus(status);
        }

        public void RaiseSelectCallback() => _onSelectChat?.Invoke(_chatData, _profile);

        async void SetupUser(string id)
        {
            await Task.Delay(1000);

            _loaded = true;
            System.Random rnd = new System.Random();
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
            int length = UnityEngine.Random.Range(4, 16);

            var name = new string(Enumerable.Repeat(chars, length)
                .Select(s => s[rnd.Next(s.Length)]).ToArray());

            _profile = new UserProfile();
            _profile.SetUsername(name);
            onInfoLoaded?.Invoke();
        }
    }

    public class UIChatElement : MonoBehaviour, ICell<UIChatInfo>
    {
        [SerializeField] UserProfileElement _profilePhoto;
        [SerializeField] TMP_Text _nameLabel;
        [SerializeField] TMP_Text _messageLabel;
        [SerializeField] TMP_Text _timeLabel;
        [SerializeField] Image _lastMessageStatus;
        [SerializeField] GameObject _notificator;
        [SerializeField] TMP_Text _notificatorCounter;
        [SerializeField] GameObject _muteIcon;
        [SerializeField] Sprite[] _statusIcons;
        [Space]
        [SerializeField] GameObject _infoBlock;
        [SerializeField] GameObject _templateBlock;
        [SerializeField] UIChatHorizontalMoveHandler _moveHandler;
        [Space]
        [SerializeField] Image _muteButtonIcon;
        [SerializeField] TMP_Text _muteLabel;
        [SerializeField] Sprite[] _muteSprites;

        int _index;
        UIChatInfo _info;

        public int GetIndex() => _index;
        public UIChatInfo GetInfo() => _info;

        public void InitCell(Action<ICell<UIChatInfo>> onClick) { }
        public void Init(ScrollRect mainScroll)
        {
            _moveHandler.Init(mainScroll, () => _info.RaiseSelectCallback());
        }

        public Task ConfigureCell(UIChatInfo contactInfo, int cellIndex)
        {
            if (_info != null)
            {
                _info.onInfoLoaded -= SetupInfo;
            }

            _info = contactInfo;
            _index = cellIndex;
            _moveHandler.ResetPosition();

            if (_info.IsLoaded())
            {
                SetupInfo();
            }
            else
            {
                _infoBlock.SetActive(false);
                _templateBlock.SetActive(true);
                _info.onInfoLoaded += SetupInfo;
            }

            return Task.CompletedTask;
        }

        public void OnDeleteButtonClick()
        {

        }

        public async void OnMuteButtonClick()
        {
            await _info.SetChatMuted();
            SetChatMuteVisual();
        }

        void SetupInfo()
        {
            var lastMessage = _info.GetData().GetLastMessage();

            SetLastMessageStatus(lastMessage.GetStatus());
            _nameLabel.text = _info.GetProfile().GetUsername();
            _nameLabel.rectTransform.sizeDelta = new Vector2(_nameLabel.preferredWidth, _nameLabel.rectTransform.sizeDelta.y);
            _profilePhoto.Setup(_info.GetProfile());
            _messageLabel.text = lastMessage.GetText();
            _timeLabel.text = lastMessage.GetSendTime();
            SetChatMuteVisual();

            _infoBlock.SetActive(true);
            _templateBlock.SetActive(false);
            _info.onInfoLoaded -= SetupInfo;
        }

        void SetLastMessageStatus(int status)
        {
            bool isMine = _info.GetData().GetLastMessage().IsMine();
            _lastMessageStatus.gameObject.SetActive(isMine);
            if (!isMine)
            {
                return;
            }

            switch (status)
            {
                case 0:
                    _lastMessageStatus.sprite = _statusIcons[0];
                    _lastMessageStatus.color = UIGameColors.MessageSentColor;
                    break;
                case 1:
                    _lastMessageStatus.sprite = _statusIcons[1];
                    _lastMessageStatus.color = UIGameColors.MessageSentColor;
                    break;
                case 2:
                    _lastMessageStatus.sprite = _statusIcons[1];
                    _lastMessageStatus.color = UIGameColors.Blue;
                    break;
                default:
                    _lastMessageStatus.sprite = _statusIcons[0];
                    _lastMessageStatus.color = UIGameColors.MessageSentColor;
                    break;
            }
        }

        void SetChatMuteVisual()
        {
            _muteIcon.SetActive(_info.GetData().IsMuted());
            _muteButtonIcon.sprite = _info.GetData().IsMuted() ? _muteSprites[0] : _muteSprites[1];
            _muteLabel.text = _info.GetData().IsMuted() ? "Unmute" : "Mute";
        }

        void OnDestroy()
        {
            if (_info != null)
            {
                _info.onInfoLoaded -= SetupInfo;
            }
        }
    }
}
