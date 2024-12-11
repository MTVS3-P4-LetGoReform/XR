using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UserInfoCanvas : MonoBehaviour
{
    public Canvas canvas;
    public Button friendButton;
    public TMP_Text username;
    
    private void Awake()
    {
        canvas = GetComponent<Canvas>();
    }

    private void Start()
    {
        RunnerManager.Instance.IsSpawned += AfterSpawn;
    }

    private void AfterSpawn()
    {
        PlayerInput input = FindAnyObjectByType<PlayerInput>();
        friendButton.onClick.AddListener(input.ToggleMessenger);
    }
}
