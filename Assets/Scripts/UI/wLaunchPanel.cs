using eeGames.Widget;
using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class wLaunchPanel : Widget
{
    public TextMeshProUGUI statusText;
    public TextMeshProUGUI ClientTypeText;
    public TextMeshProUGUI RemainingTimeText;

    protected override void Awake()
    {
        base.Awake();
        base.OnHide.AddListener(OnDestroy);
    }

    public void Start()
    {
        SetClientTypeText();

        GameEvents.instance.nm_onAnotherPlayerJoinedEvents += SetStatusText;
        GameEvents.instance.nm_updateWaitTimer += setRemainingTimeText;
    }

    void OnDestroy()
    {
        GameEvents.instance.nm_onAnotherPlayerJoinedEvents -= SetStatusText;
        GameEvents.instance.nm_updateWaitTimer -= setRemainingTimeText;
        base.OnHide.RemoveListener(OnDestroy);
        base.DestroyWidget();
    }

    public void SetStatusText(string text)
    {
        statusText.text = text;
    }

    public void SetClientTypeText()
    {
        if(PhotonNetwork.IsConnected)
        {
            if (PhotonNetwork.IsMasterClient) 
                ClientTypeText.text = "Master";
            else ClientTypeText.text = "Client";
        }
    }
    public void setRemainingTimeText(string text)
    {
        RemainingTimeText.text = text;
    }
}
