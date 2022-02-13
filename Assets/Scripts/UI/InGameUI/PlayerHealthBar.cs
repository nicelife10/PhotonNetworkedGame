using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHealthBar : MonoBehaviour
{
    public Slider healthSlider;
    public bool isMasterSlider;
    // Start is called before the first frame update
    void Start()
    {
        SetSliderValue(100);

        if (isMasterSlider)
        {
            GameEvents.instance.onUpdateMasterHealth += SetSliderValue;
        }
        else
        {
            GameEvents.instance.onUpdateClientHealth += SetSliderValue;
        }
    }

    private void OnDestroy()
    {
        if (isMasterSlider)
        {
            GameEvents.instance.onUpdateMasterHealth -= SetSliderValue;
        }
        else
        {
            GameEvents.instance.onUpdateClientHealth -= SetSliderValue;
        }
    }

    void SetSliderValue(int val)
    {
        healthSlider.value = val;
    }
}
