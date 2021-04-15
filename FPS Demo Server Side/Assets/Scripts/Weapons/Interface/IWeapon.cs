
using UnityEngine;

public interface IWeapon
{
    void Init();
    void DoDamage(IDamagable damagable, Transform attacker, AttackerTypes type);
}
