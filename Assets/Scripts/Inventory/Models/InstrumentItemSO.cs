using System;
using Inventory.Models;
using UnityEngine;

[CreateAssetMenu(fileName = "Data", menuName = "Inventory/Instrument")]
public class InstrumentItemSO : UsableItemSO
{
    public float Strength;
    public float Durability;
    public InstrumentType InstrumentType;
}
