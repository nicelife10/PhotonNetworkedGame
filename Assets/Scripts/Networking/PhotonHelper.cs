using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class PhotonHelper
{
    private static ExitGames.Client.Photon.Hashtable tempHashtable = new ExitGames.Client.Photon.Hashtable();

    #region Player_Custom_Properties_Getter
    public static T GetPlayerCustomProperty<T>(PhotonView view, string property, T defaultValue)
    {
        if (view != null && view.Owner != null && view.Owner.CustomProperties.ContainsKey(property) == true)
        {
            return (T)view.Owner.CustomProperties[property];
        }
        return defaultValue;
    }
    public static T GetPlayerCustomProperty<T>(Player player, string property, T defaultValue)
    {
        if (player != null && player.CustomProperties.ContainsKey(property) == true)
        {
            return (T)player.CustomProperties[property];
        }
        return defaultValue;
    }
    #endregion Player_Custom_Properties_Getter

    #region Player_Custom_Properties_Setter
    public static void SetPlayerCustomProperty<T>(PhotonView view, string property, T value)
    {
        SetPlayerProperty(view.Owner, property, (object)value);
    }
    public static void SetPlayerCustomProperty<T>(Player player, string property, T value)
    {
        SetPlayerProperty(player, property, (object)value);
    }
    #endregion Player_Custom_Properties_Setter

    #region Room_CustomProperties
    public static void SetRoomCustomProperty<T>(string property, T value)
    {
        SetRoomProperty(PhotonNetwork.CurrentRoom, property, value);
    }

    public static T GetRoomCustomProperty<T>(string property, T defaultValue)
    {
        if (PhotonNetwork.CurrentRoom.CustomProperties.ContainsKey(property) == true)
        {
            return (T)PhotonNetwork.CurrentRoom.CustomProperties[property];
        }
        else
        {
            Debug.LogError("Property was not set!!! default value returned!");
            return defaultValue;
        }
    }



    #endregion Room_CustomProperties Getter






    #region Helpers
    private static void SetPlayerProperty(Player player, string key, object value)
    {
        tempHashtable.Clear();
        tempHashtable.Add(key, value);
        player.SetCustomProperties(tempHashtable);
    }
    private static void SetRoomProperty(Room room, string key, object value)
    {        
        tempHashtable.Clear();
        tempHashtable.Add(key, value);
        PhotonNetwork.CurrentRoom.SetCustomProperties(tempHashtable);
    }
    #endregion Helpers
}
