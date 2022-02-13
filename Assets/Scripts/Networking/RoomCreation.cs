using eeGames.Widget;
using Photon.Pun;
using Photon.Realtime;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class RoomCreation : MonoBehaviourPunCallbacks
{
    [SerializeField]
    InputField nickname;
    public GameObject cube;
    [SerializeField]
    Button joinRoomButton;
    private void Awake()
    {
        PhotonNetwork.ConnectUsingSettings();
        PhotonNetwork.AutomaticallySyncScene = true;
    }

    private void Start()
    {
        GameEvents.instance.onClickJoinRoom += TryJoinRoom;
    }

    private void OnDestroy()
    {
        GameEvents.instance.onClickJoinRoom -= TryJoinRoom;
    }


    public override void OnConnectedToMaster()
    {
        Debug.Log("OnConnectedToMaster() was called by PUN.");
        PhotonNetwork.JoinLobby();
    }

    public override void OnJoinedLobby()
    {
        base.OnJoinedLobby();
        PhotonNetwork.LocalPlayer.NickName = "Player" + Random.Range(500, 1000);
        GameEvents.instance.OnPlayerConnectedToServer();
    }


    [ContextMenu("Join Room")]
    public void TryJoinRoom() //Gets called by Join Button inside Main Menu.
    {
        Debug.Log("TryJoinRoom");
        PhotonNetwork.JoinRandomRoom();
    }

    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        // If Join Random Failed, which means the game room is full. So, the user will continue to create new Game Room!
        RoomOptions room = new RoomOptions() { MaxPlayers = 2 };
        PhotonNetwork.CreateRoom("Room" + Random.Range(10000, 100000), room);
    }

    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        base.OnCreateRoomFailed(returnCode, message);
        Debug.Log("Failed to create room!");
        TryJoinRoom();
    }

    public override void OnJoinedRoom()
    {
        Debug.Log("OnJoinedRoom");
        Debug.Log("We are in the room and we will be waiting for other player!");

        GameEvents.instance.OnPlayerJoinedRoom();
        PhotonHelper.SetPlayerCustomProperty(PhotonNetwork.LocalPlayer, PropertiesData.SelectedPlayer, DataHandler.instance.GetSelectedCharacter());


        if(PhotonNetwork.IsMasterClient) // Set Match Start Timer Value
        {
            PhotonHelper.SetRoomCustomProperty<double>(PropertiesData.MatchStartTime, -1);
        }
        else if (!PhotonNetwork.IsMasterClient) // Turn off room visibility when 2nd player joins room. So, no one else can join the room.
        {
            PhotonNetwork.CurrentRoom.IsVisible = false;
            PhotonNetwork.CurrentRoom.IsOpen = false;
        }
    }
}
