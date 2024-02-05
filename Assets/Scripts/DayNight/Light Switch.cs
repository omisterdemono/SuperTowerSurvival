using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.GlobalIllumination;
using UnityEngine.Rendering.Universal;

public class LightSwitch : MonoBehaviour
{

    [SerializeField] private float _dayLightInnerRadius = 0;
    [SerializeField] private float _dayLightOuterRadius = 1;

    private WorldLight _worldLight;
    private Light2D _light;
    private float _lightInnerRadius;
    private float _lightOuterRadius;
    private float _lightActualOuterRadius;

    void Start()
    {
        _worldLight = FindObjectOfType<WorldLight>();
        WorldLight.OnNightChanged += WorldLight_OnNightChanged;
        _light = GetComponent<Light2D>();

        _lightInnerRadius = _light.pointLightInnerRadius;
        _lightOuterRadius = _light.pointLightOuterRadius;

        _light.intensity = 1;
        _light.pointLightOuterRadius = _dayLightOuterRadius;
        _light.pointLightInnerRadius = _dayLightInnerRadius;
    }

    private void WorldLight_OnNightChanged(object sender, System.EventArgs e)
    {
        if (_worldLight.isNight)
        {
            _light.pointLightOuterRadius = _lightOuterRadius;
            _light.pointLightInnerRadius = _lightInnerRadius;
            //_light.intensity = 1;
        }
        else
        {
            _light.pointLightOuterRadius = _dayLightOuterRadius;
            _light.pointLightInnerRadius = _dayLightInnerRadius;
            //_light.intensity = 0;
        }
        //throw new System.NotImplementedException();
    }

    void Update()
    {
        
    }
}
