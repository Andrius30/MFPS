using System;
using System.Collections;
using UnityEngine;

public class Enemy : MonoBehaviour, IDamagable
{
    //public int id;
    public float maxHealth = 100f;
    float health;

    public MeshRenderer model;

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

        PacketsToSend.EnemyHealth(this);
    }

    public void Die()
    {
        // TEST
        health = 0;
        model.enabled = false;
        StartCoroutine(RespawnEnemy());
    }
    IEnumerator RespawnEnemy()
    {
        yield return new WaitForSeconds(5f);
        model.enabled = true;
        health = maxHealth;

        PacketsToSend.EnemyRespawned(this);
    }

    public float GetHealth() => health;
}
