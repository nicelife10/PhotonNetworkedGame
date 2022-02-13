using eeGames.Widget;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class wConnectingPanel : Widget
{
    protected override void Awake()
    {
        base.Awake();
//        base.OnHide.AddListener(OnDestroy);
    }


    void OnDestroy()
    {
//        base.OnHide.RemoveListener(OnDestroy);
        base.DestroyWidget();
    }
}
