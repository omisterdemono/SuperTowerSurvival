using UnityEngine;

public class ChargeComponent
{
    public float ChargeProgress => _chargeProgressSeconds / MaxChargeSeconds;
    public float MinProgressToShotSeconds { get; set; }
    public float MaxChargeSeconds { get; set; }
    public bool IsCharging => _chargeProgressSeconds > 0;
    public bool CanShoot => _chargeProgressSeconds >= MinProgressToShotSeconds;
    public bool MaxCharged => _chargeProgressSeconds >= MaxChargeSeconds;

    private float _chargeProgressSeconds;

    public void HandleCharge()
    {
        if (_chargeProgressSeconds > MaxChargeSeconds)
        {
            _chargeProgressSeconds = MaxChargeSeconds;
        }
        _chargeProgressSeconds += Time.deltaTime;
    }

    public void ResetCharge()
    {
        _chargeProgressSeconds = 0;
    }
}
