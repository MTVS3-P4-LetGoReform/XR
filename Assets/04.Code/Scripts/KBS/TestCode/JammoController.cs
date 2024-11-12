using UnityEngine;

public class JammoController : MonoBehaviour
{
    public Canvas npcCanvasJammo;
    public Canvas marketCanvasSouvenir;
    
    void Start()
    {
        
    }

    
    void Update()
    {
        //player가 가까이 다가왔을 때
        if (Input.GetKeyDown(KeyCode.E))
        {
            npcCanvasJammo.gameObject.SetActive(true);
            
        }
    }

    public void NextButtonOnClick()
    {
        marketCanvasSouvenir.gameObject.SetActive(true);
        npcCanvasJammo.gameObject.SetActive(false);
    }
}
