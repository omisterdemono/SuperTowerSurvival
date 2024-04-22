using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TogglePowerUpPoints : MonoBehaviour
{
    [SerializeField] public TextMeshProUGUI pointsText;
    [SerializeField] private PowerUp powerUpUI;
    private int _points = 0;

    public int Points { get => _points;}

    public void Minus()
    {
        if (_points>=1)
        {
            _points--;

            pointsText.text = _points.ToString();
            powerUpUI.PowerUpPoints++;
        }
    }

    public void Plus()
    {
        if (powerUpUI.PowerUpPoints>0)
        {
            _points++;
            pointsText.text = _points.ToString();
            powerUpUI.PowerUpPoints--;
        }
    }
}
