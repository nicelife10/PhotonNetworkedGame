using System.Collections;
using System.Collections.Generic;
using UnityEngine;




[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/GameData", order = 1)]
public class GameData : ScriptableObject
{

    [SerializeField]
    private int CurrentSelectedCharacter;
    public PlayerInfo[] AvailalePlayers;

    private void OnEnable()
    {
//        GameEvents.instance.onSelectPlayerEvents += SetSelectedPlayer;
        Debug.Log("scriptable object enabled");
    }

    private void OnDisable()
    {
        Debug.Log("scriptable object disabled");
        //GameEvents.instance.onSelectPlayerEvents -= SetSelectedPlayer;
    }

    public void SetSelectedPlayer(int i)
    {
        CurrentSelectedCharacter = i;
    }
    public int GetSelectedPlayer()
    {
        return CurrentSelectedCharacter;
    }


    [System.Serializable]
public struct PlayerInfo
{
    public int PlayerID;
    public string PlayerName;
    //other player related Data
}
}