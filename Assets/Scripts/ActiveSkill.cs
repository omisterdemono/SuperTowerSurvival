using Mirror;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements.Experimental;

public class ActiveSkill : NetworkBehaviour
{
    [SerializeField] private SkillAttributes _skillAttributes;
    [SerializeField] private MovementComponent _movementComponent;

    public string SkillName { get => _name; set => _name = value; }
    public bool IsReady { get => _isReady; set => _isReady = value; }
    public ISkill Skill { get; set; }
    
    private string _name;
    private float _cooldown;
    private float _castTime;

    private bool _isStarted;
    private float _passedTime;
    private float _castProgress;

    private bool _isReady;
    //[SerializeField] public Sprite icon;
    //[SerializeField] public Image castFill;
    //[SerializeField] public Text timeText;
    //[SerializeField] public CanvasGroup CastBar;

    public void Start()
    {
        _name = _skillAttributes.Name;
        _cooldown = _skillAttributes.Cooldown;
        _castTime = _skillAttributes.CastTime;

        _movementComponent = GetComponent<MovementComponent>();
        _passedTime = _cooldown;
    }

    public void Update()
    {
        if (_passedTime < _cooldown)
        {
            _passedTime += Time.deltaTime;
        }
        UseSkill();
    }

    public void UseSkill()
    {
        if (Input.GetMouseButtonDown(1) || Input.GetKeyDown(KeyCode.Escape))
        {
            _isReady = false;
        }
        if (_passedTime >= _cooldown || _isStarted)
        {
            if (_isReady)
            {
                if (Input.GetMouseButtonDown(0) && !_isStarted)
                {
                    if (_castTime != 0)
                    {
                        StartCast();
                    }
                    else
                    {
                        _isStarted = true;
                        _passedTime = 0;
                    }
                }
                if (_castProgress == _castTime || _movementComponent.MovementVector != Vector3.zero && _isStarted)
                {
                    FinishCast();
                }
            }
        }
        else
            _isReady = false;
    }

    public void Casting()
    {
        //CastBar.alpha = 1;
        //float rate = 1.0f / CastTime;
        //castFill.fillAmount = Mathf.Lerp(0, 1, _casting * rate);
        //timeText.text = _casting.ToString("0.0");
        _castProgress += 0.1f;
        if (_castProgress >= _castTime)
        {
            _castProgress = _castTime;
            //CastBar.alpha = 0;
        }
    }

    public virtual void StartCast()
    {
        InvokeRepeating("Casting", 0, 0.1f);
        _isStarted = true;
        _passedTime = 0;
    }

    public virtual void FinishCast()
    {
        CancelInvoke("Casting");
        _castProgress = 0;
        _isReady = false;
        //CastBar.alpha = 0;
        _isStarted = false;
        Skill.UseSkill();
    }
}
