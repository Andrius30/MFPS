using System;
using UnityEngine;
using UnityEngine.UI;

public class Enemy : MonoBehaviour
{
    public Action<float> onEnemyHealthChanged;
    public Action<float> onEnemyRespawned;

    public float maxhealth;
    float health;

    public MeshRenderer model;
    public Image hpBar;

    void OnEnable()
    {
        onEnemyHealthChanged += SetHealth;
        onEnemyRespawned += EnemyRespawned;
    }

    void Die()
    {
        if (health <= 0)
        {
            model.enabled = false;
        }
    }

    public void SetHealth(float hp)
    {
        //Debug.Log($"Enemy taking {health - hp } damage");
        health = hp; // just for testing
        hpBar.fillAmount = health / maxhealth;
        Die();
    }

    void EnemyRespawned(float health)
    {
        this.health = health;
        model.enabled = true;
        hpBar.fillAmount = health / maxhealth;
    }
}
