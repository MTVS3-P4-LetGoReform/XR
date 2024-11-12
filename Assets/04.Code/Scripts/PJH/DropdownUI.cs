using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class StatusDropdown : MonoBehaviour
{
    public TMP_Dropdown statusDropdown;  // 드롭다운 컴포넌트
    public Image statusImage;
    public Sprite[] sprites;
    
    private void Start()
    {
        // 드롭다운이 변경될 때 이벤트 연결
        statusDropdown.onValueChanged.AddListener(OnStatusChanged);

        // 초기 상태 설정
        UpdateStatusText();
    }

    // 드롭다운 선택이 변경될 때 호출되는 함수
    private void OnStatusChanged(int index)
    {
        UpdateStatusText();
    }

    // 상태를 텍스트에 업데이트하는 함수
    private void UpdateStatusText()
    {
        if (statusDropdown.value == 0)
        {
            statusImage.sprite = sprites[0];
        }
        else if (statusDropdown.value == 1)
        {
            statusImage.sprite = sprites[1];
        }
    }
}