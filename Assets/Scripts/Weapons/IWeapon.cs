public interface IWeapon
{
    float Damage { get; set; }   
    void Attack();
    void Hold();
    void KeyUp();
}
