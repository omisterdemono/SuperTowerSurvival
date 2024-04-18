using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public struct PowerUpStruct
{
    public int ActiveSkill1;
    public int ActiveSkill2;
    public int PassiveSkill;
    public int Health;
    public int Speed;
    public int AttackDamage;
    public int BuildSpeed;
}


public class PowerUp : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI powerUpPointsText;
    [SerializeField] private Button _applyButton;
    [SerializeField] private TogglePowerUpPoints ActiveSkill1;
    [SerializeField] private TogglePowerUpPoints ActiveSkill2;
    [SerializeField] private TogglePowerUpPoints PassiveSkill;
    [SerializeField] private TogglePowerUpPoints Health;
    [SerializeField] private TogglePowerUpPoints Speed;
    [SerializeField] private TogglePowerUpPoints AttackDamage;
    [SerializeField] private TogglePowerUpPoints BuildSpeed;
    private PowerUpStruct _powerUp;
    private int _powerUpPoints;

    public int PowerUpPoints
    {
        get
        {
            return _powerUpPoints;
        }
        set
        {
            _powerUpPoints = value;
            powerUpPointsText.text = "Power Up Points: " + _powerUpPoints;
            if (_powerUpPoints == 0)
            {
                _applyButton.interactable = true;
            }
            else
            {
                _applyButton.interactable = false;
            }
        }
    }

    private void ApplyPoints()
    {
        Character character = FindObjectsOfType<Character>().First(c => c.isOwned);
        _powerUp.ActiveSkill1 = ActiveSkill1.Points;
        _powerUp.ActiveSkill2 = ActiveSkill2.Points;
        _powerUp.PassiveSkill = PassiveSkill.Points;
        _powerUp.Health = Health.Points;
        _powerUp.Speed = Speed.Points;
        _powerUp.AttackDamage = AttackDamage.Points;
        _powerUp.BuildSpeed = BuildSpeed.Points;
        character.PowerUp(_powerUp);
    }

    private void Start()
    {
        //zaglushka
        PowerUpPoints = 5;
        powerUpPointsText.text = "Power Up Points: " + _powerUpPoints;
    }

    private void OnNewDay()
    {
        //zaglushka
        PowerUpPoints = 5;
        powerUpPointsText.text = "Power Up Points: " + _powerUpPoints;
    }
}
