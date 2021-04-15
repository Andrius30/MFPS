using UnityEngine;

public interface IDamagable
{
    void TakeDamage(float dmg, Transform attacker, AttackerTypes type);
    void Die();
}
