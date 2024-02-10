using Mirror;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class WorldTimeDisplayer : MonoBehaviour
{
    [SerializeField] Sprite _imageSun;
    [SerializeField] Sprite _imageMoon;
    private WorldLight _worldLight;
    private TMP_Text _text;
    private Image childImage;
    private void Awake()
    {
        _text = GetComponent<TMP_Text>();
        _worldLight = FindObjectOfType<WorldLight>();
        //_worldLight = GetComponent<WorldLight>();
        childImage = this.GetComponentInChildren<Image>();
        WorldLight.OnNightChanged += WorldLight_OnNightChanged;
    }

    private void WorldLight_OnNightChanged(object sender, System.EventArgs e)
    {
        if (_worldLight.isNight)
        {
            childImage.sprite = _imageMoon;
        }
        else
        {
            childImage.sprite = _imageSun;
        }
    }

    void Start()
    {
        
    }

    void Update()
    {
        _text.SetText($"Day {_worldLight.GetDay()}\n{_worldLight.GetHour()}:{_worldLight.GetMinute()}");
    }
}
