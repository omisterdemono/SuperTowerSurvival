using UnityEngine;

public class CooldownComponent
{
    public float CooldownSeconds { get; set; }

    private float _timeToNextHit;

    public bool CanHit => _timeToNextHit == 0;

    public void ResetCooldown()
    {
        _timeToNextHit = CooldownSeconds;
    }

    public void HandleCooldown()
    {
        if (_timeToNextHit == 0)
        {
            return;
        }

        if (_timeToNextHit < 0)
        {
            _timeToNextHit = 0;
            return;
        }

        _timeToNextHit -= Time.deltaTime;
    }
}
