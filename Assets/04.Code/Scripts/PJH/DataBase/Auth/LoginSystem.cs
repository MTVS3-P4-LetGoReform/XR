using System;
using Cysharp.Threading.Tasks;
using Firebase.Database;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoginSystem : MonoBehaviour
{
    [Header("Login UI Components")]
    public TMP_InputField loginEmail;
    public TMP_InputField loginPassword;

    [Header("Signup UI Components")]
    public TMP_InputField signupEmail;
    public TMP_InputField signupPassword;
    public TMP_InputField nickname;

    [Header("UI References")]
    public TMP_Text outputText;          // 로그인 상태를 표시할 텍스트
    public Canvas canvasCharacterChoice; // 캐릭터 선택 캔버스
    public CanvasGroup canvasGroup;      // 페이드 효과를 위한 캔버스 그룹
    public GameObject imageBlackboard;    // 초기 로딩 화면의 검은 배경
    public GameObject imageMain;          // 메인 이미지
    
    [Header("Settings")]
    public float duration = 1f;          // 페이드 효과 지속 시간
    [SerializeField] private ObjectDatabase characterDatabase; // 캐릭터 데이터베이스
    [SerializeField] private int selectedObjectIndex = -1;     // 선택된 캐릭터 인덱스
    
    private async void Awake()
    {
        // 오프라인 데이터 캐시 비활성화 및 재연결
        FirebaseDatabase.DefaultInstance.SetPersistenceEnabled(false);
        FirebaseDatabase.DefaultInstance.GoOffline();
        FirebaseDatabase.DefaultInstance.GoOnline();
    }
    
    private async void Start()
    {
        await InitializeSystemAsync();
        SetupEventListeners();
        FadeInAsync().Forget();
    }

    /// <summary>
    /// Firebase 및 인증 시스템 초기화
    /// </summary>
    private async UniTask InitializeSystemAsync()
    {
        await RealtimeDatabase.InitializeFirebaseAsync();
        FirebaseAuthManager.Instance.Init();
    }

    /// <summary>
    /// 이벤트 리스너 설정
    /// </summary>
    private void SetupEventListeners()
    {
        FirebaseAuthManager.Instance.LoginState += OnChangedState;
        FirebaseAuthManager.Instance.LoginState += CanvasOn;
    }

    /// <summary>
    /// 로그인 상태 변경 시 UI 업데이트
    /// </summary>
    private void OnChangedState(bool sign)
    {
        outputText.text = $"{(sign ? "Login : " : "Logout : ")}{FirebaseAuthManager.Instance.UserId}";
    }

    /// <summary>
    /// 회원가입 처리
    /// </summary>
    public async void Create()
    {
        try
        {
            await FirebaseAuthManager.Instance.CreateAccountAsync(
                signupEmail.text,
                signupPassword.text,
                nickname.text
            );
        }
        catch (Exception e)
        {
            Debug.LogError($"계정 생성 실패: {e.Message}");
        }
    }

    /// <summary>
    /// 로그인 처리
    /// </summary>
    public async void Login()
    {
        try
        {
            await FirebaseAuthManager.Instance.LoginAsync(loginEmail.text, loginPassword.text);
        }
        catch (Exception e)
        {
            Debug.LogError($"로그인 실패: {e.Message}");
        }
    }

    /// <summary>
    /// 로그아웃 처리
    /// </summary>
    public async void LogOut()
    {
        try
        {
            await FirebaseAuthManager.Instance.LogOutAsync();
        }
        catch (Exception e)
        {
            Debug.LogError($"로그아웃 실패: {e.Message}");
        }
    }

    /// <summary>
    /// 캐릭터 선택 캔버스 표시/숨김 처리
    /// </summary>
    private void CanvasOn(bool isActive)
    {
        canvasCharacterChoice.gameObject.SetActive(isActive);
    }
    
    /// <summary>
    /// 캐릭터 선택 처리 및 저장
    /// </summary>
    public void CharacterChoiceOnClick(int ID)
    {
        if (ID < 0)
        {
            Debug.LogError($"잘못된 캐릭터 ID: {ID}");
            return;
        }

        var userId = UserData.Instance.UserId;
        if (string.IsNullOrEmpty(userId))
            return;
        
        selectedObjectIndex = characterDatabase.objectData.FindIndex(data => data.ID == ID);
        PlayerPrefs.SetInt($"select_{userId}", selectedObjectIndex);
        Debug.Log($"캐릭터 선택 완료 - UserId: {userId}, 선택된 캐릭터 인덱스: {selectedObjectIndex}");
    }

    /// <summary>
    /// 공원 씬으로 이동
    /// </summary>
    public async void GotoParkScene()
    {
        var sceneName = SceneUtility.GetScenePathByBuildIndex(1);
        await SceneLoadManager.Instance.LoadScene(sceneName);
    }
    
    /// <summary>
    /// 초기 페이드 인 효과 처리
    /// </summary>
    private async UniTask FadeInAsync()
    {
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            canvasGroup.alpha = Mathf.Clamp01(elapsed / duration);
            await UniTask.Yield();
        }

        canvasGroup.alpha = 1f;

        if (canvasGroup.alpha >= 1f)
        {
            imageBlackboard.SetActive(false);
            imageMain.SetActive(false);
        }
    }
}
