using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements.Experimental;

public abstract class ActiveSkill : NetworkBehaviour, ISkill
{
    [SerializeField] private string SkillName;
    [SerializeField] private string SkillDescription;
    [SerializeField] private float _skillCooldown;
    [SerializeField] private float _castTime;

    private bool IsStarted;
    private float _passedTime;
    private float _casting;

    //[SerializeField] public Sprite icon;
    //[SerializeField] public Image castFill;
    //[SerializeField] public Text timeText;
    //[SerializeField] public CanvasGroup CastBar;
    private bool _isReady;

    private GameObject _player;

    public void Start()
    {
        _passedTime = _skillCooldown;
    }

    public void Update()
    {
        if (_passedTime < _skillCooldown)
        {
            _passedTime += Time.deltaTime;
        }
    }

    public abstract void PowerUpSkillPoint();
   

    public virtual void UseSkill()
    {
        if (Input.GetMouseButtonDown(1) || Input.GetKeyDown(KeyCode.Escape))
        {
            _isReady = false;
        }
        if (_passedTime >= _skillCooldown || IsStarted)
        {
            if (_isReady)
            {
                if (Input.GetMouseButtonDown(0) && !IsStarted)
                {
                    if (_castTime != 0)
                    {
                        StartCast();
                    }
                    else
                    {
                        IsStarted = true;
                        _passedTime = 0;
                    }
                }
                if (_casting == _castTime || _player.GetComponent<MovementComponent>().MovementVector != Vector3.zero && IsStarted)
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
        IsStarted = true;
        _passedTime = 0;
    }

    public virtual void FinishCast()
    {
        CancelInvoke("Casting");
        _casting = 0;
        _isReady = false;
        //CastBar.alpha = 0;
        IsStarted = false;
    }
}
