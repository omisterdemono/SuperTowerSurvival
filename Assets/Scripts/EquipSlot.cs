using Assets.Scripts.Weapons;
using UnityEngine;

public class EquipSlot : MonoBehaviour
{
    private Transform _childToFlip;
    private IEquipable _equipable;
    private bool _wasFlipped = false;

    private void Awake()
    {
        _childToFlip = gameObject.transform.GetChild(0);
        _equipable = GetComponentInChildren<IEquipable>();
    }

    public void Rotate(float angle)
    {
        if (_equipable.NeedRotation)
        {
            transform.rotation = Quaternion.Euler(new Vector3(0, 0, angle));
        }

        if (_equipable.NeedFlip)
        {
            var condition = !(transform.eulerAngles.z <= 90.0f || transform.eulerAngles.z >= 270.0f);
            if (condition != _wasFlipped)
            {
                _childToFlip.localScale = new Vector3(1, _childToFlip.localScale.y * -1.0f, 1);
                _wasFlipped = !_wasFlipped;
            }
        }
    }

}
