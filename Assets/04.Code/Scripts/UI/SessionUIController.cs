using UnityEngine;
using UnityEngine.UI;

/* 스케치 혹은 텍스트 프롬프트 입력 창을 띄울수 있는 UI Controller 클래스 */
public class SessionUIController : MonoBehaviour
{
    public Button sketchPromptTabBtn;
    public Button txtPromptTabBtn;
    
    public GameObject sketchPromptTab;
    public GameObject txtPromptTab;
    
    void Start()
    {
        sketchPromptTabBtn.onClick.AddListener(ActivateSkectchPromptTab);
        txtPromptTabBtn.onClick.AddListener(ActivateTxtPromptTab);
    }

    /* 스케치 프롬프트 탭 활성화 */
    public void ActivateSkectchPromptTab()
    {
        sketchPromptTab.SetActive(true);
        txtPromptTab.SetActive(false);
    }
    /* 텍스트 프롬프트 탭 활성화 */
    public void ActivateTxtPromptTab()
    {
        sketchPromptTab.SetActive(false);
        txtPromptTab.SetActive(true);
    }
}
