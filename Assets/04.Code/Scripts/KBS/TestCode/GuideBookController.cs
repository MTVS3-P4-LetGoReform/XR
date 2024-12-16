using UnityEngine;

public class GuideBookController : MonoBehaviour
{
    public Canvas canvasGuide;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.B))
        {
            canvasGuide.gameObject.SetActive(true);
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            canvasGuide.gameObject.SetActive(false);
        }
}
}
