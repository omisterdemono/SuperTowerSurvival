using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerNameInput : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private TMP_InputField _nameInputField = null;
    [SerializeField] private Button _continueButton = null;

    public static string DisplayName { get; private set; }
    private const string PlayerPrefsNameKey = "PlayerName";

    private void Start() => SetUpInputField();

    private void SetUpInputField()
    {
        if (!PlayerPrefs.HasKey(PlayerPrefsNameKey)) return;
        
        string defaultName = PlayerPrefs.GetString(PlayerPrefsNameKey);

        _nameInputField.text = defaultName;

        SetPlayerName(defaultName);
    }

    public void SetPlayerName(string name)
    {
        DisplayName = _nameInputField.text;

        PlayerPrefs.SetString(PlayerPrefsNameKey, DisplayName);
    }

    public void CloseGame()
    {
        Application.Quit();
    }
}
