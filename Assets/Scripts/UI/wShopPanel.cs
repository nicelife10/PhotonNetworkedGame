using eeGames.Widget;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class wShopPanel : Widget
{
    public Button PlayerSelect_1;
    public Button PlayerSelect_2;
    public Button PlayerSelect_3;
    public Button PlayerSelect_4;
    public Button PlayerSelect_5;
    public Button PlayerSelect_6;

    public Button ProceedButton; // goes back to menu!


    delegate void playeSwitcher(int i);
    // Start is called before the first frame update
    void Start()
    {
        PlayerSelect_1.onClick.AddListener(delegate { OnClickSelectPlayer(0); });
        PlayerSelect_2.onClick.AddListener(delegate { OnClickSelectPlayer(1); });
        PlayerSelect_3.onClick.AddListener(delegate { OnClickSelectPlayer(2); });
        PlayerSelect_4.onClick.AddListener(delegate { OnClickSelectPlayer(3); });
        PlayerSelect_5.onClick.AddListener(delegate { OnClickSelectPlayer(4); });
        PlayerSelect_6.onClick.AddListener(delegate { OnClickSelectPlayer(5); });

        ProceedButton.onClick.AddListener(OnClikProceed);
    }

    private void OnDestroy()
    {
        PlayerSelect_1.onClick.RemoveAllListeners();
        PlayerSelect_2.onClick.RemoveAllListeners();
        PlayerSelect_3.onClick.RemoveAllListeners();
        PlayerSelect_4.onClick.RemoveAllListeners();
        PlayerSelect_5.onClick.RemoveAllListeners();
        PlayerSelect_6.onClick.RemoveAllListeners();

        ProceedButton.onClick.RemoveAllListeners();
    }

    public void OnClickSelectPlayer(int i)
    {
        GameEvents.instance.OnSelectPlayerEvents(i);
    }
    public void OnClikProceed()
    {
        WidgetManager.Instance.Pop(WidgetName.ShopPanel);
    }
}
