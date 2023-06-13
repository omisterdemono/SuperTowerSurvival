public interface IAttacker
{
    float Damage { get; set; }   
    void Attack();
    void Rotate(float angle);
}
