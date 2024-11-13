using System;
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

    [SerializeField] private ObjectDatabase characterDatabase;
    [SerializeField] private int selectedObjectIndex = -1;
    private void Start()
    {
        FirebaseAuthManager.Instance.LoginState += OnChangedState;
        FirebaseAuthManager.Instance.Init();
        InitializeFirebase();
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
        canvasCharacterChoice.gameObject.SetActive(true);
    }

    public void LogOut()
    {
        FirebaseAuthManager.Instance.LogOut();
    }
    
    public void CharacterChoiceOnClick(int ID)
    {
        var id = UserData.Instance.UserId;
        selectedObjectIndex = characterDatabase.objectData.FindIndex(data => data.ID == ID);
        PlayerPrefs.SetInt($"select_{id}",selectedObjectIndex);
        var b = PlayerPrefs.GetInt($"select_{id}", -1);
        if (selectedObjectIndex < 0)
        {
            Debug.LogError($"No ID Found{ID}");
            return;
        }
        
        Instantiate(characterDatabase.objectData[selectedObjectIndex].Prefab);
        
        var sceneName = SceneUtility.GetScenePathByBuildIndex(1);
        SceneLoadManager.Instance.LoadScene(sceneName);
      
    }
}
