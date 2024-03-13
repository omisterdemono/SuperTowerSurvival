using Inventory.Models;
using UnityEngine;

[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/InstrumentAttributes")]
public class InstrumentAttributes : UsableItemSO
{
    public float Strength;
    public float Durability;
    public InstrumentType InstrumentType;
}
