using System;
using Assets.Scripts.Weapons;
using UnityEngine;

public class EquipSlot : MonoBehaviour
{
    private Transform _childToFlip;
    public IEquipable _equipable;
    private bool _wasFlipped = false;

    private void Awake()
    {
        ChangeRotatingChild(0);
    }

    public void ChangeRotatingChild(int index)
    {
        _childToFlip = gameObject.transform.GetChild(index);
        _equipable = _childToFlip.GetComponent<IEquipable>();
    }

    private void Update()
    {
        if (_equipable.NeedFlip)
        {
            var condition = !(transform.eulerAngles.z <= 90.0f || transform.eulerAngles.z >= 270.0f);
            if (condition != _wasFlipped)
            {
                transform.localScale = new Vector3(1, transform.localScale.y * -1.0f, 1);
                _wasFlipped = !_wasFlipped;
            }
        }
    }

    public void Rotate(float angle)
    {
        if (_equipable.NeedRotation)
        {
            transform.rotation = Quaternion.Euler(new Vector3(0, 0, angle));
        }
    }
}