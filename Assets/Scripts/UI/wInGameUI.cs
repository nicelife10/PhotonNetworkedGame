using eeGames.Widget;
using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class wInGameUI : Widget
{

    [Space(10)]
    [Header("Gameplay Stuff")]
    public TextMeshProUGUI TimerText;
    public TextMeshProUGUI CountDownText;

    [Space(10)]
    [Header("GameComplete")]
    public GameObject GameCompletePanel;
    public TextMeshProUGUI GameResultText;

    private int Timer = 60;
    protected override void Awake()
    {
        base.Awake();

    }

    private void Start()
    {
        GameCompletePanel.SetActive(false);
        CountDownText.text = "waiting..";
        TimerText.text = "";
        GameEvents.instance.onCountDownStartEvents += OnGameStart;
        GameEvents.instance.onUpdateGamePlayTimer += SetGamePlayTimerText;
        GameEvents.instance.onFightOutcomeEvents += ShowGameCompletePanel;
    }

    void OnGameStart()
    {
        Timer = 60; 
        StartCoroutine(CountDownStart());
    }

    WaitForSeconds OneSecond = new WaitForSeconds(1f);
    IEnumerator CountDownStart()
    {
        yield return OneSecond;
        CountDownText.text = "3";
        yield return OneSecond;
        CountDownText.text = "2";
        yield return OneSecond;
        CountDownText.text = "1";
        yield return OneSecond;
        CountDownText.text = "FIGHT !!";
        yield return OneSecond;
        CountDownText.text = "";

        TimerText.text = "";
    }

    public double remaingingTime;
    public double FightStartTime;

    void SetGamePlayTimerText(int Timer)
    {
        TimerText.text = ((int)Timer).ToString();
    }

    void ShowGameCompletePanel(FightOutcome val)
    {
        GameCompletePanel.SetActive(true);
        switch (val)
        {
            case FightOutcome.Win:
                GameResultText.text = "YOU Won!";
                break;
            case FightOutcome.Lose:
                GameResultText.text = "You Lost!";
                break;
            case FightOutcome.Draw:
                GameResultText.text = "Draw!";
                break;
        }
    }

    private void OnDestroy()
    {
        GameEvents.instance.onCountDownStartEvents -= OnGameStart;
        GameEvents.instance.onUpdateGamePlayTimer -= SetGamePlayTimerText;
        GameEvents.instance.onFightOutcomeEvents -= ShowGameCompletePanel;

    }
}
