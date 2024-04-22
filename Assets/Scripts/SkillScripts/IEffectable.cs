using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IEffectable
{
    public void ApplyEffect(StatusEffect effect);
    public void HandleEffects();
    public void RemoveEffect(StatusEffect effect);
}
