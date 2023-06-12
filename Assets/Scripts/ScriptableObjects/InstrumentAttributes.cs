using UnityEngine;

[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/InstrumentAttributes")]
public class InstrumentAttributes : ScriptableObject
{
    public float Strength;
    public float Durability;
    public InstrumentType InstrumentType;
}
