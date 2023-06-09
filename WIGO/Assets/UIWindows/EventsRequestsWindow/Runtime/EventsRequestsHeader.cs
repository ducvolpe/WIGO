using System;
using TMPro;
using UnityEngine;
using DG.Tweening;

namespace WIGO.Userinterface
{
    public class EventsRequestsHeader : MonoBehaviour
    {
        private enum HeaderCategory
        {
            Events,
            Requests
        }

        [SerializeField] TMP_Text _eventsLabel;
        [SerializeField] TMP_Text _requestsLabel;
        [SerializeField] GameObject _requestsCounter;
        [SerializeField] TMP_Text _counterLabel;

        HeaderCategory _currentCategory = HeaderCategory.Events;
        Action<int> _onCategorySelect;
        Sequence _switchSequence;

        public void Initialize(Action<int> onCategorySelect)
        {
            _onCategorySelect = onCategorySelect;
        }

        public void SetupRequestsCount(int count)
        {
            if (count < 1)
            {
                _requestsCounter.SetActive(false);
                return;
            }

            _requestsCounter.SetActive(true);
            _counterLabel.text = count.ToString();
        }

        public void OnChangeCategory(int index)
        {
            int category = (int)_currentCategory;
            if (index == category)
            {
                return;
            }

            _onCategorySelect?.Invoke(index);
        }

        public void ChangeCategory(int category)
        {
            CancelSwitch();

            _currentCategory = (HeaderCategory)category;

            float eventsAlpha = _currentCategory == HeaderCategory.Events ? 1f : 0.5f;
            float requestsAlpha = _currentCategory == HeaderCategory.Requests ? 1f : 0.5f;
            _switchSequence = DOTween.Sequence();
            _switchSequence.Append(_eventsLabel.DOFade(eventsAlpha, 0.2f))
                .Join(_requestsLabel.DOFade(requestsAlpha, 0.2f))
                .OnComplete(() => _switchSequence = null);
        }

        public void ResetHeader()
        {
            _currentCategory = HeaderCategory.Events;
            UIGameColors.SetTransparent(_eventsLabel, 1f);
            UIGameColors.SetTransparent(_requestsLabel, 0.5f);
        }

        void CancelSwitch()
        {
            if (_switchSequence != null)
            {
                _switchSequence.Kill();
                _switchSequence = null;
            }
        }
    }
}
