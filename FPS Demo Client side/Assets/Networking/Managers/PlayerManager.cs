using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    public int id;
    public string username;

    [Space(10)]
    [Header("Player health")]
    public float maxHealth = 100f;
    public float health;
    [Space(10)]

    [Header("Weapons")]
    public List<BaseWeapon> allWeapons;
    public Transform weaponPosition;

    public MeshRenderer model;

    [HideInInspector] public BaseWeapon newWeapon;
    BaseWeapon oldWeapon;

    internal void Initialize(int id, string userName)
    {
        this.id = id;
        this.username = userName;
        health = maxHealth;
    }
    public void SetHealth(float health)
    {
        this.health = health;
        if (this.health <= 0)
            Die();
    }
    void Die()
    {
        model.enabled = false;
        PlayerController.onLocalPlayerLiveStateChanged?.Invoke(true);
    }
    public void Respawn()
    {
        model.enabled = true;
        SetHealth(maxHealth);
        PlayerController.onLocalPlayerLiveStateChanged?.Invoke(false);
    }

    public void ChangeWeapon(int id, string weaponName, int fireMode)
    {
        if (oldWeapon != null)
        {
            Destroy(oldWeapon.gameObject);
            oldWeapon = null;
        }
        GameObject gm = Instantiate(allWeapons[id].gameObject, weaponPosition);
        newWeapon = gm.GetComponent<BaseWeapon>();
        newWeapon.Initialize(id, weaponName, fireMode);
        oldWeapon = newWeapon;
    }
}
