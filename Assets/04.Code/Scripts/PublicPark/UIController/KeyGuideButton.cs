using UnityEngine;
using UnityEngine.UI;

public class KeyGuideButton : MonoBehaviour
{
    public Image imageKeyGuide;
    
    public void CloseGuideUIOnClick()
    {
        imageKeyGuide.gameObject.SetActive(false);
    }
}
