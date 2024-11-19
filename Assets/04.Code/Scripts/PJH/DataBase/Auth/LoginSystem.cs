using System;
using System.Collections;
using Cysharp.Threading.Tasks;
using GLTFast.Schema;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using static RealtimeDatabase;
public class LoginSystem : MonoBehaviour
{
    public TMP_InputField loginEmail;
    public TMP_InputField loginPassword;

    public TMP_InputField signupEmail;
    public TMP_InputField signupPassword;
    public TMP_InputField nickname;

    public TMP_Text outputText;
    public Canvas canvasCharacterChoice;
    
    public CanvasGroup canvasGroup;
    public float duration = 1f;
    public GameObject imageBlackboard;
    public GameObject imageMain;

    [SerializeField] private ObjectDatabase characterDatabase;
    [SerializeField] private int selectedObjectIndex = -1;
    private void Start()
    {
        InitializeFirebase();
        FirebaseAuthManager.Instance.Init();
        
        FirebaseAuthManager.Instance.LoginState += OnChangedState;
        FirebaseAuthManager.Instance.LoginState += CanvasOn;

        StartCoroutine(FadeIn());
    }

    private void OnChangedState(bool sign)
    {
        outputText.text = sign ? "Login : " : "Logout : ";
        outputText.text += FirebaseAuthManager.Instance.UserId;
    }

    public void Create()
    {
        string e = signupEmail.text;
        string p = signupPassword.text;
        string n = nickname.text;
        
        FirebaseAuthManager.Instance.CreateAccount(e,p,n);
    }

    public void Login()
    {
        FirebaseAuthManager.Instance.Login(loginEmail.text,loginPassword.text);
    }

    public void LogOut()
    {
        FirebaseAuthManager.Instance.LogOut();
    }

    private void CanvasOn(bool isActive)
    {
        canvasCharacterChoice.gameObject.SetActive(isActive);
    }
    
    public void CharacterChoiceOnClick(int ID)
    {
        var id = UserData.Instance.UserId;
        if (ID < 0)
        {
            Debug.LogError($"No ID Found{ID}");
            return;
        }

        if (id == null)
            return;
        
        selectedObjectIndex = characterDatabase.objectData.FindIndex(data => data.ID == ID);
        PlayerPrefs.SetInt($"select_{id}",selectedObjectIndex);
        Debug.Log($"select_{id}, selectedObjectIndex : "+selectedObjectIndex);
    }

    public async void GotoPlayScene()
    {
        var sceneName = SceneUtility.GetScenePathByBuildIndex(1);
        await SceneLoadManager.Instance.LoadScene(sceneName);
    }
    
    private IEnumerator FadeIn()
    {
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            canvasGroup.alpha = Mathf.Clamp01(elapsed / duration);
            yield return null;
        }

        canvasGroup.alpha = 1f; // 마지막에 alpha를 1로 고정

        if (canvasGroup.alpha >= 1f)
        {
            imageBlackboard.gameObject.SetActive(false);
            imageMain.gameObject.SetActive(false);
        }
    }
}
