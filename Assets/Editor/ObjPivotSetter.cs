#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

public class PivotSetterWindow : EditorWindow
{
    private GameObject srcObject;
    
    [MenuItem("Tools/PivotSetter")]
    public static void ShowWindow()
    {
        // 에디터 창을 표시
        PivotSetterWindow wnd = GetWindow<PivotSetterWindow>();
        wnd.titleContent = new GUIContent("Pivot Setter");
    }

    public void CreateGUI()
    {
        // 에디터 창의 루트 VisualElement
        VisualElement root = rootVisualElement;

        // 프리팹을 선택할 수 있는 필드 추가
        ObjectField objectField = new ObjectField("Prefab Object") { objectType = typeof(GameObject) };
        objectField.RegisterValueChangedCallback(evt => srcObject = evt.newValue as GameObject);
        root.Add(objectField);

        // 피벗 설정 버튼 추가
        Button pivotButton = new Button(SetPivot) { text = "Set Pivot" };
        root.Add(pivotButton);
    }

    private void SetPivot()
    {
        // 선택된 프리팹이 있는지 확인
        if (srcObject == null)
        {
            Debug.LogError("Prefab object를 선택해 주세요.");
            return;
        }

        // 임시 인스턴스를 생성하고 하위의 MeshRenderer를 검색
        GameObject tempInstance = Instantiate(srcObject);
        MeshRenderer[] meshRenderers = tempInstance.GetComponentsInChildren<MeshRenderer>();
        float minY = float.MaxValue;

        // 최하단의 y값 찾기
        foreach (MeshRenderer meshRenderer in meshRenderers)
        {
            minY = Mathf.Min(minY, meshRenderer.bounds.min.y);
        }

        // 오프셋을 모든 자식 오브젝트에 적용
        Vector3 offset = new Vector3(0, minY, 0);
        foreach (Transform child in tempInstance.transform)
        {
            child.localPosition -= offset;
        }

        // 수정된 인스턴스를 프리팹으로 저장
        string assetPath = AssetDatabase.GetAssetPath(srcObject);
        PrefabUtility.SaveAsPrefabAsset(tempInstance, assetPath);
        DestroyImmediate(tempInstance);

        Debug.Log("Pivot set to the bottom of the prefab.");
    }
}
#endif
