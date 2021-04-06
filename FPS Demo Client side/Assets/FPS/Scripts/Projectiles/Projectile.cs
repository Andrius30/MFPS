using System;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    public int id;
    [SerializeField] float selfDestructionTimer = 5f;

    void Awake()
    {
        GameManager.projectiles.Remove(id);
        Destroy(gameObject, selfDestructionTimer);
    }
}
