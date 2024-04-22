using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObjects/StatusEffect")]
public class StatusEffect : ScriptableObject
{
    public string Name;
    public Sprite Icon;
    public EEffect EffectType;
    public EApplyType ApplyType;
    public float HandleTime;
    public float Value;
}
