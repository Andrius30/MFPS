using System;
using UnityEngine;

public class ProjectileSpawner
{
    public static Action<int, Vector3> onProjectileSpawn;
    PlayerManager player;

    public ProjectileSpawner(PlayerManager player)
    {
        this.player = player;
        onProjectileSpawn += SpawnProjectile;
    }

    void SpawnProjectile(int id, Vector3 position)
    {
        GameObject gm = MonoBehaviour.Instantiate(player.projectilePrefab, position, Quaternion.identity);
        Projectile projectile = gm.GetComponent<Projectile>();
        projectile.Initialize(id);
        GameManager.projectiles.Add(id, projectile);
    }
}
