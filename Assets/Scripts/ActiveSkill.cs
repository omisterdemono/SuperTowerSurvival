using Mirror;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements.Experimental;

public class ActiveSkill : NetworkBehaviour
{
    [SerializeField] protected SkillAttributes _skillAttributes;
    private MovementComponent _movementComponent;

    public string SkillName { get => _name; set => _name = value; }
    public bool IsReady { get => _isReady; set => _isReady = value; }
    protected ISkill Skill;
    
    private string _name;
    private float _cooldown;
    private float _castTime;

    private bool _isStarted;
    private float _passedTime;
    private float _castProgress;

    private bool _isReady;
    
    private GameObject _castBar;
    private GameObject _skillHolder;

    [SerializeField] private GameObject _skillButton;

    public void Start()
    {
        _name = _skillAttributes.Name;
        _cooldown = _skillAttributes.Cooldown;
        _castTime = _skillAttributes.CastTime;

        _movementComponent = GetComponent<MovementComponent>();
        _passedTime = _cooldown;

        _castBar = GameObject.FindGameObjectWithTag("CastFill");
        _skillHolder = GameObject.FindGameObjectWithTag("SkillHolder");

        _skillButton.GetComponent<Image>().sprite = _skillAttributes.SkillIcon;

        if (isOwned)
        {
            _skillButton = Instantiate(_skillButton);
            _skillButton.gameObject.transform.SetParent(_skillHolder.transform);
        }
    }

    public void Update()
    {
        if (!isLocalPlayer) return;

        if (_passedTime < _cooldown)
        {
            _passedTime += Time.deltaTime;
            _skillButton.transform.GetChild(0).GetComponent<Image>().fillAmount = 1 - 1/_cooldown * _passedTime;
        }

        if (IsReady)
        {
            _skillButton.GetComponent<Image>().color = new Color(1, 1, 1, 0.6f);
            UseSkill();
        }
    }

    public void UseSkill()
    {
        if (Input.GetMouseButtonDown(1) || Input.GetKeyDown(KeyCode.Escape))
        {
            _isReady = false;
            _skillButton.GetComponent<Image>().color = new Color(1, 1, 1, 1);
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
                if (_movementComponent.MovementVector != Vector3.zero && _isStarted)
                {
                    FinishCast();
                }
                else if (_castProgress == _castTime)
                {
                    FinishCastPositive();
                }
            }
        }
        else
        {
            _isReady = false;
            _skillButton.GetComponent<Image>().color = new Color(1, 1, 1, 1);
        }
    }

    public void Casting()
    {
        _castBar.GetComponent<CanvasGroup>().alpha = 1;
        float rate = 1.0f / _skillAttributes.CastTime;
        _castBar.transform.GetChild(0).GetComponent<Image>().fillAmount = Mathf.Lerp(0, 1, _castProgress * rate);
        _castBar.GetComponentInChildren<Text>().text = _castProgress.ToString("0.0");
        _castProgress += 0.1f;
        if (_castProgress >= _castTime)
        {
            _castProgress = _castTime;
            _castBar.GetComponent<CanvasGroup>().alpha = 0;
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
        _castBar.GetComponent<CanvasGroup>().alpha = 0;
        _isStarted = false;
        _skillButton.GetComponent<Image>().color = new Color(1, 1, 1, 1);
    }

    public virtual void FinishCastPositive()
    {
        FinishCast();
    }
}
