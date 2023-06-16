using Assets.Scripts.Weapons;
using UnityEngine;

public class EquipedSlot : MonoBehaviour
{
    private SpriteRenderer _itemSpriteRenderer;
    private IEquipable _equipable;

    private void Awake()
    {
        _itemSpriteRenderer = gameObject.transform.GetChild(0).GetComponent<SpriteRenderer>();
        _equipable = GetComponentInChildren<IEquipable>();
    }

    public void Rotate(float angle)
    {
        transform.rotation = Quaternion.Euler(new Vector3(0, 0, angle));

        if(_equipable.NeedFlip) 
        {
            _itemSpriteRenderer.flipY = !(transform.eulerAngles.z <= 90.0f || transform.eulerAngles.z >= 270.0f);
        }
    }

}
