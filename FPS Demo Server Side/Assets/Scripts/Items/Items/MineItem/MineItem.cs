
using MFPS.ServerCharacters;
using UnityEditor;
using UnityEngine;

public class MineItem : Item
{
    [SerializeField] AttackerTypes attackerType = AttackerTypes.Mine;

    [SerializeField] float explosionDamage;
    [SerializeField] float explosionRadius;

    protected override void Execute(Player player)
    {
        Vector3 explosionPos = transform.position;
        Collider[] colliders = Physics.OverlapSphere(explosionPos, explosionRadius);
        foreach (Collider hit in colliders)
        {
            Player damagable = hit.GetComponent<Player>();
            if (damagable == null) continue;
            
            float damage = CalculateDamageByDistanceFromExplosionPoint(hit);
            damagable.TakeDamage(damage, transform, attackerType);
            ItemsSpawner.itemsDict.Remove(itemID);
            Destroy(gameObject);
        }
    }

    float CalculateDamageByDistanceFromExplosionPoint(Collider hit)
    {
        Vector3 closestPoint = hit.ClosestPointOnBounds(transform.position);
        float distance = Vector3.Distance(closestPoint, transform.position);
        float dmgRatio = 1f - Mathf.Clamp01(distance / explosionRadius);
        return dmgRatio *= explosionDamage;
    }

    private void Update() // Test damage ratio
    {
        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            Vector3 explosionPos = transform.position;
            Collider[] colliders = Physics.OverlapSphere(explosionPos, explosionRadius);
            foreach (Collider hit in colliders)
            {
                Player damagable = hit.GetComponent<Player>();
                if (damagable == null) continue;

                float damage = CalculateDamageByDistanceFromExplosionPoint(hit);
                damagable.TakeDamage(damage, transform, attackerType);
                PacketsToSend.ExecuteItem(this, hit.transform.GetComponent<Player>());
                ItemsSpawner.itemsDict.Remove(itemID);
                Destroy(gameObject);
            }
        }
    }

#if UNITY_EDITOR
    Vector3 offset;
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, explosionRadius);

        Handles.color = Color.cyan;
        offset = new Vector3(0, explosionRadius + 1, 0);
        GUIStyle style = new GUIStyle();
        style.normal.textColor = Color.cyan;
        style.fontSize = 20;
        Handles.Label(transform.position + offset, "Explosion Radius", style);
        Handles.DrawLine(transform.position, transform.position + offset);
    }
#endif
}

