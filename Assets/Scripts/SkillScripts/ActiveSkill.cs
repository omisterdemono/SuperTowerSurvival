using Mirror;
using System;
using System.Collections;
using System.Collections.Generic;
using Infrastructure.UI;
using TMPro;
using Unity.IO.LowLevel.Unsafe;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements.Experimental;

public class ActiveSkill : NetworkBehaviour
{
    [SerializeField] protected SkillAttributes _skillAttributes;
    
    public SkillButton SkillButton { get; set; }

    protected float Level = 0;

    public bool IsReady
    {
        get => _isReady;
        set => _isReady = value;
    }

    public SkillAttributes SkillAttributes => _skillAttributes;

    private MovementComponent _movementComponent;
    protected ISkill Skill;
    
    private float _cooldown;
    private float _castTime;

    private bool _isStarted;
    private float _passedTime;
    private float _castProgress;

    private bool _isReady;
    
    private GameObject _castBar;
    private CanvasGroup _castBarCanvasGroup;
    private Image _castBarImageFill;
    private Text _castBarText;
    
    public void Start()
    {
        _cooldown = _skillAttributes.Cooldown;
        _castTime = _skillAttributes.CastTime;

        _movementComponent = GetComponent<MovementComponent>();
        _passedTime = _cooldown;
        _castBar = GameObject.FindGameObjectWithTag("CastFill");
        
        if (isOwned)
        {
            _castBarCanvasGroup = _castBar.GetComponent<CanvasGroup>();
            _castBarImageFill = _castBar.transform.GetChild(0).GetComponent<Image>();
            _castBarText = _castBar.GetComponentInChildren<Text>();
        }
    }

    public void Update()
    {
        if (!isOwned) return;

        if (_passedTime < _cooldown)
        {
            _passedTime += Time.deltaTime;
            SkillButton.SkillCooldown.fillAmount = 1 - 1 / _cooldown * _passedTime;
        }

        if (IsReady && CanUseSkill())
        {
            SkillButton.SkillIcon.color = new Color(1, 1, 1, 0.6f);
            UseSkill();
        }
        else
        {
            IsReady = false;
        }
    }

    protected virtual bool CanUseSkill()
    {
        return true;
    }

    public void UseSkill()
    {
        if (Input.GetMouseButtonDown(1) || Input.GetKeyDown(KeyCode.Escape))
        {
            _isReady = false;
            SkillButton.SkillIcon.color = new Color(1, 1, 1, 1);
        }
        if (_passedTime >= _cooldown || _isStarted)
        {
            if (_isReady)
            {
                if (Input.GetMouseButtonDown(0) && !_isStarted || _castTime==0)
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
                if (_movementComponent.MovementVector != Vector3.zero && _isStarted && _castTime != 0)
                {
                    FinishCast();
                }
                else if (_castProgress == _castTime || _castTime == 0)
                {
                    FinishCastPositive();
                }
            }
        }
        else
        {
            _isReady = false;
            SkillButton.SkillIcon.color = new Color(1, 1, 1, 1);
        }
    }

    public void Casting()
    {
        _castBarCanvasGroup.alpha = 1;
        float rate = 1.0f / _skillAttributes.CastTime;
        _castBarImageFill.fillAmount = Mathf.Lerp(0, 1, _castProgress * rate);
        _castBarText.text = _castProgress.ToString("0.0");
        _castProgress += 0.1f;
        if (_castProgress >= _castTime)
        {
            _castProgress = _castTime;
            _castBarCanvasGroup.alpha = 0;
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
        _castBarCanvasGroup.alpha = 0;
        _isStarted = false;
        SkillButton.SkillIcon.color = new Color(1, 1, 1, 1);
    }

    public virtual void FinishCastPositive()
    {
        FinishCast();
    }
}
