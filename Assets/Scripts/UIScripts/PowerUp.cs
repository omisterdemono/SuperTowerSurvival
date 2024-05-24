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
    private PowerUpStruct _powerUpPointsOnNewDay;
    private int _powerUpPoints;
    private WorldLight _light;
    private CanvasGroup _canvasGroup;

    public int PowerUpPoints
    {
        get
        {
            return _powerUpPoints;
        }
        set
        {
            _powerUpPoints = value;
            powerUpPointsText.text = _powerUpPoints.ToString();
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

    private void SetVisibility(bool visible)
    {
        if (visible)
        {
            _canvasGroup.alpha = 1;
            _canvasGroup.interactable = true;
            _canvasGroup.blocksRaycasts = true;
        }
        else
        {
            _canvasGroup.alpha = 0;
            _canvasGroup.interactable = false;
            _canvasGroup.blocksRaycasts = false;
        }
    }

    public void ApplyPoints()
    {
        SetVisibility(false);
        Character character = FindObjectsOfType<Character>().FirstOrDefault(c => c.isOwned);

        _powerUp.ActiveSkill1 = ActiveSkill1.Points - _powerUpPointsOnNewDay.ActiveSkill1;
        _powerUp.ActiveSkill2 = ActiveSkill2.Points - _powerUpPointsOnNewDay.ActiveSkill2;
        _powerUp.PassiveSkill = PassiveSkill.Points - _powerUpPointsOnNewDay.PassiveSkill;
        _powerUp.Health = Health.Points - _powerUpPointsOnNewDay.Health;
        _powerUp.Speed = Speed.Points - _powerUpPointsOnNewDay.Speed;
        _powerUp.AttackDamage = AttackDamage.Points - _powerUpPointsOnNewDay.AttackDamage;
        _powerUp.BuildSpeed = BuildSpeed.Points - _powerUpPointsOnNewDay.BuildSpeed;

        character?.PowerUp(_powerUp);
    }

    private void setNewPoints()
    {
        

        
    }

    private void Start()
    {
        PowerUpPoints = 5;
        powerUpPointsText.text = _powerUpPoints.ToString();
        _light = FindObjectOfType<WorldLight>();
        _canvasGroup = GetComponent<CanvasGroup>();
        WorldLight.OnNightChanged += WorldLight_OnNightChanged;
    }

    private void WorldLight_OnNightChanged(object sender, System.EventArgs e)
    {
        if(_light == null)
        {
            _light = FindObjectOfType<WorldLight>();
        }

        if (!_light.isNight)
        {
            SetVisibility(true);
            PowerUpPoints += _light.GetDay();

            _powerUpPointsOnNewDay.ActiveSkill1 = ActiveSkill1.Points;
            _powerUpPointsOnNewDay.ActiveSkill2 = ActiveSkill2.Points;
            _powerUpPointsOnNewDay.PassiveSkill = PassiveSkill.Points;
            _powerUpPointsOnNewDay.Health = Health.Points;
            _powerUpPointsOnNewDay.Speed = Speed.Points;
            _powerUpPointsOnNewDay.AttackDamage = AttackDamage.Points;
            _powerUpPointsOnNewDay.BuildSpeed = BuildSpeed.Points;
            //powerUpPointsText.text = _powerUpPoints.ToString();
        }
    }
}
