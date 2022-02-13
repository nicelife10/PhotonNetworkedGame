using eeGames.Widget;
using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public enum GameState
{
    InitialCountDown,
    GamePlay,
    GameComplete
}
public enum FightOutcome
{
    Win,
    Lose,
    Draw
}


public class GameSetup : MonoBehaviour
{
    public static GameSetup instance;

    public GameState GameState;
    public FightOutcome FightOutcome;
    [Space(10)]
    [Header("Player References")]
    public PhotonPlayer LocalPlayer;

    public GameObject MasterPlayer;
    public GameObject ClientPlayer;

    [Space(10)]
    [Header("Spawn Points")]
    public Transform MasterSpawnPoint;
    public Transform ClientSpawnPoint;

    [Space(10)]
    [Header("Time Elapsed")]
    public double TimeElapsed;


    GameTimeExtention GameTimeExtention;
    private void OnEnable()
    {
        if (instance == null)
            instance = this;

        WidgetManager.Instance.UnWindStack();
        WidgetManager.Instance.Push(WidgetName.InGameUIPanel);
    }

    // Start is called before the first frame update
    void Start()
    {
        GameState = GameState.InitialCountDown;
        GameEvents.instance.onInstantiateNetworkedPlayerAvatarEvents += SpawnPlayer;
        GameEvents.instance.onCountDownEndEvents += SetGamePlayState;
        GameEvents.instance.onGameTimerCompleteEvents += SetGameEndState;

        GameTimeExtention = new GameTimeExtention(this);
    }

    private void OnDestroy()
    {
        GameEvents.instance.onInstantiateNetworkedPlayerAvatarEvents -= SpawnPlayer;
        GameEvents.instance.onCountDownEndEvents -= SetGamePlayState;
        GameEvents.instance.onGameTimerCompleteEvents -= SetGameEndState;
    }

    void SpawnPlayer()
    {
        Debug.LogError("SpawnPlayer");
        int selected = PhotonHelper.GetPlayerCustomProperty(PhotonNetwork.LocalPlayer, PropertiesData.SelectedPlayer, -1);
        if (selected == -1)
            Debug.LogError("Player Custom Property Not set!");
        else
        {
            if (PhotonNetwork.IsMasterClient)
                PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs", "Player" + selected), MasterSpawnPoint.position, Quaternion.identity);
            else
                PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs", "Player" + selected), ClientSpawnPoint.position, Quaternion.identity);
        }

        Invoke(nameof(MasterSetStartTimer), 2f);
    }

    void MasterSetStartTimer()
    {
        if (PhotonNetwork.IsMasterClient) // Trigger Match Start Timer
        {
            PhotonHelper.SetRoomCustomProperty<double>(PropertiesData.MatchStartTime, PhotonNetwork.Time); // Game is started when the Start Timer is sent to both the players! from OnPropertiesUpdate inside NetworkingManager
        }
    }

    private void Update()
    {
        if (GameTimeExtention != null && GameState != GameState.GameComplete)
        {
            GameTimeExtention?.Tick();
            TimeElapsed = GameTimeExtention.TimeElapsed;
        }

        if(GameState == GameState.GamePlay)
        {
            if (LocalPlayer.GetOpponentHealth() < 1 || LocalPlayer.GetLocalPlayerHealth() < 1)// if opponents or my health is less then 1
            {
                SetGameEndState();
            }
        }
    }
    public FightOutcome GetFightOutCome()
    {
        if(GameTimeExtention.isGameEndCountDownReached)
        {
            Debug.LogError("Draw");
            return FightOutcome.Draw;
        }
        else if(LocalPlayer.GetLocalPlayerHealth() > LocalPlayer.GetOpponentHealth())
        {
            Debug.LogError("Win");
            return FightOutcome.Win;
        }
        else
        {
            Debug.LogError("Lose");
            return FightOutcome.Lose;
        }
    }


    public void SetGamePlayState()
    {
        Debug.Log("SetGamePlayState");
        OnGameStateChanged(GameState.GamePlay);
    }
    public void SetGameEndState() // this is final nail in coffin xD
    {
        OnGameStateChanged(GameState.GameComplete);
    }
    public void OnGameStateChanged(GameState state)
    {
        GameState = state;
        switch (state)
        {
            case GameState.InitialCountDown:
                break;
            case GameState.GamePlay:
                break;
            case GameState.GameComplete:
                GameEvents.instance.OnFightOutcome(GetFightOutCome());
                break;
        }
    }
}

public class GameTimeExtention
{
    public GameSetup Gamesetup;
    int syncWaitTime = 2;
    int CountDownTime = 3;
    int GameCompleteTime = 60;// combination of  = syncWaitTime + CountDownTime;

    public GameTimeExtention(GameSetup gs)
    {
        Gamesetup = gs;
    }

    bool CanStartGameTimer()
    {
        if (PhotonHelper.GetRoomCustomProperty<double>(PropertiesData.MatchStartTime, -1) != -1)
        {
            return (PhotonNetwork.Time - PhotonHelper.GetRoomCustomProperty<double>(PropertiesData.MatchStartTime, -1) > 2f);
        }
        return false;
    }
    public void Tick()
    {
        if (Gamesetup.GameState == GameState.GameComplete)
            return;

        if (CanStartGameTimer())
            CountDown();
    }


    double MatchStartTime => PhotonHelper.GetRoomCustomProperty<double>(PropertiesData.MatchStartTime, -1);
    public double TimeElapsed;
    bool isCountDownStarted = false;
    bool isGameCountDownStarted = false;
    public bool isGameEndCountDownReached { get; private set; } = false;

    bool waitForPlayersToSyncStartTime => TimeElapsed > syncWaitTime && !isCountDownStarted;
    bool waitForGameStart321CountDownToFinish => TimeElapsed > ((syncWaitTime + CountDownTime)) && !isGameEndCountDownReached;
    bool waitForGameCountDownToEnd => TimeElapsed > ((syncWaitTime + CountDownTime + GameCompleteTime) + 1) && !isGameEndCountDownReached;
    void CountDown()
    {
        TimeElapsed = (PhotonNetwork.Time - MatchStartTime);
        if (waitForGameCountDownToEnd) // Game Ends Here //set 60f this to 60f
        {
            isGameEndCountDownReached = true;
            GameEvents.instance.OnGameTimerCompleteEvents();
            Debug.Log("Timer - Now End GamePlay Timer Herer");
        }
        else if (waitForGameStart321CountDownToFinish) //set 20f this to 4.9f
        {
            if (!isGameCountDownStarted)
            {
                isGameCountDownStarted = true;
                GameEvents.instance.OnCountDownEndEvents();
                Debug.Log("Timer - Now Start GamePlay Timer");
            }
            GameEvents.instance.OnUpdateGamePlayTimer( ((GameCompleteTime)) +  - (((int)TimeElapsed) - ((syncWaitTime + CountDownTime))));
            //            GameEvents.instance.OnCountDownStartEvents();
        }
        else if (waitForPlayersToSyncStartTime) // set 10f this to 2f
        {
            isCountDownStarted = true;
            GameEvents.instance.OnCountDownStartEvents();
            Debug.Log("Timer - Now Start Game Count Down Timer");
        }
    }
}