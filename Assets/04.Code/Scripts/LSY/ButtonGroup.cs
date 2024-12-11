using UnityEngine;
using UnityEngine.UI;

public class ButtonGroup : MonoBehaviour
{
    [SerializeField] private Button[] buttons;
    private Button selectedButton;

    public void SelectButton(Button button)
    {
        // 이전 선택된 버튼 해제
        if (selectedButton != null)
        {
            SetButtonState(selectedButton, false);
        }

        // 새로운 버튼 선택
        selectedButton = button;
        SetButtonState(selectedButton, true);
    }

    public void DeselectAllButtons()
    {
        if (selectedButton != null)
        {
            SetButtonState(selectedButton, false);
            selectedButton = null;
        }
    }

    private void SetButtonState(Button button, bool isSelected)
    {
        // 버튼의 시각적 상태를 변경 (예: 색상 변경)
        ColorBlock colors = button.colors;
        colors.normalColor = isSelected ? HexColorConverter.SetUIHexColor("#66FF97") : Color.white; // 선택된 경우 초록색
        button.colors = colors;
    }
}