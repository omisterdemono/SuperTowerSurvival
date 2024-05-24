using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class JoinLobbyMenu : MonoBehaviour
{
    [SerializeField] private NetworkManagerLobby _networkManagerLobby = null;

    [Header("UI")]
    [SerializeField] private GameObject _landingPagePanel = null;

    [SerializeField] private TMP_InputField _ipAddressInputField = null;

    [SerializeField] private Button _joinButton = null;

    private void OnEnable()
    {
        NetworkManagerLobby.OnClientConnected += HandleClientConnected;
        NetworkManagerLobby.OnClientDisconnected += HandleClientDisconnected;
    }

    private void OnDisable()
    {
        NetworkManagerLobby.OnClientConnected -= HandleClientConnected;
        NetworkManagerLobby.OnClientDisconnected -= HandleClientDisconnected;
    }

    private void Start()
    {
        _networkManagerLobby = FindObjectOfType<NetworkManagerLobby>();
    }

    public void JoinLobby()
    {
        string ipAddress = _ipAddressInputField.text;

        _networkManagerLobby.networkAddress = ipAddress;
        _networkManagerLobby.StartClient();

        _joinButton.interactable = false;
    }

    private void HandleClientConnected()
    {
        _joinButton.interactable = true;

        gameObject.SetActive(false);
        _landingPagePanel.SetActive(false);
    }

    private void HandleClientDisconnected()
    {
        _joinButton.interactable = false;
    }
}
