using UnityEngine;
using System;
using System.Collections.Generic;
using UnityEngine.UI;
using Cysharp.Threading.Tasks;

public class FriendListUIManager : MonoBehaviour
{
    [Header("UI References")]
    public Transform friendListParent;        // 친구 목록을 표시할 부모 Transform
    public GameObject friendItemPrefab;       // 친구 항목 프리팹
    public Transform popUpParent;             // 팝업을 표시할 부모 Transform
    public GameObject popUpPrefab;            // 팝업 프리팹
    public Button[] closeCanvas;              // 캔버스를 닫는 버튼 배열
    
    [SerializeField] private Canvas friendListCanvas;  // 친구 목록 캔버스
    
    private string _currentUserId;            // 현재 로그인한 사용자 ID
    
    private void Start()
    {
        InitializeCanvas();
        SetupEventListeners();
        friendListCanvas.enabled = false;
        RefreshListAsync().Forget();
    }

    /// <summary>
    /// Canvas 컴포넌트 초기화 및 검증
    /// </summary>
    private void InitializeCanvas()
    {
        if (friendListCanvas == null)
        {
            friendListCanvas = GetComponent<Canvas>();
            if (friendListCanvas == null)
            {
                Debug.LogError("Canvas 컴포넌트를 찾을 수 없습니다.");
                return;
            }
        }
    }

    /// <summary>
    /// 이벤트 리스너 설정
    /// </summary>
    private void SetupEventListeners()
    {
        foreach (var button in closeCanvas)
        {
            button.onClick.AddListener(OffMessenger);
        }

        FriendRequestUIManager.RefreshList += () => RefreshListAsync().Forget();
        RunnerManager.Instance.IsSpawned += AfterSpawn;
        UserData.ChangeName += OnChangedUserId;
        OnChangedUserId();
    }

    /// <summary>
    /// 플레이어 스폰 후 메신저 이벤트 연결
    /// </summary>
    private void AfterSpawn()
    {
        PlayerInput.OnMessenger += ToggleFriendCanvas;
    }

    /// <summary>
    /// 사용자 ID 변경 시 호출되는 메서드
    /// </summary>
    private void OnChangedUserId()
    {
        _currentUserId = UserData.Instance.UserId;
    }
    
    /// <summary>
    /// 컴포넌트 제거 시 이벤트 정리
    /// </summary>
    private void OnDestroy()
    {
        PlayerInput.OnMessenger -= ToggleFriendCanvas;
    }

    /// <summary>
    /// 메신저 UI를 비활성화합니다.
    /// </summary>
    private void OffMessenger()
    {
        PlayerInput.OnMessenger?.Invoke(false);
    }

    /// <summary>
    /// 친구 목록 캔버스의 활성화 상태를 토글합니다.
    /// </summary>
    /// <param name="isActive">활성화 여부</param>
    public void ToggleFriendCanvas(bool isActive)
    {
        if (friendListCanvas != null)
        {
            friendListCanvas.enabled = isActive;
        }
        else
        {
            Debug.LogWarning("Friend List Canvas가 null입니다. 상태를 변경할 수 없습니다.");
        }
    }

    /// <summary>
    /// 친구 목록을 새로고침합니다.
    /// </summary>
    private async UniTask RefreshListAsync()
    {
        try
        {
            var friendIds = await FriendRequestManager.GetFriendsAsync(_currentUserId);
            ClearFriendList();

            if (friendIds.Count > 0)
            {
                await LoadFriendDataAsync(friendIds);
            }
        }
        catch (Exception e)
        {
            Debug.LogError($"친구 목록 로드 실패: {e.Message}");
        }
    }

    /// <summary>
    /// 기존 친구 목록 UI를 정리합니다.
    /// </summary>
    private void ClearFriendList()
    {
        foreach (Transform child in friendListParent)
        {
            Destroy(child.gameObject);
        }
    }

    /// <summary>
    /// 친구 데이터를 비동기적으로 로드합니다.
    /// </summary>
    /// <param name="friendIds">친구 ID 목록</param>
    private async UniTask LoadFriendDataAsync(List<string> friendIds)
    {
        try
        {
            var friendPaths = new List<string>();
            foreach (var friendId in friendIds)
            {
                friendPaths.Add($"users/{friendId}");
            }

            var users = await RealtimeDatabase.ReadMultipleDataAsync<User>(friendPaths);
            foreach (var userEntry in users)
            {
                CreateFriendListItem(userEntry);
            }
        }
        catch (Exception e)
        {
            Debug.LogError($"친구 정보 일괄 로드 실패: {e.Message}");
        }
    }

    /// <summary>
    /// 친구 목록 항목을 생성합니다.
    /// </summary>
    /// <param name="userEntry">사용자 정보</param>
    private void CreateFriendListItem(KeyValuePair<string, User> userEntry)
    {
        GameObject friendItem = Instantiate(friendItemPrefab, friendListParent);
        FriendItem itemScript = friendItem.GetComponent<FriendItem>();
        
        itemScript.FriendId = userEntry.Key;
        itemScript.SetFriendData(userEntry.Value, popUpParent);
        
        Debug.Log($"친구 항목 생성: {userEntry.Value.name}");
    }
}

