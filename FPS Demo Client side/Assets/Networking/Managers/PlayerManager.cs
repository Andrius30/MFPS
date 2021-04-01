using System;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    public int id;
    public string username;
    public static bool isDied; // cia bugas, nes ant visu playeriu sitas scriptas gyvena

    [Space(10)]
    [Header("Player health")]
    public float maxHealth = 100f;
    public float health;
    [Space(10)]
    [Header("Items settings")]
    [SerializeField] int itemsHave;
    [SerializeField] int maxItems = 3;
    
    [Space(10)]
    [Header("Projectiles settings")]
    public ProjectileSpawner projectileSpawner;
    public GameObject projectilePrefab;

    public MeshRenderer model;

    internal void Initialize(int id, string userName)
    {
        this.id = id;
        this.username = userName;
        health = maxHealth;

        projectileSpawner = new ProjectileSpawner(this);
    }
    public void SetHealth(float health)
    {
        this.health = health;

        if (this.health <= 0)
        {
            Die();
        }
    }
    void Die()
    {
        model.enabled = false;
        isDied = true;
    }
    public void Respawn()
    {
        model.enabled = true;
        SetHealth(maxHealth);
        isDied = false;
    }
    public void SetItemsPicked()
    {
        if (itemsHave < maxItems)
            itemsHave++;
    }
    public void DecreaseItemsCount()
    {
        if (itemsHave > 0)
            itemsHave--;
    }
}
