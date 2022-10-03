using UnityEngine;
using Photon.Pun;
using Photon.Chat;
using TMPro;
using UnityEngine.UI;
using ExitGames.Client.Photon;

public class ChatManager : MonoBehaviour, IChatClientListener
{
    [Header("Chat")]
    ChatClient chatClient;
    [SerializeField] private string userID;
    [SerializeField] private TMP_Text chatText;
    [SerializeField] private TMP_InputField textMessage;
    [SerializeField] private TMP_InputField textUserName;
    [SerializeField] private TMP_InputField textFriendsName;

    [Header("Login")]
    [SerializeField] private TMP_InputField inputName;
    [SerializeField] private GameObject panelChat;
    [SerializeField] private GameObject panelLogin;
    [SerializeField] private GameObject panelFriends;

    private string[] friends;

    public void DebugReturn(DebugLevel level, string message)
    {
        Debug.Log($"{level}, {message}");
    }

    public void OnChatStateChange(ChatState state)
    {
        Debug.Log(state);
    }

    public void OnConnected()
    {
        chatText.text += "\nВи підключилися до чату!";
        chatClient.Subscribe("Chat");
    }

    public void OnDisconnected()
    {
        chatClient.Unsubscribe(new string[] { "Chat" });
    }

    public void OnGetMessages(string channelName, string[] senders, object[] messages)
    {
        for (int i = 0; i < senders.Length; i++)
        {
            chatText.text += $"\n[{channelName}] {senders[i]}: {messages[i]}";
        }
    }

    public void OnPrivateMessage(string sender, object message, string channelName)
    {
        chatText.text += $"\nПриватне повідомлення від {sender}: {message}";
    }

    public void OnStatusUpdate(string user, int status, bool gotMessage, object message)
    {
        chatText.text += $"\nВаш друг {user} змінив статус на {message} ({status}).";
    }

    public void OnSubscribed(string[] channels, bool[] results)
    {
        for (int i = 0; i < channels.Length; i++)
        {
            chatText.text += $"\nВи підключені до {channels[i]}.";
        }
    }

    public void OnUnsubscribed(string[] channels)
    {
        for (int i = 0; i < channels.Length; i++)
        {
            chatText.text += $"\nВи відключені від {channels[i]}.";
        }
    }

    public void OnUserSubscribed(string channel, string user)
    {
        chatText.text += $"\nКористувач {user} підключився до {channel}.";
    }

    public void OnUserUnsubscribed(string channel, string user)
    {
        chatText.text += $"\nКористувач {user} відключився від {channel}.";
    }

    void Start()
    {
        chatClient = new ChatClient(this);
    }

    void Update()
    {
        chatClient.Service();
    }

    public void SendButton()
    {
        if (textUserName.text == "")
        {
            chatClient.SetOnlineStatus(4, "(спілкується)");
            if (textMessage.text != "")
            {
                chatClient.PublishMessage("Chat", textMessage.text);
            }
        }
        else
        {
            chatClient.SetOnlineStatus(4, "(спілкується)");
            if (textMessage.text != "")
            {
                chatClient.SendPrivateMessage(textUserName.text, textMessage.text);

            }
        }
    }

    public void LoginButton()
    {
        panelChat.SetActive(true);
        panelLogin.SetActive(false);
        panelFriends.SetActive(true);

        userID = inputName.text;
        chatClient.Connect(PhotonNetwork.PhotonServerSettings.AppSettings.AppIdChat, PhotonNetwork.AppVersion, new AuthenticationValues(userID));
    }

    public void AddFriendsButon()
    {
        //string[] friends = textFriendsName.text.Trim().Split(',');
        friends = textFriendsName.text.Trim().Split(',');
        chatClient.AddFriends(friends);
        chatClient.SetOnlineStatus(2, "(добавляє друзів)");
    }

    public void RemoveFriendsButon()
    {
        //string[] friends = textFriendsName.text.Trim().Split(',');
        friends = textFriendsName.text.Trim().Split(',');
        chatClient.RemoveFriends(friends);
        chatClient.SetOnlineStatus(3, "(видаляє друзів)");
    }
}
