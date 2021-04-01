using System;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    [SerializeField] float projectileDamage = 20f;
    public static int id;
    int thrownByPlayer;

    void Start()
    {
        id++;
        if (id >= 9999999)
            id = 0;
        PacketsToSend.SpawnProjectile(id, transform.position, thrownByPlayer);
    }

    void FixedUpdate()
    {
        PacketsToSend.ProjectilePosition(id, transform.position);
    }

    void OnTriggerEnter(Collider other)
    {
        if (other)
        {
            IDamagable damagable = other.GetComponent<IDamagable>();
            if (damagable != null)
            {
                damagable.TakeDamage(projectileDamage);
            }
            Explode();
            DestryProjectile();
        }
    }

    public void Initialize(int id) => thrownByPlayer = id;

    void DestryProjectile() => Destroy(gameObject);
    void Explode() => PacketsToSend.Explode(id, transform.position);
}
