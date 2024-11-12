using System;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;

public class UserInfoCanvas : MonoBehaviour
{
    public Canvas canvas;
    
    public TMP_Text username;

    private void Awake()
    {
        canvas = GetComponent<Canvas>();
    }
}
