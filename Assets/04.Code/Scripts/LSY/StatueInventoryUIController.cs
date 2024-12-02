using System;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;

public class StatueInventoryUIController : MonoBehaviour
{
    // 스태츄 인벤토리 최상위 캔버스 오브젝트
    public GameObject statueInventoryCanvas;
    // 스태츄 인벤토리 전체 UI
    public RectTransform statueInventoryUI;
    // 하위 스태츄 정보 UI(왼쪽 페이지)
    public CanvasGroup statueInventoryInfo;
    // 하위 스태츄 목록 UI(오른쪽 페이지)
    public RectTransform statueIvnentoryList;
    // 리스트 콘텐츠 영역 배경
    public RectTransform ContentBG;
    // 카테고리 영역 배경
    public RectTransform CategoryBG;
    // imageBtns 영역
    public RectTransform imageBtnsArea;
    // // 중간 컨투어 경계
    // public CanvasGroup contourLine;
    // 뒤로가기 버튼
    public CanvasGroup bakcBtn;

    // 스태츄 설치 버튼들
    public List<GameObject> installBtns;
    public List<Button> imageBtns;
    
    // 인벤토리 모드 스태츄 인벤토리 전체 UI 이동 포인트
    public RectTransform inventoryModePoint;
    // 인벤토리 모드 스태츄 정보UI 이동 포인트
    public RectTransform listInventoryModePoint;
    // 인벤토리 모드 ImageBtns 이동 포인트
    public RectTransform imageBtnsInventoryModePoint;
    
    // 인테리어 모드 스태츄 인벤토리 전체 UI 이동 포인트
    public RectTransform interiorModePoint;
    // 인테리어 모드 스태츄 정보UI 이동 포인트
    public RectTransform listInteriorModePoint;
    // 인테리어 모드 ImageBtns 이동 포인트
    public RectTransform imageBtnsInteriorModePoint;
    

    public float duration = 1.0f;

    public void Start()
    {
        for (int i = 0; i < imageBtns.Count; i++)
        {
            //imageBtns[i].onClick.AddListener(() => ActivateInstallBtn(i));
            installBtns[i].GetComponent<Button>().onClick.AddListener(SetInteriorMode);
            
        }
    }

    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            SetInventoryMode();
        }
    }
    public void SetInteriorMode()
    {
        Debug.Log("interiorModePoint : "+ interiorModePoint.anchoredPosition);
        Debug.Log("listinteriorModePoint : "+ listInteriorModePoint.anchoredPosition);
        // 이동 애니메이션
        //contourLine.DOFade(0f, duration).OnComplete(()=> contourLine.gameObject.SetActive(false));
        bakcBtn.DOFade(0f, duration).OnComplete(()=> bakcBtn.gameObject.SetActive(false));
        statueInventoryInfo.DOFade(0f, duration).OnComplete(()=> statueInventoryInfo.gameObject.SetActive(false));
        statueInventoryUI.DOAnchorPos(interiorModePoint.anchoredPosition, duration);
        statueIvnentoryList.DOAnchorPos(listInteriorModePoint.anchoredPosition, duration);
        imageBtnsArea.DOAnchorPos(imageBtnsInteriorModePoint.anchoredPosition, duration);
        statueIvnentoryList.DOSizeDelta(new Vector2(584f, 960f), duration);
        ContentBG.DOSizeDelta(new Vector2(584f, 696f), duration);
        CategoryBG.DOSizeDelta(new Vector2(584f, 793f), duration);
        //bakcBtn.gameObject.GetComponent<RectTransform>().DOAnchorPos(listInteriorModePoint.anchoredPosition, duration);
        Cursor.lockState = CursorLockMode.Locked;
    }

    public void SetInventoryMode()
    {
        //contourLine.gameObject.SetActive(true);
        bakcBtn.gameObject.SetActive(true);
        statueInventoryInfo.gameObject.SetActive(true);
        //contourLine.DOFade(1f, duration);
        bakcBtn.DOFade(1f, duration);
        statueInventoryInfo.DOFade(1f, duration);
        statueInventoryUI.DOAnchorPos(inventoryModePoint.anchoredPosition, duration);
        statueIvnentoryList.DOAnchorPos(listInventoryModePoint.anchoredPosition, duration);
        imageBtnsArea.DOAnchorPos(imageBtnsInventoryModePoint.anchoredPosition, duration);
        statueIvnentoryList.DOSizeDelta(new Vector2(1194f, 960f), duration);
        ContentBG.DOSizeDelta(new Vector2(1194f, 696f), duration);
        CategoryBG.DOSizeDelta(new Vector2(1194f, 793f), duration);
        Cursor.lockState = CursorLockMode.None;
        
    }

    public void ActivateInstallBtn(int idx)
    {
        Debug.Log("ActivateInstallBtn idx : " + idx);
        installBtns[idx].SetActive(true);
    }
    
    
}