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
    public Transform weaponsparent;
    public BaseWeapon newWeapon { get; protected set; }
    public int startingWeaponIndex = 0;
    
    public MeshRenderer model;
    public Animator characterAnimator;

    internal void Initialize(int id, string userName)
    {
        this.id = id;
        this.username = userName;
        health = maxHealth;

        PacketsToSend.SetStartingWeapon(GetAllWeapons()[startingWeaponIndex]); // send msg: set starting weapon
    }

    #region Damagable section
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
    #endregion

    #region Weapons section
    public void ChangeWeapon(int id, string weaponName, int fireMode)
    {
        DisableAllWeapons();
        SetWeapon(id, weaponName, fireMode);
    }
    void SetWeapon(int id, string name, int fireMode)
    {
        newWeapon = GetAllWeapons()[id];
        newWeapon.gameObject.SetActive(true);
        newWeapon.Initialize(id, name, fireMode);
    }
    void DisableAllWeapons()
    {
        foreach (var weapon in GetAllWeapons())
        {
            weapon.gameObject.SetActive(false);
        }
    }
    BaseWeapon[] GetAllWeapons() => weaponsparent.GetComponentsInChildren<BaseWeapon>(true); 
    #endregion

    public void PlayMoveAnimation(float x, float z)
    {
        characterAnimator.SetFloat("horizontal", x);
        characterAnimator.SetFloat("vertical", z);
    }
    public void PlayAimingAnimation(float angle,Quaternion localRot)
    {
        weaponsparent.localRotation = localRot;
        characterAnimator.SetFloat("aimAngle", angle);
    }
}
