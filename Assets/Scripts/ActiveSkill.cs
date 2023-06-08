using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements.Experimental;

public abstract class ActiveSkill : NetworkBehaviour, ISkill
{
    [SerializeField] public string SkillName;
    [SerializeField] public string SkillDescription;
    [SerializeField] private float _skillCooldown;

    public bool IsStarted;
    public float CastTime;
    public float PassedTime;
    private float _casting;

    //[SerializeField] public Sprite icon;
    //[SerializeField] public Image castFill;
    //[SerializeField] public Text timeText;
    //[SerializeField] public CanvasGroup CastBar;
    public bool IsReady;

    private GameObject _player;

    public void Start()
    {
        PassedTime = _skillCooldown;
    }

    public void Update()
    {
        if (PassedTime < _skillCooldown)
        {
            PassedTime += Time.deltaTime;
        }
    }

    public abstract void PowerUpSkillPoint();
   

    public virtual void UseSkill()
    {
        if (Input.GetMouseButtonDown(1) || Input.GetKeyDown(KeyCode.Escape))
        {
            IsReady = false;
        }
        if (PassedTime >= _skillCooldown || IsStarted)
        {
            if (IsReady)
            {
                if (Input.GetMouseButtonDown(0) && !IsStarted)
                {
                    if (CastTime != 0)
                    {
                        StartCast();
                    }
                    else
                    {
                        IsStarted = true;
                        PassedTime = 0;
                    }
                }
                if (_casting == CastTime || _player.GetComponent<MovementComponent>().MovementVector != Vector3.zero && IsStarted)
                {
                    FinishCast();
                }
            }
        }
        else
            IsReady = false;
    }

    public void Casting()
    {
        //CastBar.alpha = 1;
        //float rate = 1.0f / CastTime;
        //castFill.fillAmount = Mathf.Lerp(0, 1, _casting * rate);
        //timeText.text = _casting.ToString("0.0");
        _casting += 0.1f;
        if (_casting >= CastTime)
        {
            _casting = CastTime;
            //CastBar.alpha = 0;
        }
    }

    public virtual void StartCast()
    {
        InvokeRepeating("Casting", 0, 0.1f);
        IsStarted = true;
        PassedTime = 0;
    }

    public virtual void FinishCast()
    {
        CancelInvoke("Casting");
        _casting = 0;
        IsReady = false;
        //CastBar.alpha = 0;
        IsStarted = false;
    }
}
