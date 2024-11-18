using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;

public class TabInputFieldNavigation : MonoBehaviour
{
    public TMP_InputField[] inputFields;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            SelectNextInputField();
        }
    }

    private void SelectNextInputField()
    {
        GameObject currentObject = EventSystem.current.currentSelectedGameObject;

        if (currentObject != null && currentObject.GetComponent<TMP_InputField>() != null)
        {
            // 현재 선택된 InputField를 가져옴
            TMP_InputField currentInputField = currentObject.GetComponent<TMP_InputField>();
            int currentIndex = System.Array.IndexOf(inputFields, currentInputField);

            // 다음 InputField로 이동
            int nextIndex = (currentIndex + 1) % inputFields.Length;
            inputFields[nextIndex].ActivateInputField();
        }
    }
}