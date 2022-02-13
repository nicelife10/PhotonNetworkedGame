using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class NetworkingManager : MonoBehaviourPunCallbacks
{
    public static NetworkingManager instance;
    PhotonView PV;

    private float waitTimer = 10f;
    private int playersInGame = 0;

    #region MonobehaviourCallbacks
    private void Awake()
    {
        if(instance == null)
        {
            PV = gameObject.GetComponent<PhotonView>();
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
    // called first
    public override void OnEnable()
    {
        base.OnEnable();
        Debug.Log("OnEnable called");
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void Start()
    {
    }

    public override void OnDisable()
    {
        base.OnDisable();
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if(scene.buildIndex == 0)
        {
            playersInGame = 0; // resetting it! should be actually inside OnPlayerLeft!
        }

        if(scene.buildIndex == 1) // Game Scene
        {
            if (PhotonNetwork.IsMasterClient)
            {
                IncrementInGamePlayerCount();
            }
            else
            {
                PV.RPC(nameof(RPC_ClientLoadedGame), RpcTarget.MasterClient);
            }
        }
    }


    #endregion MonobehaviourCallbacks

    #region PhotonCallbacks
    public override void OnJoinedRoom()
    {
        base.OnJoinedRoom();

        if(PhotonNetwork.IsMasterClient)
        {
            StartCoroutine(nameof(WaitingForOpponent));
        }
    }
    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        base.OnPlayerEnteredRoom(newPlayer);
        StopCoroutine("WaitingForOpponent");

        if(PhotonNetwork.IsMasterClient) // Enter Game when Other(client) has joined the game
        {
            GameEvents.instance.nm_OnAnotherPlayerJoinedEvents(newPlayer.NickName + " Entered!");
            Invoke(nameof(Master_LoadGame), 2.5f);
        }
    }
    public override void OnPlayerPropertiesUpdate(Player targetPlayer, ExitGames.Client.Photon.Hashtable changedProps)
    {
        base.OnPlayerPropertiesUpdate(targetPlayer, changedProps);

        if (changedProps.ContainsKey(PropertiesData.Health)) // if Health Variable is changed
        {
            int updatedHealth = PhotonHelper.GetPlayerCustomProperty<int>(targetPlayer, PropertiesData.Health, 100);
            if(SceneManager.GetActiveScene().buildIndex == 1) // if in Game Scene
            {
                if(targetPlayer.IsMasterClient)
                {
                    GameEvents.instance.OnUpdateMasterHealth(updatedHealth);
                }
                else
                {
                    GameEvents.instance.OnUpdateClientHealth(updatedHealth);
                }
            }
        }
    }

    public override void OnRoomPropertiesUpdate(ExitGames.Client.Photon.Hashtable propertiesThatChanged)
    {
        base.OnRoomPropertiesUpdate(propertiesThatChanged);

        if(propertiesThatChanged.ContainsKey(PropertiesData.MatchStartTime))
        {
            if(SceneManager.GetActiveScene().buildIndex == 1)
            {
                if (PhotonHelper.GetRoomCustomProperty<double>(PropertiesData.MatchStartTime, -1f) != -1)
                {
//                    GameEvents.instance.OnCountDownStartEvents();
                }
            }
        }
    }


    #endregion PhotonCallbacks

    #region RPCs


    [PunRPC]
    void RPC_ClientLoadedGame()
    {
        IncrementInGamePlayerCount();
    }
    [PunRPC]
    void RPC_LoadPlayerAvatars()
    {
        GameEvents.instance.OnInstantiateNetworkedPlayerAvatar();
        Debug.LogError("Load Player Avatar");
    }
    #endregion RPCs

    #region HelpreMethods

    IEnumerator WaitingForOpponent()
    {
        waitTimer = 10f;
        while (true)
        {
            yield return new WaitForSecondsRealtime(1f);
            GameEvents.instance.nm_UpdateWaitTimer(waitTimer.ToString());
            waitTimer--;

            if (waitTimer < 1)
            {
                StopCoroutine("WaitingForOpponent");
                PhotonNetwork.LeaveRoom();
                GameEvents.instance.nm_OnLocalPlayerLeft();
            }
        }
    }

    void Master_LoadGame()
    {
        if(PhotonNetwork.IsMasterClient)
        {
            Debug.LogError("Master Load Scene 1");
            SceneManager.LoadScene(1); //automatically sync scene is set to true!
        }
    }

    void IncrementInGamePlayerCount()
    {
        playersInGame++;
        Debug.LogError("playersInGame: " + playersInGame);
        if (playersInGame == 2)
        {
            PV.RPC(nameof(RPC_LoadPlayerAvatars), RpcTarget.AllViaServer);
            Debug.LogError("All Players Are in Game Scene!");
        }
    }

    #endregion HelpreMethods

}


/* Zombie Code
     //Not In Use!
    IEnumerator CheckIfBothPlayersAreInGameSceneThenTriggerLoadAvatars()
    {
        int playersInGameScene = 0;
        while (playersInGameScene != 2)
        {
            yield return new WaitForSeconds(0.1f);
            playersInGameScene = 0;
            foreach (Player x in PhotonNetwork.PlayerList)
            {
                if(PhotonHelper.GetPlayerCustomProperty(x, PropertiesData.InGame, false) == true)
                {
                    playersInGameScene++;
                }
            }
            Debug.LogError("playersInGameScene: " + playersInGameScene);

            if (playersInGameScene == 2)
            {
                view.RPC(nameof(RPC_LoadPlayerAvatars), RpcTarget.AllViaServer);
                StopCoroutine("CheckIfBothPlayersAreInGameSceneThenTriggerLoadAvatars");
            }
        }
    } 

    void InstantiatePlayerAvatars_OnSceneLoaded(Scene scene)
    {
        PhotonHelper.SetPlayerCustomProperty(PhotonNetwork.LocalPlayer, PropertiesData.InGame, true);
    }
*/
