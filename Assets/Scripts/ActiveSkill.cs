using Mirror;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements.Experimental;

public class ActiveSkill : NetworkBehaviour
{
    [SerializeField] private string _skillName;
    [SerializeField] private string _skillDescription;
    [SerializeField] private float _skillCooldown;
    [SerializeField] private float _castTime;

    private bool _isStarted;
    private float _passedTime;
    private float _casting;

    //[SerializeField] public Sprite icon;
    //[SerializeField] public Image castFill;
    //[SerializeField] public Text timeText;
    //[SerializeField] public CanvasGroup CastBar;
    private bool _isReady;

    [SerializeField] private MovementComponent _movementComponent;
    private ISkill _skill;

    public string SkillName { get => _skillName; set => _skillName = value; }
    public bool IsReady { get => _isReady; set => _isReady = value; }
    public ISkill Skill { get => _skill; set => _skill = value; }

    public void Start()
    {
        _movementComponent = GetComponent<MovementComponent>();
        _passedTime = _skillCooldown;
    }

    public void Update()
    {
        if (_passedTime < _skillCooldown)
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
        if (_passedTime >= _skillCooldown || _isStarted)
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
                if (_casting == _castTime || _movementComponent.MovementVector != Vector3.zero && _isStarted)
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
        _casting += 0.1f;
        if (_casting >= _castTime)
        {
            _casting = _castTime;
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
        _casting = 0;
        _isReady = false;
        //CastBar.alpha = 0;
        _isStarted = false;
        _skill.UseSkill();
    }
}
