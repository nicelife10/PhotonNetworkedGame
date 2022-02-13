using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhotonPlayer : MonoBehaviour
{
    private PhotonView PV;

    public int Health = 100;
    // Start is called before the first frame update
    void Start()
    {
        Debug.LogError("isMaster: " + PhotonNetwork.IsMasterClient);

        PV = GetComponent<PhotonView>();

        if(PV.Owner.IsLocal)
            GameSetup.instance.LocalPlayer = this;

        if (PV.Owner.IsMasterClient)
        {
            GameSetup.instance.MasterPlayer = gameObject;
        }
        else
        {
            GameSetup.instance.ClientPlayer = gameObject;
        }
    }

    private void Update()
    {
        if (!PV.IsMine)
            return;

        if (GameSetup.instance.GameState != GameState.GamePlay)
            return;

        if(Input.GetKeyDown(KeyCode.Space))
        {
            HitOpponentPlayer();
        }
    }


    public void HitOpponentPlayer()
    {
        foreach(Player x in PhotonNetwork.PlayerList)
        {
            if (!x.IsLocal)
                PhotonHelper.SetPlayerCustomProperty<int>(x, 
                    PropertiesData.Health, 
                    PhotonHelper.GetPlayerCustomProperty<int>(x, PropertiesData.Health, 100) - 5);
        }
    }

    public int GetLocalPlayerHealth()
    {
        return PhotonHelper.GetPlayerCustomProperty<int>(PhotonNetwork.LocalPlayer, PropertiesData.Health, 100);
    }

    public int GetOpponentHealth()
    {
        foreach (Player x in PhotonNetwork.PlayerList)
        {
            if (!x.IsLocal)
              return  PhotonHelper.GetPlayerCustomProperty<int>(x, PropertiesData.Health, 100);
        }
        return 100;
    }

    public void onGetHit(int i)
    {

    }
}
