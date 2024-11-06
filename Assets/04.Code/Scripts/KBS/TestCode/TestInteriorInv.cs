using UnityEngine;
using UnityEngine.UI;

public class TestInteriorInv : MonoBehaviour
{
    [SerializeField] 
    private Image[] images;

    private int currentIndex = 0;
    void Start()
    {
        UpdateImage();
    }

    public void NextImageOnClick()
    {
        currentIndex = (currentIndex + 1) % images.Length;  // 인덱스를 다음으로 넘김
        UpdateImage();
    }

    public void PreviousImageOnClick()
    {
        currentIndex = (currentIndex - 1 + images.Length) % images.Length;  // 인덱스를 다음으로
        UpdateImage();
    }

    private void UpdateImage()
    {
        for (int i = 0; i < images.Length; i++)
        {
            images[i].gameObject.SetActive(i == currentIndex);
        }
    }
}
