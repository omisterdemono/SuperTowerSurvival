using Mirror;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

[RequireComponent(typeof(Light2D))]
public class WorldLight : NetworkBehaviour
{
    [SerializeField] private float dayBeginHourOffset = 6;
    [SyncVar] public float generalTime;
    [SyncVar] public float currentTime;
    [SyncVar] public float dayLengthMinutes;
    [SyncVar] public bool isNight;
    public Action OnIsNightChanged;
    public static event EventHandler OnNightChanged;


    [SyncVar] private float midday;
    [SyncVar] private float gameHour;
    [SyncVar] private float translateTime;
    [SyncVar] private float displayTime;
    [SyncVar] private int dayNumber;


    [SerializeField] private Gradient gradient;
    private Light2D _light;

    private void Start()
    {
        midday = dayLengthMinutes * 60 / 2;
        gameHour = midday / 12;
        dayNumber = 1;
        generalTime = 0;
        isNight = false;
        //currentTime = 6 * gameHour;
        //SetTime(0.5f);
    }

    void Awake()
    {
        _light = GetComponent<Light2D>();
    }

    void Update()
    {
        if (!isLocalPlayer)
        {
            ServerCalculation();
        }
        ClientUpload();
    }

    [Server]
    void ServerCalculation()
    {
        generalTime += 1 * Time.deltaTime;
        currentTime += 1 * Time.deltaTime;
        //translateTime = ((currentTime - dayBeginHourOffset) / (midday * 2));
        translateTime = (currentTime / (midday * 2));

        if (translateTime >= 1)
        {
            currentTime -= midday * 2;
            dayNumber++;
        }

        //_light.color = gradient.Evaluate(translateTime);
        //displayTime = translateTime * 24f;

        if (currentTime > midday + 2 * gameHour && !isNight)
        //if ((currentTime > 20 * gameHour || currentTime < 6 * gameHour) && !isNight)
        {
            isNight = true;
            //OnIsNightChanged?.Invoke();
            OnNightChanged?.Invoke(this, EventArgs.Empty);

        }
        if (currentTime < midday + 2 * gameHour && isNight)
        //if ((currentTime > 6 * gameHour || currentTime < 20 * gameHour) && isNight)
        {
            isNight = false;
            OnIsNightChanged?.Invoke();
            OnNightChanged?.Invoke(this, EventArgs.Empty);
        }

    }

    [Client]
    void ClientUpload()
    {
        _light.color = gradient.Evaluate(translateTime);
        displayTime = translateTime * 24f;
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
        if (min > 59)
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
