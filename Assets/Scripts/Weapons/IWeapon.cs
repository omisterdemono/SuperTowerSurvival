using Inventory.Models;

public interface IWeapon
{
    float Damage { get; set; }   
    ItemSO Item { get; set; }   
    void Attack();
    void Hold();
    void KeyUp();
}
