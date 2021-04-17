using FPSClient.Timers;
using UnityEngine;
using UnityEngine.Animations.Rigging;

public enum FireMode
{
    normal = 1,
    burst,
    auto
}
public enum WeaponState
{
    Idle,
    Shooting,
    Reloading,
    OutOfAmmo,
    DrawWeapon
}
public class ClientWeapon : MonoBehaviour
{
    public WeaponState weaponState;
    #region Vars
    public int weaponID;
    public string weaponName;

    [Space(10)]
    [Header("Weapon other settings")]
    public int maxBullets;
    public int bulletsLeft;
    public float coolDown;

    [Space(10)]
    [Header("Weapon sounds")]
    public AudioClip shootingSound;
    public AudioClip reloadingSound;
    public AudioClip drawWeaponSound;
    public AudioClip outOfAmmoSound;

    [Space(10)]
    [Header("Weapon Animator")]
    public Animator weaponAnimator;

    [Space(10)]
    [Header("Weapon muzle flash prefab")]
    public GameObject muzleFlash;
    public Light muzleLight;

    [Space(10)]
    [Header("Unity animation riging. RIG")]
    [SerializeField] Rig rig;

    [Space(10)]
    [Header("Weapon shoot position")]
    public Transform shootPosition;
    public Transform model;

    int fireMode;
    Weapons_SFX weapons_SFX;
    Weapon_VFX weapon_VFX;
    public Timer timer { get; set; }
    public WeaponAnimations weaponAnimations { get; set; }
    #endregion

    void Awake()
    {
        timer = new Timer(0, false);
        weapons_SFX = new Weapons_SFX(this);
        weapon_VFX = new Weapon_VFX(this);
        weaponAnimations = new WeaponAnimations(this);
    }
    void Update()
    {
        timer.StartTimer();
        if (timer.IsDone())
        {
            if (muzleLight.enabled)
                muzleLight.enabled = false;
        }

    }

    public void Initialize(int id, string weaponName, int fireMode, int maxBullets, int bulletsLeft,float cd)
    {
        weaponID = id;
        this.weaponName = weaponName;
        this.fireMode = fireMode;
        this.maxBullets = maxBullets;
        this.bulletsLeft = bulletsLeft;
        this.coolDown = cd;
    }
    public int GetFireMode() => fireMode;
    public void ActionsByWeaponState(int state)
    {
        switch (state)
        {
            case (int)WeaponState.Idle:
                weaponState = WeaponState.Idle;
                ResetSomeStates();
                break;
            case (int)WeaponState.Shooting:
                weaponState = WeaponState.Shooting;
                weapons_SFX.PlayShootAudio();
                weapon_VFX.PlayMuzleFlash();
                weaponAnimations.PlayShootingAnimation();
                break;
            case (int)WeaponState.Reloading:
                weaponState = WeaponState.Reloading;
                weapons_SFX.PlayAudio(reloadingSound);
                weaponAnimations.PlayReloadingAnimation();
                break;
            case (int)WeaponState.OutOfAmmo:
                weaponState = WeaponState.OutOfAmmo;
                weapons_SFX.PlayAudio(outOfAmmoSound);
                break;
            case (int)WeaponState.DrawWeapon:
                weaponState = WeaponState.DrawWeapon;
                weapons_SFX.PlayAudio(drawWeaponSound);
                break;
        }
    }
    public void UpdateBullets(int maxBs, int bullets)
    {
        maxBullets = maxBs;
        bulletsLeft = bullets;
    }
    void ResetSomeStates()
    {
        weaponAnimator.SetBool("isReloading", false);
    }
    void SetRig(int value) => rig.weight = value;

    void OnEnable() => SetRig(1);
    void OnDisable() => SetRig(0);
}
