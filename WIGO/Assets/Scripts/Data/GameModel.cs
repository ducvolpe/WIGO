using System.Threading.Tasks;
using UnityEngine;
using WIGO;
using WIGO.Core;

[System.Serializable]
public class GameModel
{
    [SerializeField] string _ltoken;
    [SerializeField] NotificationSettings _notifications;

    ProfileData _myProfile;
    EventCard _myEvent;

    public string GetRegisterToken() => _ltoken;
    public NotificationSettings GetNotifications() => _notifications;
    public bool HasMyOwnEvent() => _myEvent != null;

    public void SaveLongToken(string token)
    {
        _ltoken = token;
        SaveData();
    }

    public void SaveNotifications(NotificationSettings settings)
    {
        _notifications = settings;
        SaveData();
    }

    public void SetMyEvent(EventCard card)
    {
        _myEvent = card;
    }

    public async Task<bool> TryLogin()
    {
        if (string.IsNullOrEmpty(_ltoken))
        {
            return false;
        }

        _myProfile = await NetService.TryLogin(_ltoken);
        return _myProfile != null;
    }

    void SaveData()
    {
        string saveData = JsonReader.Serialize(this);
        PlayerPrefs.SetString("SaveData", saveData);
    }
}

[System.Serializable]
public struct NotificationSettings
{
    public bool responses;
    public bool newMessages;
    public bool newEvent;
    public bool expireEvent;
    public bool areYouOK;
    public bool estimate;
}
