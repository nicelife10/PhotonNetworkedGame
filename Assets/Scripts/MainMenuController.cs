using eeGames.Widget;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenuController : MonoBehaviour
{
    public static MainMenuController instance;

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        Init();
    }

    private void Init()
    {
        ShowConnectingPanel();
        GameEvents.instance.onPlayerConnectedToServer += ShowMainMenu;
        GameEvents.instance.onPlayerJoinedRoom += ShowLaunchPanel;
    }

    private void OnDestroy()
    {
        GameEvents.instance.onPlayerConnectedToServer -= ShowMainMenu;
        GameEvents.instance.onPlayerJoinedRoom -= ShowLaunchPanel;
    }

    public void ShowMainMenu()
    {
        WidgetManager.Instance.Push(WidgetName.MainMenuPanel);
    }
    public void HideMainMenu()
    {
        WidgetManager.Instance.Pop(WidgetName.MainMenuPanel);
    }

    public void ShowConnectingPanel()
    {
        WidgetManager.Instance.Push(WidgetName.ConnectingPanel);
    }
    public void HideConnectingPanel()
    {
        WidgetManager.Instance.Pop(WidgetName.ConnectingPanel);
    }

    public void ShowLaunchPanel()
    {
        WidgetManager.Instance.Push(WidgetName.LaunchPanel);
    }
    public void HideLaunchPanel()
    {
        WidgetManager.Instance.Pop(WidgetName.LaunchPanel);
    }
}
