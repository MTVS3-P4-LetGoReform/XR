using System;
using UnityEngine;
using Photon.Chat;
using ExitGames.Client.Photon;
using TMPro;

public class ChatController : MonoBehaviour, IChatClientListener
{
	public TMP_InputField inputField;
	public TMP_Text outputText;

	private ChatClient _chatClient;
	private string _userName;
	private string _currentChannelName;
	private string _appId = "f9b6f292-fcf5-424d-8d31-75b7450643f2";

	private void Awake()
	{
		_userName = UserData.Instance.UserName;
	}

	void Start()
	{
		Application.runInBackground = true;

		_currentChannelName = "Channel 001";

		_chatClient = new ChatClient(this);

		_chatClient.Connect(_appId, "1", new AuthenticationValues(_userName));

		//AddLine(string.Format("연결시도", _userName));
		AddLine("연결시도");
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
	}

	public void OnUnsubscribed(string[] channels)
	{
		AddLine(string.Format("채널 퇴장 ({0})", string.Join(",", channels)));
	}

	public void OnGetMessages(string channelName, string[] senders, object[] messages)
	{
		for (int i = 0; i < messages.Length; i++)
		{
			AddLine(string.Format("{0} : {1}", senders[i], messages[i].ToString()));
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

	public void Input_OnEndEdit(string text)
	{
		if (_chatClient.State == ChatState.ConnectedToFrontEnd)
		{
			//chatClient.PublishMessage(currentChannelName, text);
			_chatClient.PublishMessage(_currentChannelName, inputField.text);

			inputField.text = "";
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
