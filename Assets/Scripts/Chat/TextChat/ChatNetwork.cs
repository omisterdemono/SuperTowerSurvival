using Mirror;
using Mirror.Examples.Chat;
using System;
using UnityEngine;

public class ChatNetwork : NetworkBehaviour
{
    // Singleton for easy access from UI
    public static ChatNetwork Instance;

    // Synced list of messages; auto-replicates to all clients
    public readonly SyncList<string> messages = new SyncList<string>();

    [Header("Limits")]
    [SerializeField] private int maxMessages = 200;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else if (Instance != this) Destroy(gameObject);
    }

    public override void OnStartClient()
    {
        // Update UI when list changes (add/remove/insert/clear).
        messages.Callback += OnMessagesChanged;
    }

    public override void OnStopClient()
    {
        messages.Callback -= OnMessagesChanged;
    }

    private void OnMessagesChanged(SyncList<string>.Operation op, int index, string oldItem, string newItem)
    {
        // Any client updates its UI when list changes
        if (ChatUI.Instance != null)
            ChatUI.Instance.Refresh(messages);
    }

    [Command(requiresAuthority = false)]
    public void CmdSendMessage(string text, string sender)
    {
        if (string.IsNullOrWhiteSpace(text)) return;

        // Trim + cap length for safety
        text = text.Trim();
        if (text.Length > 512) text = text.Substring(0, 512);

        var stamp = DateTime.Now.ToString("HH:mm");
        var final = $"[{stamp}] {sender}: {text}";

        // Keep list bounded
        if (messages.Count >= maxMessages)
            messages.RemoveAt(0);

        messages.Add(final);
    }
}
