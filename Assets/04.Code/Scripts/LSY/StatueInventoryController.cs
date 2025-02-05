﻿using System.Collections.Generic;
using System.IO;
using GLTFast;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR;
using GltfImport = GLTFast.GltfImport;

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

    public List<Button> installBtns;

    public Sprite mockDataSprite1;
    public Sprite mockDataSprite2;
    public Sprite mockDataSprite3;
    public Sprite mockDataSprite4;

    private GltfImport gltfImport3;
    private GltfImport gltfImport4;
    
    public async void Start()
    {
        statueDatas = new List<StatueData>();
        GltfImport gltfImport1 = await GltfLoader.LoadGLTF(Path.Combine(Application.persistentDataPath,"Models", "inven0.glb"));
        GltfImport gltfImport2 = await GltfLoader.LoadGLTF(Path.Combine(Application.persistentDataPath,"Models", "inven1.glb"));
        AddStatueToInven("m_id_0001", "tempImage1", "tempModel1", mockDataSprite1, gltfImport1, "tempcreatorId");
        AddStatueToInven("m_id_0002", "tempImage2","tempModel2", mockDataSprite2, gltfImport2, "tempcreatorId");
        gltfImport3 = await GltfLoader.LoadGLTF(Path.Combine(Application.persistentDataPath, "Models", "MainMockDataModel.glb"));
        gltfImport4 = await GltfLoader.LoadGLTF(Path.Combine(Application.persistentDataPath, "Models", "hamo.glb"));
    }

    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.Minus))
        {
            AddStatueToInven("m_id_0003", "tempImage3", "tempModel3", mockDataSprite3, gltfImport3, "tempcreatorId");
        }

        if (Input.GetKeyDown(KeyCode.Equals))
        {
            AddStatueToInven("m_id_0004", "tempImage4", "tempModel4", mockDataSprite4, gltfImport4, "tempcreatorId");
        }
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
   public void AddStatueToInven(string modelId, string imageName, string modelName, Sprite sprite, GltfImport gltfImport, string creatorId)
    {
        
        Debug.Log("AddStatueToInven");
        // 새로운 StatueData 선언
        StatueData newStatueData = new StatueData(modelId, imageName, modelName, sprite, gltfImport, creatorId);
        Debug.Log($"modelId - {newStatueData.modelId} ");
        // 리스트에 새로운 StatueData 추가
        statueDatas.Add(newStatueData);
        // 
        //GameObject childObject = inventoryTargetList[curIndex].transform.GetChild(0).gameObject;
        //Debug.Log($"childObject : {childObject.name}");
        if (inventoryTargetList[curIndex] == null)
        {
            Debug.LogError("inventoryTargetList[curIndex] is null!");
        }
        Debug.Log($"curIndex : {curIndex}");
        inventoryTargetList[curIndex].SetActive(true);
        Image curImage = inventoryTargetList[curIndex].transform.GetChild(0).GetComponent<Image>(); ;
        curImage.sprite = sprite;
        Color color = curImage.color;
        color.a = 1.0f;
        curImage.color = color;
        
        curIndex++;
    }

    public async void StatueInvenTestBtn()
    {
        Sprite imageSprite = SpriteConverter.ConvertFromPNG("MainMockDataImage.png");
        Debug.Log($"{imageSprite}");
        GltfImport gltfImport = await GltfLoader.LoadGLTF(PathConverter.GetModelPath("MainMockDataModel.glb"));
        Debug.Log("flag2");
        AddStatueToInven("m_0000", "tempImage0", "tempModel0", imageSprite, gltfImport, "teampUserId");
        Debug.Log("flag3");
    }
    

}