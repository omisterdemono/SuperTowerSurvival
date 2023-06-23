using System;
using UnityEngine;

public class RotationSpritesLoader : MonoBehaviour
{
    [SerializeField] private Sprite[] _rotationSprites;
    [SerializeField] private int[] _defenceItemsIndexes;
    void Start()
    {
        //Sprite[] sprites = Resources.LoadAll<Sprite>("Sprites/SpriteAnimations/DefenseGun/DefenseGun");
    }

    void Update()
    {
        
    }
}
