using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/SkillAttributes")]

public class SkillAttributes : ScriptableObject
{
    public string Name;
    public string SkillDescription;
    public float Cooldown;
    public float CastTime;
    public Sprite SkillIcon;
}
