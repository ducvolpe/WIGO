using System.Collections.Generic;
using UnityEngine;
using WIGO.Userinterface;

namespace WIGO.Core
{
    [System.Serializable]
    public class EventCard
    {
        [SerializeField] string _id;
        [SerializeField] string _userId;
        [TextArea]
        [SerializeField] string _description;
        [SerializeField] List<string> _hashtags;
        [SerializeField] string _videoPath;
        [SerializeField] float _videoAspect;
        [SerializeField] int _distanceTime;
        [SerializeField] int _remainingTime;
        [SerializeField] int _participants;
        [SerializeField] string _location;

        public string GetId() => _id;
        // [TODO]: replace later
        public string GetUser() => _userId;
        public string GetDescription() => _description;
        public IReadOnlyList<string> GetHashtags() => _hashtags;
        public string GetVideoPath() => _videoPath;
        public float GetVideoAspect() => _videoAspect;
        public int CalculateDistanceTime() => _distanceTime;
        public int GetRemainingTime() => _remainingTime;
        public EventGroupSizeType GetGroupSizeType() => _participants > 1 ? EventGroupSizeType.Group : EventGroupSizeType.Single;
        public string GetLocation() => _location;
        public bool HasCategory(EventCategory category)
        {
            return _hashtags.Exists(x => string.Compare(x, category.ToString()) == 0);
        }

        public static EventCard CreateEmpty()
        {
            var empty = new EventCard();
            empty.SetParams("2un0jt746qkp98", 4);
            return empty;
        }

        public void SetParams(string userId, int participants)
        {
            _id = "82gam325skweh9";
            _userId = userId;
            _participants = participants;
        }
    }

    public enum EventGroupSizeType
    {
        None,
        Single,
        Group
    }

    public enum EventGenderType
    {
        Any,
        Male,
        Female
    }
}
