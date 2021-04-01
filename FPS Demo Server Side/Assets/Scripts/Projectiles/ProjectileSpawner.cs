using UnityEngine;

public class ProjectileSpawner
{
    Player player;
    Rigidbody rb;

    public ProjectileSpawner(Player player) => this.player = player;

    public void SpawnProjectile(Vector3 direction)
    {
        GameObject gm = MonoBehaviour.Instantiate(player.projectilePrefab, player.launchPosition.position, player.launchPosition.rotation);
        Projectile projectile = gm.GetComponent<Projectile>();
        projectile.Initialize(player.id);
        rb = gm.GetComponent<Rigidbody>();
        rb.AddForce(direction * player.throwForce, ForceMode.Impulse);
    }

}
