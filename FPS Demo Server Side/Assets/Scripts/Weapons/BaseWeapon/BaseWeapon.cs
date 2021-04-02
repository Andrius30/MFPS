using UnityEngine;

public enum FireMode
{
    normal = 1,
    burst,
    auto
}

public class BaseWeapon : MonoBehaviour, IWeapon
{  
    public FireMode firemode;

    public int id;
    public string weaponName;

    [Header("Weapon damage seetings")]
    [SerializeField] float weaponDamage;
    [Space(10)]
    [Header("Weapon cooldown settings")]
    public float coolDown;

    protected IWeapon weaponType;

    public void DoDamage(IDamagable damagable)
    {
        damagable.TakeDamage(weaponDamage);
    }


}
