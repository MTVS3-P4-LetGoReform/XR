using System;
using UnityEngine;

public class UserInfoCanvas : MonoBehaviour
{
    public Canvas canvas;

    private void Awake()
    {
        canvas = GetComponent<Canvas>();
    }
}
