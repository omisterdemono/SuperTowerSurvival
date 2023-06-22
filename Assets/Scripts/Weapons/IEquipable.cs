namespace Assets.Scripts.Weapons
{
    public interface IEquipable
    {
        bool NeedFlip { get; set; }
        bool NeedRotation { get; set; }
    }
}
