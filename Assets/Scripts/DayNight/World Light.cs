using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

[RequireComponent(typeof(Light2D))]
public class WorldLight : MonoBehaviour
{
    public float duration = 5f;

    public float generalTime;
    public float currentTime;
    public float dayLengthMinutes;
    public bool isNight;
    public Action OnIsNightChanged;

    private float midday;
    private float gameHour;
    private float translateTime;
    private float displayTime;
    private int dayNumber;


    [SerializeField] private Gradient gradient;
    private Light2D _light;

    private void Start()
    {
        midday = dayLengthMinutes * 60 / 2;
        gameHour = midday / 12;
        dayNumber = 1;
        generalTime = 0;
        isNight = false;
        SetTime(0.5f);
    }

    void Awake()
    {
        _light = GetComponent<Light2D>();
    }

    void Update()
    {
        generalTime += 1 * Time.deltaTime;
        currentTime += 1 * Time.deltaTime;
        translateTime = (currentTime / (midday * 2));

        if (translateTime >= 1)
        {
            currentTime -= midday * 2;
            dayNumber++;
        }

        _light.color = gradient.Evaluate(translateTime);
        displayTime = translateTime * 24f;

        if(currentTime > midday + 2 * gameHour && !isNight)
        {
            isNight = true;
        }
        if(currentTime < midday + 2 * gameHour && isNight)
        {
            isNight = false;
            OnIsNightChanged?.Invoke();
        }

    }

    public int GetDay()
    {
        return dayNumber;
    }

    public int GetHour()
    {
        int hour = (int)Mathf.FloorToInt(displayTime);
        if (hour > 23)
        {
            hour = 23;
        }
        return hour;
    }

    public int GetMinute()
    {
        float decimalPart = displayTime - (int)displayTime;
        int min = Mathf.RoundToInt(decimalPart * 60);
        if(min > 59)
        {
            min = 59;
        }
        //int roundedMin = (min / 10) * 10;

        return min;
    }

    public void SetTime(float dayRatio)
    {
        float setTime = (midday * 2) * dayRatio;
        generalTime = setTime;
        currentTime = setTime;
    }
}
