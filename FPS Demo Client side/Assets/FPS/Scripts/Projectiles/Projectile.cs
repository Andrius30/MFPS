using UnityEngine;

public class Projectile : MonoBehaviour
{
    public int id { get; set; }
    public GameObject explosionPrefab;

    void Start() => Destroy(gameObject, 3f);

    public void Initialize(int id) => this.id = id;

    public void Explode(Vector3 pos)
    {
        Instantiate(explosionPrefab, pos, Quaternion.identity);
        GameManager.projectiles.Remove(id);
        Destroy(gameObject);
    }
}
