using UnityEngine;

public class CooldownComponent
{
    public float CooldownSeconds { get; set; }

    private float _timeToNextPerform;

    public bool CanPerform => _timeToNextPerform == 0;

    public void ResetCooldown()
    {
        _timeToNextPerform = CooldownSeconds;
    }

    public void HandleCooldown()
    {
        if (_timeToNextPerform == 0)
        {
            return;
        }

        if (_timeToNextPerform < 0)
        {
            _timeToNextPerform = 0;
            return;
        }

        _timeToNextPerform -= Time.deltaTime;
    }
}
