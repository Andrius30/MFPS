
using UnityEngine;

public interface IDamagable
{

    void TakeDamage(float dmg, Transform attacker = null);
    void Die();
}
