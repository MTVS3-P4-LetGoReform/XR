using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class InteriorUIController : MonoBehaviour
{
    public Button openInterior;
    public Button closeInterior;

    public Canvas canvasInterior;
    public Image userInterface;
    public Image imageInterGuide;

    private bool isPushed = false;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.O))
        {
            closeInterior.gameObject.SetActive(true);
            canvasInterior.gameObject.SetActive(true);
            userInterface.gameObject.SetActive(false);
        }
    }

    public void OpenInteriorOnClick()
    {
        closeInterior.gameObject.SetActive(true);
        canvasInterior.gameObject.SetActive(true);
        userInterface.gameObject.SetActive(false);
    }

    public void CloseInteriorOnClick()
    {
        closeInterior.gameObject.SetActive(false);
        canvasInterior.gameObject.SetActive(false);
        userInterface.gameObject.SetActive(true);
    }

    public void CloseInteriorGuideButtonOnClick()
    {
        imageInterGuide.gameObject.SetActive(false);
    }
    
    
    
    

  /*  private void Update()
    {
        if (Input.GetKey(KeyCode.Tab))
        {
            isPushed = true;
            
        }

        if (Input.GetKeyUp(KeyCode.Tab))
        {
            isPushed = false;
        }

        if (isPushed)
        {
            Cursor.lockState = CursorLockMode.None;
        }
        else
        {
            Cursor.lockState = CursorLockMode.Locked;
        }
    } */
}
