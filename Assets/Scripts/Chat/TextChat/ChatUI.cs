using Mirror;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ChatUI : MonoBehaviour
{
    public static ChatUI Instance;
    private const string PlayerPrefsNameKey = "PlayerName";

    [Header("UI References")]
    [SerializeField] private TMP_InputField inputField;
    [SerializeField] private TextMeshProUGUI logText;    // Multi-line text in a ScrollView
    [SerializeField] private ScrollRect scrollRect;
    [SerializeField] private Button sendButton;

    [Header("Player")]
    [SerializeField] private string playerName;

    public TMP_InputField InputField { get => inputField;}

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else if (Instance != this) Destroy(gameObject);

        if (string.IsNullOrWhiteSpace(playerName))
            playerName = PlayerPrefs.GetString(PlayerPrefsNameKey);
    }

    private void Start()
    {
        if (sendButton != null)
            sendButton.onClick.AddListener(SendFromUI);

        // Submit on Enter
        if (inputField != null)
            inputField.onSubmit.AddListener(_ => SendFromUI());

        // If we joined late, render existing messages
        if (ChatNetwork.Instance != null)
            Refresh(ChatNetwork.Instance.messages);
    }

    public void Refresh(System.Collections.Generic.IList<string> messages)
    {
        if (logText == null) return;

        // Build log
        System.Text.StringBuilder sb = new System.Text.StringBuilder();
        for (int i = 0; i < messages.Count; i++)
        {
            sb.AppendLine(messages[i]);
        }
        logText.text = sb.ToString();

        // Auto-scroll to bottom next frame
        if (scrollRect != null)
            Canvas.ForceUpdateCanvases();
        if (scrollRect != null)
        {
            scrollRect.verticalNormalizedPosition = 0f;
            Canvas.ForceUpdateCanvases();
        }
    }

    private void SendFromUI()
    {
        if (inputField == null || string.IsNullOrWhiteSpace(inputField.text))
            return;

        if (ChatNetwork.Instance == null)
            return;

        // Only allow sending once connected as client or host
        if (!NetworkClient.active)
            return;

        ChatNetwork.Instance.CmdSendMessage(inputField.text, playerName);
        inputField.text = string.Empty;
        inputField.ActivateInputField();
        inputField.Select();
    }
}
