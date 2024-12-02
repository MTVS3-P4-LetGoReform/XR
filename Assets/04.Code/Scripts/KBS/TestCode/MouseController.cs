using UnityEngine;

public class MouseController : MonoBehaviour
{
    private bool isTapKeyPressed = false;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        MousePointController();
    }
    
    private void MousePointController()
    {
        if (Input.GetKey(KeyCode.Tab))
        {
            isTapKeyPressed = true;
            
            
        }

        if (Input.GetKeyUp(KeyCode.Tab))
        {
            isTapKeyPressed = false;
            
            
        }

        if (isTapKeyPressed)
        {
            Cursor.lockState = CursorLockMode.None;
            
        }
        else
        {
            Cursor.lockState = CursorLockMode.Locked;
        }
    }
}
