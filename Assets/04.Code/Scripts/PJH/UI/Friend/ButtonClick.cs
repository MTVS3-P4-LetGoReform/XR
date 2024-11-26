using System;
using UnityEngine;
using UnityEngine.UI;

public class ButtonClick : MonoBehaviour
{
    public Button onReqFriend;
    public GameObject reqObject;

    private bool _currentState;
    private void Start()
    {
        _currentState = false;
        reqObject.SetActive(_currentState);
        onReqFriend.onClick.AddListener(ToggleReqObject);
    }

    private void ToggleReqObject()
    {
        if (reqObject != null)
        {
            _currentState = !_currentState;
            reqObject.SetActive(_currentState);
        }
        else
        {
            Debug.LogWarning("reqObject is not assigned!");
        }
    }
}
