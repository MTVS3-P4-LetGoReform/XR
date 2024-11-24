using System.Collections.Generic;
using GLTFast;
using UnityEngine;
using UnityEngine.UI;

public class StatueInventoryController : MonoBehaviour
{
    // 초기 인덱스 - 시연용으로 2개의 스태츄를 미리 넣어 놓음.
    private const int Initial_Index = 0;
    // 인벤토리 한 장 최대 크기
    private const int Max_Inventory_Num = 20;
    // 현재 빈 인벤토리 칸 인덱스
    [SerializeField] private int curIndex = Initial_Index;

    public List<GameObject> inventoryTargetList;
    
    public List<StatueData> statueDatas;

    public void Start()
    {
        statueDatas = new List<StatueData>();
    }
    // /* FIXME : 인벤토리 아이템 수에 맞게 아이템 생성하도록 메서드 */
    // // 인벤토리 칸의 부모 객체
    // [SerializeField] private GameObject ContentArea;
    // [SerializeField] private GameObject ContentUIPrefab;
    //
    // public void InventoryBoxGenerator()
    // {
    //     
    // }

    /* FIXME : DB랑 연결 */
    
    // 인벤토리에 StatueData 추가 후 UI 보여주기
    public void AddStatueToInven(string modelId, Sprite sprite, GltfImport gltfImport)
    { 
        // 새로운 StatueData 선언
        StatueData newStatueData = new StatueData(modelId, sprite, gltfImport);
        // 리스트에 새로운 StatueData 추가
        statueDatas.Add(newStatueData);
        // 
        //GameObject childObject = inventoryTargetList[curIndex].transform.GetChild(0).gameObject;
        //Debug.Log($"childObject : {childObject.name}");
        Image curImage = inventoryTargetList[curIndex].transform.GetChild(0).GetComponent<Image>(); ;
        curImage.sprite = sprite;
        Color color = curImage.color;
        color.a = 1.0f;
        curImage.color = color;
        inventoryTargetList[curIndex].SetActive(true);
        curIndex++;
    }

    public async void StatueInvenTestBtn()
    {
        Sprite imageSprite = SpriteConverter.ConvertFromPNG("DebugModeImage.png");
        Debug.Log($"{imageSprite}");
        GltfImport gltfImport = await GltfLoader.LoadGLTF(PathConverter.GetModelPath("DebugModeModel.glb"));
        Debug.Log("flag2");
        AddStatueToInven("m_0000", imageSprite, gltfImport);
        Debug.Log("flag3");
    }
    

}