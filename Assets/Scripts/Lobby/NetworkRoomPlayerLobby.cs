using JetBrains.Annotations;
using Mirror;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class NetworkRoomPlayerLobby : NetworkBehaviour
{
    [Header("UI")]
    [SerializeField] private GameObject lobbyUI = null;
    [SerializeField] private TMP_Text[] playerNameTexts = new TMP_Text[5];
    [SerializeField] private TMP_Text[] playerReadyTexts = new TMP_Text[5];
    [SerializeField] private Button startGameButton = null;
    [SerializeField] private Button readyButton = null;

    [SyncVar]
    public int CurrentCharacter = 0;

    [SerializeField] private List<Button> _choseCharacterButton = new List<Button>();

    [SyncVar(hook = nameof(HandleDisplayNameChanged))]
    public string DisplayName = "Loading...";
    [SyncVar(hook = nameof(HandleReadyStatusChanged))]
    public bool IsReady = false;

    private bool isLeader;

    public bool IsLeader 
    {
        set 
        {
            isLeader = value;
            startGameButton.gameObject.SetActive(true);
        } 
    }

    private NetworkManagerLobby room;

    public NetworkManagerLobby Room 
    { 
        get
        {
            if (room != null) return room;

            return room = NetworkManager.singleton as NetworkManagerLobby;
        }
    }

    private void Update()
    {
        if (IsReady)
        {
            var colors = readyButton.colors;
            colors.normalColor = Color.red;
            colors.highlightedColor = Color.red;
            colors.selectedColor = Color.red;
            readyButton.colors = colors;
            readyButton.GetComponentInChildren<TMP_Text>().text = "Not Ready";
        }
        else
        {
            var colors = readyButton.colors;
            colors.normalColor = Color.green;
            colors.highlightedColor = Color.green;
            colors.selectedColor = Color.green;
            readyButton.colors = colors;
            readyButton.GetComponentInChildren<TMP_Text>().text = "Ready";
        }
    }

    public override void OnStartAuthority()
    {
        CmdSetDisplayName(PlayerNameInput.DisplayName);
        lobbyUI.SetActive(true);
    }

    public override void OnStartClient()
    {
        Room.RoomPlayers.Add(this);

        UpdateDisplay();
    }

    public override void OnStopClient()
    {
        Room.RoomPlayers.Remove(this);

        UpdateDisplay();
    }

    public void HandleReadyStatusChanged(bool oldValue, bool newValue) => UpdateDisplay();
    public void HandleDisplayNameChanged(string oldValue, string newValue) => UpdateDisplay();

    private void UpdateDisplay()
    {
        if (!hasAuthority)
        {
            foreach (var player in Room.RoomPlayers)
            {
                if (player.hasAuthority)
                {
                    player.UpdateDisplay();
                    break;
                }
            }

            return;
        }


        for (int i = 0; i < playerNameTexts.Length; i++)
        {
            playerNameTexts[i].text = "Waiting for player...";
            playerReadyTexts[i].text = string.Empty;
        }

        for (int i = 0; i < Room.RoomPlayers.Count; i++)
        {
            playerNameTexts[i].text = Room.RoomPlayers[i].DisplayName;
            playerReadyTexts[i].text = Room.RoomPlayers[i].IsReady ?
                "<color=green>Ready</color>" :
                "<color=red>Not Ready</color>";
        }
    }

    public void HandleReadyToStart(bool readyToStart)
    {
        if (!isLeader) return;

        startGameButton.interactable = readyToStart;
    }

    [Command]
    public void CmdSetDisplayName(string displayName)
    {
        DisplayName = displayName;
    }

    [Command]
    public void CmdReadyUp()
    {
        IsReady = !IsReady;

        Room.NotifyPlayersOfReadyState();
    }

    [Command]
    public void CmdStartGame()
    {
        if (Room.RoomPlayers[0].connectionToClient != connectionToClient) return;

        Room.StartGame();
    }

    [Command]
    public void UpdateChosenCharacter(int chosenCharacter)
    {
        CurrentCharacter = chosenCharacter;
    }

    public void ChangeCharacter(int characterIndex)
    {
        if (!IsReady)
        {
            _choseCharacterButton[CurrentCharacter].GetComponent<Outline>().effectColor = Color.white;
            _choseCharacterButton[characterIndex].GetComponent<Outline>().effectColor = Color.green;
            UpdateChosenCharacter(characterIndex);
        }
    }
}
