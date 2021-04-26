
using UnityEngine;

public class MineItem : Item
{
    [SerializeField] AttackerTypes attackerType = AttackerTypes.Mine;

    [SerializeField] float explosionDamage;

    protected override void Execute(Collider other)
    {
        IDamagable damagable = other.GetComponent<IDamagable>();
        if (damagable != null)
        {
            damagable.TakeDamage(explosionDamage, transform, attackerType);
            ItemsSpawner.itemsDict.Remove(itemID);
            Destroy(gameObject);
        }
    }

}
