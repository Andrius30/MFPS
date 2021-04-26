using System.Collections.Generic;
using UnityEngine;

public class MineItem : Item
{
    [SerializeField] float explosionForce;
    [SerializeField] float explosionRadius;
    [SerializeField] float upwardsModifier = 3.0f;
    [SerializeField] float destroyTime = 1.5f;
    [SerializeField] List<MeshRenderer> model;

    protected override void Execute(Collider other)
    {
        Vector3 explosionPos = transform.position;
        Collider[] colliders = Physics.OverlapSphere(explosionPos, explosionRadius);
        foreach (Collider hit in colliders)
        {
            Rigidbody rb = hit.GetComponent<Rigidbody>();

            if (rb != null)
                rb.AddExplosionForce(explosionForce, hit.ClosestPointOnBounds(transform.position), explosionRadius, upwardsModifier);
        }
        model.ForEach(x => x.enabled = false);
        GameManager.items.Remove(itemID);
        Destroy(gameObject, destroyTime);
    }

    //void OnDrawGizmos()
    //{
    //    Gizmos.color = Color.yellow;
    //    Gizmos.DrawWireSphere(transform.position, explosionRadius);
    //}
}
