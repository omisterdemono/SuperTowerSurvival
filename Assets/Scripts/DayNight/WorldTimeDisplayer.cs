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
    [SerializeField] TMP_Text _DayText;
    [SerializeField] TMP_Text _TimeText;
    [SerializeField] Image childImage;
    private void Start()
    {
        //_text = GetComponent<TMP_Text>();
        //childImage = this.GetComponentInChildren<Image>();
        _worldLight = FindObjectOfType<WorldLight>();
        WorldLight.OnNightChanged += WorldLight_OnNightChanged;
    }

    private void WorldLight_OnNightChanged(object sender, System.EventArgs e)
    {
        if (_worldLight == null)
        {
            _worldLight = FindObjectOfType<WorldLight>();
            return;
        }
        
        //if (_worldLight.isNight)
        //{
        //    childImage.sprite = _imageMoon;
        //}
        //else
        //{
        //    childImage.sprite = _imageSun;
        //}
    }

    private void Update()
    {
        if (_worldLight == null)
        {
            _worldLight = FindObjectOfType<WorldLight>();
            return;
        }
        _DayText.SetText(_worldLight.GetDay().ToString());
        _TimeText.SetText($"{_worldLight.GetHour()}:{_worldLight.GetMinute()}");
        //_TimeText.SetText($"{1}:{2}", _worldLight.GetHour(), _worldLight.GetMinute());
        //_text.SetText($"Day {_worldLight.GetDay()}\n{_worldLight.GetHour()}:{_worldLight.GetMinute()}");
    }
}
