using UnityEngine;
using Photon.Chat;
using ExitGames.Client.Photon;
using TMPro;
using UnityEngine.UI;

public class ChatController : MonoBehaviour, IChatClientListener
{
	public TMP_InputField inputField;
	public TMP_Text outputText;

	public Button chatOn;
	public Button chatOff;

	public GameObject chatOnPanel;
	public GameObject chatPanel;

	private Canvas _canvas;
	private ChatClient _chatClient;
	private string _userName;
	private string _currentChannelName;
	private const string AppId = "f9b6f292-fcf5-424d-8d31-75b7450643f2";
	

	private void Awake()
	{
		UserData.ChangeName += OnChangedUserName;
		_userName = UserData.Instance.UserName;
		_canvas = GetComponent<Canvas>();
		chatOn.onClick.AddListener(() => ToggleChatPanel(true));
		chatOff.onClick.AddListener(() => ToggleChatPanel(false));
	}

	void Start()
	{
		PlayerInput.OnChat += Chating;
		inputField.onSubmit.AddListener(Input_OnEndEdit);
		
		Application.runInBackground = true;

		_currentChannelName = "메인 광장";

		_chatClient = new ChatClient(this);

		_chatClient.Connect(AppId, "1", new AuthenticationValues(_userName));

		//AddLine(string.Format("연결시도", _userName));
		AddLine("연결시도");
	}

	private void ToggleChatPanel(bool active)
	{
		chatOnPanel.SetActive(!active);
		chatPanel.SetActive(active);
	}

	private void OnChangedUserName()
	{
		_userName = UserData.Instance.UserName;
	}
	
	private void Chating(bool chatOn)
	{
		if (!inputField.isFocused && chatOn)
		{
			inputField.ActivateInputField(); 
			return;
		}
		if(inputField.isFocused && inputField.text.Trim() == "" && !chatOn)
		{
			inputField.DeactivateInputField();
			return;
		}
		if (!chatOn)
		{
			inputField.DeactivateInputField();
		}
	}
	
	public void AddLine(string lineString)
	{
		outputText.text += lineString + "\r\n";
	}

	public void OnApplicationQuit()
	{
		if (_chatClient != null)
		{
			_chatClient.Disconnect();
		}
	}

	public void DebugReturn(DebugLevel level, string message)
	{
		if (level == DebugLevel.ERROR)
		{
			Debug.LogError(message);
		}
		else if (level == DebugLevel.WARNING)
		{
			Debug.LogWarning(message);
		}
		else
		{
			Debug.Log(message);
		}
	}
	
	public void OnConnected()
	{
		AddLine("서버에 연결되었습니다.");

		_chatClient.Subscribe(_currentChannelName, 10);
	}

	public void OnDisconnected()
	{
		AddLine("서버에 연결이 끊어졌습니다.");
	}

	public void OnChatStateChange(ChatState state)
	{
		Debug.Log("OnChatStateChange = " + state);
	}

	public void OnSubscribed(string[] channels, bool[] results)
	{
		AddLine(string.Format("채널 입장 ({0})", string.Join(",", channels)));
		_canvas.enabled = true;
	}

	public void OnUnsubscribed(string[] channels)
	{
		AddLine(string.Format("채널 퇴장 ({0})", string.Join(",", channels)));
	}

	public void OnGetMessages(string channelName, string[] senders, object[] messages)
	{
		for (int i = 0; i < messages.Length; i++)
		{
			AddLine(string.Format("{0} : {1}", senders[i], messages[i]));
		}
	}

	public void OnPrivateMessage(string sender, object message, string channelName)
	{
		Debug.Log("OnPrivateMessage : " + message);
	}

	public void OnStatusUpdate(string user, int status, bool gotMessage, object message)
	{
		Debug.Log("status : " + string.Format("{0} is {1}, Msg : {2} ", user, status, message));
	}

	void Update()
	{
		_chatClient.Service();
	}

	public void Input_OnValueChanged(string text)
	{
		Debug.Log(text);
	}

	public void Input_OnEndEdit(string text)
	{
		if (_chatClient.State == ChatState.ConnectedToFrontEnd)
		{
			if(inputField.text.Trim() == "")
			{
				inputField.DeactivateInputField();
				return;
			}
			
			_chatClient.PublishMessage(_currentChannelName,text);
			
			inputField.text = "";
			inputField.DeactivateInputField();
		}
	}

    public void OnUserSubscribed(string channel, string user)
    {
        //throw new System.NotImplementedException();
    }

    public void OnUserUnsubscribed(string channel, string user)
    {
        //throw new System.NotImplementedException();
    }
}
