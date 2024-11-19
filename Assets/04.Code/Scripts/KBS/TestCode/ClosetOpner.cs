using System;
using UnityEngine;

public class ClosetOpner : MonoBehaviour
{
    public Canvas canvasCustom;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if(Input.GetKeyDown(KeyCode.E))
            {
                canvasCustom.gameObject.SetActive(true);
            }
        }
    }
}
