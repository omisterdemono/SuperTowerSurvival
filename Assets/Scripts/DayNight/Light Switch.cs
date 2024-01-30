using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.GlobalIllumination;
using UnityEngine.Rendering.Universal;

public class LightSwitch : MonoBehaviour
{
    private WorldLight _worldLight;
    private Light2D _light;

    void Start()
    {
        _worldLight = FindObjectOfType<WorldLight>();
        WorldLight.OnNightChanged += WorldLight_OnNightChanged;
        _light = GetComponent<Light2D>();
        _light.intensity = 0;
    }

    private void WorldLight_OnNightChanged(object sender, System.EventArgs e)
    {
        if (_worldLight.isNight)
        {
            _light.intensity = 1;
        }
        else
        {
            _light.intensity = 0;
        }
        //throw new System.NotImplementedException();
    }

    void Update()
    {
        
    }
}
