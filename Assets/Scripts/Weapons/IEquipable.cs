using UnityEngine;

namespace Assets.Scripts.Weapons
{
    public interface IEquipable
    {
        bool NeedFlip { get; set; }
        bool NeedRotation { get; set; }
        void Interact();
        void Hold();
        void FinishHold();
        void ChangeAnimationState();
        bool CanPerform { get; }
        float CooldownSeconds { get; }
        Vector3 MousePosition { get; set; }
    }
}
