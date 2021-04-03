using System;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    public int id;
    [SerializeField] float selfDestructionTimer = 5f;

    void Awake() => Destroy(gameObject, selfDestructionTimer);
}
