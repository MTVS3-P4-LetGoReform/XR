using System;
using UnityEngine;
using UnityEngine.UI;

public class ButtonClick : MonoBehaviour
{
    public Button onReq;

    public GameObject reqObject;

    private bool _active;
    private void Start()
    {
        onReq.onClick.AddListener(ToggleReqObject);
    }

    private void ToggleReqObject()
    {
        bool reqActive = reqObject.activeSelf;
        reqObject.SetActive(!reqActive);
    }
}
