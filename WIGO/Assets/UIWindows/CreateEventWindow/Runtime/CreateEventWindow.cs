using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using WIGO.Core;

namespace WIGO.Userinterface
{
    public class CreateEventWindow : EventInfoWindow
    {
        [SerializeField] CreateEventView _view;
        [SerializeField] HashtagDoubleScroll _hashtagScroll;

        EventGenderType _selectedGender = EventGenderType.Any;
        EventGroupSizeType _selectedSizeType = EventGroupSizeType.None;
        bool _locationSelected;

        public override void OnReopen(WindowId previous, UIWindowModel cachedModel)
        {
            _animator.OnReopen();
        }

        public override void CloseUnactive()
        {
            ClearWindow();
        }

        public override void Setup(string path)
        {
            base.Setup(path);
            _hashtagScroll.CreateHashtags(CheckIfAvailable);
        }

        public void OnSelectGender(int index)
        {
            int prevIndex = (int)_selectedGender;
            if (index == prevIndex)
            {
                return;
            }

            _selectedGender = (EventGenderType)index;
            _view.SelectGender(prevIndex, index);
            CheckIfAvailable();
        }

        public void OnSelectGroupSize(int index)
        {
            int prevIndex = (int)_selectedSizeType;
            if (index == prevIndex)
            {
                return;
            }

            _selectedSizeType = (EventGroupSizeType)index;
            _view.SelectGroupSize(prevIndex, index);
            CheckIfAvailable();
        }

        public override async void OnPublishClick()
        {
            if (IsAvailable())
            {
                var myEvent = await CreateEventOrResponse();
                if (myEvent != null)
                    ServiceLocator.Get<GameModel>().SetMyEvent(myEvent);

                await Task.Delay(1200);
                ServiceLocator.Get<UIManager>().SwitchTo(WindowId.FEED_SCREEN);
            }
        }

        public void OnLocationSelectClick()
        {
            //ServiceLocator.Get<UIManager>().Open<LocationSelectWindow>(WindowId.LOCATION_SELECT_SCREEN, (window) => window.Setup(OnLocationSelected));
            OnLocationSelected(true);
        }

        public void OnEndEditDesc(string text)
        {
            CheckIfAvailable();
        }

        protected override void ClearWindow()
        {
            base.ClearWindow();
            _hashtagScroll.Clear();
            _view.ResetView(_selectedGender, _selectedSizeType);
            _selectedSizeType = EventGroupSizeType.None;
            _selectedGender = EventGenderType.Any;
        }

        protected override bool IsAvailable()
        {
            int categoriesCount = _hashtagScroll.GetSelectedCategories().Count();
            return _locationSelected && !string.IsNullOrEmpty(_descIF.text) && categoriesCount > 0 && _selectedSizeType != EventGroupSizeType.None;
        }

        void OnLocationSelected(bool value)
        {
            _locationSelected = value;
            _view.SetLocation("Hookah bar LaBrume", "Nizhny Susalny lane, 5c10");
            CheckIfAvailable();
        }
    }
}
