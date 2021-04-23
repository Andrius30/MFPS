using UnityEngine;

public interface IWeapon
{
    void DoDamage(IDamagable damagable, Transform attacker, AttackerTypes type);
}
