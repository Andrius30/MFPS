using System.Collections;
using UnityEngine;

public class Enemy : MonoBehaviour, IDamagable
{
    public float maxHealth = 100f;
    float health;

    void Start()
    {
        health = maxHealth;
    }

    public void TakeDamage(float dmg)
    {
        if (health > 0)
            health -= dmg;
        if (health <= 0)
            Die();
    }

    public void Die()
    {
        // TEST
        health = 0;
        Debug.Log("$Enemy died :20:red;".Interpolate());
        StartCoroutine(RespawnEnemy());
    }
    IEnumerator RespawnEnemy()
    {
        yield return new WaitForSeconds(5f);
        health = maxHealth;
    }
}
