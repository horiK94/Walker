using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [SerializeField]
    Text floorText = null;

    [SerializeField]
    Text retryText = null;

    public void SetFloor(int _floor)
    {
        floorText.text = _floor + "F";
    }

    public void AppearRetryText()
    {
        retryText.enabled = true;
    }
}
