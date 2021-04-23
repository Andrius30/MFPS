using MFPS.ServerCharacters;
using System.Collections;
using UnityEngine;

public class Enemy : MonoBehaviour, IDamagable
{
    public AttackerTypes attackerType = AttackerTypes.Enemy;

    public int id;
    public float maxHealth = 100f;
    float health;

    [Space(10)]
    [Header("Attack settings")]
    public float dmg = 10f;

    public MeshRenderer model;

    void Start()
    {
        health = maxHealth;
    }

    void Shoot()
    {
        if (Physics.Raycast(transform.position, transform.forward, out RaycastHit hit, 25f))
        {
            if (hit.transform != null)
            {
                IDamagable damagable = hit.transform.GetComponent<IDamagable>();
                if (damagable != null)
                {
                    damagable.TakeDamage(dmg, this.transform, attackerType);
                }
            }
        }
    }

    //// =========== DEBUGING ==========================
    //void Update()
    //{
    //    if (Input.GetKeyDown(KeyCode.Mouse0))
    //    {
    //        Player pl = FindObjectOfType<Player>();
    //        IDamagable damagable = pl.GetComponent<IDamagable>();
    //        if (damagable != null)
    //        {
    //            damagable.TakeDamage(dmg, this.transform, attackerType);
    //        }
    //    }
    //}
    //// =========== DEBUGING ==========================

    public void TakeDamage(float dmg, Transform attacker, AttackerTypes type)
    {
       // Debug.Log($"Enemy taking damage {dmg} from {attacker.name} attacker type {type}");
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
