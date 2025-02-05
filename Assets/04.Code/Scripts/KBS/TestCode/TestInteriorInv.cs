using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TestInteriorInv : MonoBehaviour
{
    [SerializeField] 
    private GameObject[] images;
    [SerializeField] 
    private GameObject[] blockPages;
    [SerializeField] 
    private TMP_Text PagesCount;

    private int currentIndex = 0;
    void Start()
    {
        UpdateImage();
    }

    public void NextImageOnClick()
    {
        currentIndex = (currentIndex + 1) % blockPages.Length;  // 인덱스를 다음으로 넘김
        UpdateImage();
    }

    public void PreviousImageOnClick()
    {
        currentIndex = (currentIndex - 1 + blockPages.Length) % blockPages.Length;  // 인덱스를 다음으로
        UpdateImage();
    }

    private void UpdateImage()
    {
        for (int i = 0; i < blockPages.Length; i++)
        {
            blockPages[i].gameObject.SetActive(i == currentIndex);
        }
    }

    private void Update()
    {
        PagesCount.text = $"{currentIndex+1}/2";
    }


    public void ShowImg(int index)
    {

        if (index >= 0 && index < images.Length)
        {
            images[index].SetActive(true);
            switch (index)
            {
                case 0:
                    Debug.Log("블럭 탭");
                    images[1].SetActive(false);
                    images[2].SetActive(false);
                    images[3].SetActive(false);
                    images[4].SetActive(false);
                    break;
                case 1:
                    Debug.Log("가구 탭");
                    images[0].SetActive(false);
                    images[2].SetActive(false);
                    images[3].SetActive(false);
                    images[4].SetActive(false);
                    break;
                case 2:
                    Debug.Log("자연 탭");
                    images[0].SetActive(false);
                    images[1].SetActive(false);
                    images[3].SetActive(false);
                    images[4].SetActive(false);
                    break;
                case 3:
                    Debug.Log("조형 탭");
                    images[0].SetActive(false);
                    images[1].SetActive(false);
                    images[2].SetActive(false);
                    images[4].SetActive(false);
                    break;
                case 4:
                    Debug.Log("즐겨찾기 탭");
                    images[0].SetActive(false);
                    images[1].SetActive(false);
                    images[2].SetActive(false);
                    images[3].SetActive(false);
                    break;
            }
        }
    }

    public void BlockTabPage()
    {
        
    }
}
