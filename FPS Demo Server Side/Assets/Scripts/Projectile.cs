using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Projectile : MonoBehaviour
{
    public static int id;

    [SerializeField] float selfDestructionTimer = 5f;
    [HideInInspector] public int nextProjectile;

    void Awake()
    {
        id++;
        nextProjectile = id;
        Destroy(gameObject, selfDestructionTimer);
    }

    void FixedUpdate() => PacketsToSend.ProjectilePosition(this);

    void OnTriggerEnter(Collider other) => Destroy(gameObject);
}
