using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameEvents : MonoBehaviour
{
    public static GameEvents instance;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    #region RoomCreationEvents
    public event Action onPlayerConnectedToServer;
    public event Action onClickJoinRoom;
    public event Action onPlayerJoinedRoom;
    public event Action onJoinedRoomFailed;
    public void OnPlayerConnectedToServer()
    {
        onPlayerConnectedToServer?.Invoke();
    }
    public void OnClickJoinRoom()
    {
        onClickJoinRoom?.Invoke();
    }

    public void OnPlayerJoinedRoom()
    {
        onPlayerJoinedRoom?.Invoke();
    }

    public void OnJoinedRoomFailed()
    {
        onJoinedRoomFailed?.Invoke();
    }

    #endregion RoomCreationEvents

    #region NetworkingManagerEvents
    public event Action<string> nm_onAnotherPlayerJoinedEvents;
    public event Action<string> nm_updateWaitTimer;

    public event Action nm_onLocalPlayerLeft;
    public event Action nm_onAnotherPlayerLeft;
    public void nm_OnAnotherPlayerJoinedEvents(string text)
    {
        if (nm_onAnotherPlayerJoinedEvents != null)
            nm_onAnotherPlayerJoinedEvents.Invoke(text);
    }
    public void nm_UpdateWaitTimer(string text)
    {
        if (nm_updateWaitTimer != null)
            nm_updateWaitTimer.Invoke(text);
    }

    public void nm_OnLocalPlayerLeft()
    {
        nm_onLocalPlayerLeft?.Invoke();
    }


    public void nm_OnAnotherPlayerLeft()
    {
        nm_onAnotherPlayerLeft?.Invoke();
    }
    #endregion NetworkingManagerEvents

    #region MainMenuEvents
    public event Action<int> onSelectPlayerEvents;
    public void OnSelectPlayerEvents(int i)
    {
        onSelectPlayerEvents?.Invoke(i);
    }
    #endregion MainMenuEvents

    #region GamePlayEvents
    public event Action onInstantiateNetworkedPlayerAvatarEvents;
    public void OnInstantiateNetworkedPlayerAvatar()
    {
        onInstantiateNetworkedPlayerAvatarEvents?.Invoke();
        Debug.LogError("Players Avatar Got instantaited!");
    }

    public event Action onCountDownStartEvents;
    public void OnCountDownStartEvents()
    {
        onCountDownStartEvents?.Invoke();
    }

    public event Action onCountDownEndEvents;
    public void OnCountDownEndEvents()
    {
        onCountDownEndEvents?.Invoke();
    }

    public event Action<int> onUpdateGamePlayTimer;
    public void OnUpdateGamePlayTimer(int val)
    {
        onUpdateGamePlayTimer?.Invoke(val);
    }
    public event Action onGameTimerCompleteEvents;
    public void OnGameTimerCompleteEvents()
    {
        onGameTimerCompleteEvents?.Invoke();
    }

    public event Action<int> onUpdateMasterHealth;
    public void OnUpdateMasterHealth(int i)
    {
        onUpdateMasterHealth?.Invoke(i);
    }
    public event Action<int> onUpdateClientHealth;
    public void OnUpdateClientHealth(int i)
    {
        onUpdateClientHealth?.Invoke(i);
    }

    public event Action<FightOutcome> onFightOutcomeEvents;

    public void OnFightOutcome(FightOutcome val)
    {
        onFightOutcomeEvents?.Invoke(val);
    }
    #endregion GamePlayEvents

}
