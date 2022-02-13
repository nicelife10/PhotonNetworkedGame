using eeGames.Widget;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class wMainMenuPanel : Widget
{
    public Button JoinGameButton;
    public Button ShopButton;
    protected override void Awake()
    {
        base.Awake();
    }

    private void OnEnable()
    {
        EnableJoinButton();
    }

    private void Start()
    {
        JoinGameButton.onClick?.AddListener(GameEvents.instance.OnClickJoinRoom);
        JoinGameButton.onClick?.AddListener(DisableJoinButton);
        ShopButton.onClick?.AddListener(OnClickShop);
        GameEvents.instance.onJoinedRoomFailed += EnableJoinButton;
        GameEvents.instance.nm_onLocalPlayerLeft += EnableJoinButton;
    }


    void OnDestroy()
    {
        JoinGameButton.onClick?.RemoveListener(GameEvents.instance.OnClickJoinRoom);
        JoinGameButton.onClick?.RemoveListener(DisableJoinButton);
        ShopButton.onClick?.RemoveListener(OnClickShop);
        GameEvents.instance.onJoinedRoomFailed -= EnableJoinButton;
        GameEvents.instance.nm_onLocalPlayerLeft -= EnableJoinButton;
        base.DestroyWidget();
    }

    void EnableJoinButton()
    {
        JoinGameButton.interactable = true;
    }
    void DisableJoinButton()
    {
        JoinGameButton.interactable = false;
    }

    void OnClickShop()
    {
        WidgetManager.Instance.Push(WidgetName.ShopPanel);
    }
}
