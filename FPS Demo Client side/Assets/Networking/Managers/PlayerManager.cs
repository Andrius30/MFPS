using FPSClient.Timers;
using System;
using System.Linq;
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
    public PlayerState playerState;
    public SurfaceTypes currentSurface;

    public int id;
    public string username;
    public bool isLocalPlayer;

    [Space(10)]
    [Header("Player health")]
    public float maxHealth = 100f;
    [Space(10)]

    [Header("Weapons")]
    public Transform weaponsparent;
    public Vector3 weaponParentPositionOnCrouching;
    public ClientWeapon newWeapon { get; protected set; }
    public int startingWeaponIndex = 0;

    public MeshRenderer model;
    public Animator characterAnimator;
    public PlayerAnimations playerAnimations { get; private set; }

    [Space(10)]
    [Header("Player foot steps audio settings")]
    [SerializeField] PlayerAudio playerAudio; // scriptable object

    [Space(10)]
    [Header("Surface checking settings")]
    public Transform surfaceCheckPosition;
    public float checkDistance = 1f;
    public float checkRatio = 0.3f;
    public bool isDebugEnabled = false;

    [Space(10)]
    [Header("Player Health UI")]
    public GameObject healthCanvas;
    [Header("Player Amunition UI")]
    [Space(10)]
    public GameObject ammunitionCanvas;

    Timer timer;

    [HideInInspector] public AudioSource playerSource;
    SurfaceChecker surfaceChecker;
    public PlayerHealth playerHealth { get; private set; }
    public PlayerHealth_UI playerHealth_UI { get; private set; }
    public PlayerAmunition_UI playerAmunition { get; private set; }

    internal void Initialize(int id, string userName, bool isLocal)
    {
        this.id = id;
        this.username = userName;
        isLocalPlayer = isLocal;
        transform.name = userName;
        // ===============================================================

        playerAudio.Init(this);
        playerSource = GetComponent<AudioSource>();
  
        PacketsToSend.InitializeWeaponsAndSetStartingWeapon(GetAllWeapons(), startingWeaponIndex); // send msg: set starting weapon
        SurfaceAnimationEvents.onFoostepPlay += playerAudio.Play;

        // ================================================================================
        playerAnimations = new PlayerAnimations(this);
        playerHealth = new PlayerHealth(this);
        playerHealth_UI = new PlayerHealth_UI(this);
        playerAmunition = new PlayerAmunition_UI(this);
        surfaceChecker = new SurfaceChecker(this);
        timer = new Timer(checkRatio, false);
    }

    void Update()
    {
        timer.StartTimer();
        if (timer.IsDone())
        {
            surfaceChecker.CheckSurface();
            timer.SetTimer(checkRatio, false);
        }
    }

    #region Damagable section
    public void GetAttackerAndDmg(int type, int id, float dmg)
    {
        Transform attacker = GetType(id, type);
        if (!DamageIndicators.isVisible.Invoke(attacker))
            DamageIndicators.onIndicatorCreated?.Invoke(attacker);
        BloodSplatter.onDamage?.Invoke(dmg);
    }


    public void Die()
    {
        model.enabled = false;
    }
    public void Respawn()
    {
        model.enabled = true;
        playerHealth.SetHealth(maxHealth);
        playerHealth_UI.ShowHealthAt_UI(playerHealth.GetHealth());
    }
    #endregion

    #region Weapons section
    public void ChangeWeapon(int id, string weaponName, int fireMode, int maxBullets, int bulletsLeft, float cd)
    {
        DisableAllWeapons();
        SetWeapon(id, weaponName, fireMode, maxBullets, bulletsLeft, cd);
        playerAmunition.SetText(maxBullets, bulletsLeft);
    }
    public int GetWeaponsLength() => GetAllWeapons().Length;

    void SetWeapon(int id, string name, int fireMode, int maxBullets, int bulletsLeft, float cd)
    {
        newWeapon = GetAllWeapons()[id];
        newWeapon.gameObject.SetActive(true);
        newWeapon.Initialize(id, name, fireMode, maxBullets, bulletsLeft, cd);
        newWeapon.weaponAnimations.DrawAnimations();
    }
    void DisableAllWeapons()
    {
        foreach (var weapon in GetAllWeapons())
        {
            weapon.gameObject.SetActive(false);
        }
    }
    ClientWeapon[] GetAllWeapons() => weaponsparent.GetComponentsInChildren<ClientWeapon>(true);
    Transform GetType(int attackerID, int attackerTypes)
    {
        switch (attackerTypes)
        {
            case (int)AttackerTypes.Player:
                return GameManager.players[attackerID].transform;
            case (int)AttackerTypes.Enemy:
                return GameManager.enemies[attackerID].transform;
        }
        return null;
    }
    #endregion

}
