using System;
using UnityEngine;
using UnityEngine.UI;

public class Highlight : MonoBehaviour
{
    public Image highlight;

    private void Start()
    {
        highlight.enabled = false;
    }
    
    public void OnPointerEnter()
    {
        highlight.enabled = true;
    }

    public void OnPointerExit()
    {
        highlight.enabled = false;
    }
}
