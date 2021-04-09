using System.Collections.Generic;
using UnityEngine;

public enum PlayerState
{
    Idle,
    Running,
    Walking,
    Crouching
}

public class PlayerManager : MonoBehaviour
{
    PlayerState playerState;

    public int id;
    public string username;

    [Space(10)]
    [Header("Player health")]
    public float maxHealth = 100f;
    public float health;
    [Space(10)]

    [Header("Weapons")]
    public Transform weaponsparent;
    public Vector3 weaponParentPositionOnCrouching;
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
    }
    public void Respawn()
    {
        model.enabled = true;
        SetHealth(maxHealth);
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
    public void PlayCrouchAnimation(bool isCrouching)
    {
        if (isCrouching)
        {
            CrouchAnimation(true, weaponParentPositionOnCrouching);
        }
        else
        {
            CrouchAnimation(false, Vector3.zero);
        }
    }
    public void PlayWalkAnimation(bool isWalking)
    {
        if (isWalking)
            characterAnimator.SetBool("isWalking", isWalking);
        else
            characterAnimator.SetBool("isWalking", isWalking);
    }
    public void PlayAimingAnimation(float angle, Quaternion localRot)
    {
        weaponsparent.localRotation = localRot;
        characterAnimator.SetFloat("aimAngle", angle);
    }
    void CrouchAnimation(bool isPlaying, Vector3 pos)
    {
        characterAnimator.SetBool("isCrouching", isPlaying);
        weaponsparent.localPosition = pos;
    }

    /// <summary>
    /// Play network animations
    /// </summary>
    /// <param name="state"></param>
    public void PlayAnimationsDependingOnPlayerState(int state)
    {
        switch (state)
        {
            case (int)PlayerState.Crouching:
                PlayCrouchAnimation(true);
                PlayWalkAnimation(false);
                break;
            case (int)PlayerState.Walking:
                PlayWalkAnimation(true);
                PlayCrouchAnimation(false);
                break;
            case (int)PlayerState.Running:
                PlayCrouchAnimation(false);
                PlayWalkAnimation(false);
                break;
        }
    }
}
