using UnityEngine;
using UnityEngine.UI;

public class ButtonGroupManager : MonoBehaviour
{
    [SerializeField] private ButtonGroup[] buttonGroups;

    public void OnButtonClicked(ButtonGroup currentGroup, Button clickedButton)
    {
        // 다른 그룹의 버튼 해제
        foreach (ButtonGroup group in buttonGroups)
        {
            if (group == currentGroup)
            {
                group.DeselectAllButtons();
            }
        }

        // 현재 클릭된 버튼 선택 상태 유지
        currentGroup.SelectButton(clickedButton);
    }
    
    public void OnButtonClickedFromReference(ButtonReference reference)
    {
        OnButtonClicked(reference.buttonGroup, reference.button);
    }
}
