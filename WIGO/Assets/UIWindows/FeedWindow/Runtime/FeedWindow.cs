using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using WIGO.Core;

namespace WIGO.Userinterface
{
    public class FeedWindow : UIWindow
    {
        [SerializeField] UserProfile _myProfile;                            // temp
        [SerializeField] UserProfileWithTextElement _userProfileElement;
        [SerializeField] FiltersEventsController _filtersController;
        [SerializeField] UIEventCardElement _cardPrefab;
        [SerializeField] RectTransform _cardsdContent;
        [SerializeField] EndOfPostsController _endOfPostsController;
        [SerializeField] GameObject _loadingLabel;
        [SerializeField] EventCard[] _testCards;
        [SerializeField] bool _eventCreated;

        List<EventCard> _loadedCards = new List<EventCard>();
        UIEventCardElement _currentCard;
        CancellationTokenSource _cts;
        int _currentCardIndex;

        public override void OnOpen(WindowId previous)
        {
            RefreshFeed();
        }

        public override void OnClose(WindowId next, Action callback = null)
        {
            _currentCard?.Clear();
            _currentCard = null;
            _filtersController.ResetFilters();
            _currentCardIndex = 0;
            _endOfPostsController.Deactivate();
            _loadingLabel.SetActive(false);
            callback?.Invoke();
        }

        public void OnProfileClick()
        {
            //ServiceLocator.Get<UIManager>().Open<ProfileWindow>(WindowId.PROFILE_SCREEN, (window) => window.Setup(_myProfile));
        }

        public void OnChatsClick()
        {
            ServiceLocator.Get<UIManager>().Open<ChatsListWindow>(WindowId.CHATS_LIST_SCREEN);
        }

        public void OnSettingsClick()
        {
            ServiceLocator.Get<UIManager>().Open<SettingsWindow>(WindowId.SETTINGS_SCREEN);
        }

        public void OnCreateEventClick()
        {
            if (_eventCreated)
            {
                ServiceLocator.Get<UIManager>().Open<EventsRequestsWindow>(WindowId.EVENTS_REQUESTS_SCREEN);
            }
            else
            {
                ServiceLocator.Get<UIManager>().Open<ResponseInfoWindow>(WindowId.RESPONSE_INFO_SCREEN, (window) => window.Setup());
            }
        }

        public void OnRefreshFeedClick()
        {
            _endOfPostsController.Deactivate();
            RefreshFeed();
        }

        public void OnTurnOnNotificationsClick()
        {
            ServiceLocator.Get<UIManager>().Open<SettingsWindow>(WindowId.SETTINGS_SCREEN, (window) => window.Setup(new int[] { 0, 0 }));
        }

        public void OnFiltersOffClick()
        {
            _filtersController.ResetFilters();
            RefreshFeed();
        }

        protected override void Awake()
        {
            _filtersController.Initialize(OnApplyFilterCategory);
            //var allPhotos = _myProfile.GetPhotos().GetAllPhotos();
            //int index = _myProfile.GetPhotos().GetSelectedIndex();
            //_userProfileElement.Setup(allPhotos[index], _myProfile.GetDisplayName());
        }

        void OnApplyFilterCategory()
        {
            _cts?.Cancel();
            _currentCard?.Clear();
            _endOfPostsController.Deactivate();
            RefreshFeed();
        }

        async void RefreshFeed()
        {
            _currentCard?.Clear();
            _currentCard = null;
            _endOfPostsController.Deactivate();
            _loadingLabel.SetActive(true);
            _cts = new CancellationTokenSource();

            // fake loading posts
            var category = _filtersController.GetFilterCategory();
            await Task.Delay(600, _cts.Token);

            if (_cts.IsCancellationRequested)
            {
                return;
            }

            _cts = null;
            _loadingLabel.SetActive(false);
            _loadedCards = category == EventCategory.All
                ? new List<EventCard>(_testCards)
                : new List<EventCard>(Array.FindAll(_testCards, x => x.HasCategory(category)));
            _currentCardIndex = 0;

            if (_loadedCards.Count == 0)
            {
                SetEndOfPosts();
                return;
            }

            CreateNextCard();
        }

        void CreateNextCard()
        {
            if (_currentCardIndex >= _loadedCards.Count)
            {
                // [TODO]: add label
                _currentCard = null;
                SetEndOfPosts();
                return;
            }

            var card = _loadedCards[_currentCardIndex];
            _currentCardIndex++;

            _currentCard = Instantiate(_cardPrefab, _cardsdContent);
            _currentCard.Setup(card, OnCardSkip);
        }

        void OnCardSkip(bool accept)
        {
            if (accept)
            {
                ServiceLocator.Get<UIManager>().Open<ResponseInfoWindow>(WindowId.RESPONSE_INFO_SCREEN, (window) => window.Setup(true));
                return;
            }

            CreateNextCard();
        }

        void SetEndOfPosts()
        {
            var model = ServiceLocator.Get<GameModel>();

            bool hasMyEvent = model.HasMyOwnEvent();
            if (!hasMyEvent)
            {
                _endOfPostsController.Activate(EndOfPostsType.HaveNoMyEvent);
                return;
            }

            var notifications = ServiceLocator.Get<GameModel>().GetNotifications();
            if (!notifications.newEvent)
            {
                _endOfPostsController.Activate(EndOfPostsType.NotificationsOff);
                return;
            }

            if (_filtersController.FiltersApplied())
            {
                _endOfPostsController.Activate(EndOfPostsType.FiltersSearch);
                return;
            }

            _endOfPostsController.Activate(EndOfPostsType.EmptyFeed);
        }
    }
}
