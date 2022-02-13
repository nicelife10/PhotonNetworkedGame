using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ShowMessage : MonoBehaviour
{
    [SerializeField]
    TextMeshProUGUI text;
    public void SetMessage(string message)
    {
        text.text = message;
    }
}
