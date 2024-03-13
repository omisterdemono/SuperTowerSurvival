using System;
using Inventory.Models;
using UnityEngine;

[CreateAssetMenu(fileName = "Data", menuName = "Inventory/Instument")]
public class InstrumentItemSO : UsableItemSO
{
    public float Strength;
    public float Durability;
    public InstrumentType InstrumentType;
}
