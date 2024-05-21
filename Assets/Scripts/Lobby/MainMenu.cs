using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenu : MonoBehaviour
{
    [SerializeField] private NetworkManagerLobby _networkManager = null;

    [Header("UI")]
    [SerializeField] private GameObject _landingPagePanel = null;

    private void Start()
    {
        _networkManager = FindObjectOfType<NetworkManagerLobby>();
    }

    public void HostLobby()
    {
        _networkManager.StartHost();
        _landingPagePanel.SetActive(false);
    }
}
