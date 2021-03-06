using FPSClient.Timers;
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
    public PlayerState playerState;
    public SurfaceTypes currentSurface;

    public int id;
    public string username;
    public bool isLocalPlayer;

    public List<GameObject> playerDeathModelsWithAnimations = new List<GameObject>();
    [SerializeField] GameObject deathCamera;
    [SerializeField] Vector3 deathCameraOffset;
    [SerializeField] Vector3 deathModelOffset;
    [Space(10)]
    [Header("Player health")]
    public float maxHealth = 100f;
    [Space(10)]

    [Header("Weapons")]
    public Transform weaponsparent;
    public Vector3 weaponParentPositionOnCrouching;
    public ClientWeapon newWeapon { get; protected set; }
    public int startingWeaponIndex = 0;

    public GameObject model;
    public Animator characterAnimator;
    public PlayerAnimations playerAnimations { get; private set; }

    [Space(10)]
    [Header("Player foot steps audio settings")]
    public PlayerAudio playerAudio;  // scriptable object

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

    [Space(10)]
    [Header("Effects")]
    public HitSurface_VFX hitSurface_VFX; // scriptable object

    Timer timer;
    [HideInInspector] public AudioSource playerSource;
    SurfaceChecker surfaceChecker;
    public PlayerHealth playerHealth { get; private set; }
    public PlayerHealth_UI playerHealth_UI { get; private set; }
    public PlayerAmunition_UI playerAmunition { get; private set; }

    public void Initialize(int id, string userName, bool isLocal)
    {
        this.id = id;
        this.username = userName;
        isLocalPlayer = isLocal;
        transform.name = userName;
        // ===============================================================

        playerAudio.Init(this);
        playerSource = GetComponent<AudioSource>();

        if (isLocalPlayer)
            PacketsToSend.InitializeWeaponsAndSetStartingWeapon(GetAllWeapons(), startingWeaponIndex); // send msg: set starting weapon

        SurfaceAnimationEvents.onFoostepPlay += playerAudio.Play;
        SurfaceAnimationEvents.onJumpPlay += playerAudio.PlayJumpAudio;
        SurfaceAnimationEvents.onLandPlay += playerAudio.PlayLandAudio;

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
        model.SetActive(false);
        CreateDeathModel();
    }
    public void Respawn()
    {
        model.SetActive(true);
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

    void CreateDeathModel()
    {
        GameObject gm = Instantiate(GetRandomDeathModel(), transform.position + deathModelOffset, transform.rotation);
        if (isLocalPlayer) // create death cam if its local player
        {
            Transform deathCampos = UtilityMethods.onChildRequested?.Invoke(gm.transform, "swat:Head");
            GameObject deathCam = Instantiate(deathCamera, deathCampos.position + deathCameraOffset, deathCampos.rotation);
            deathCam.transform.SetParent(deathCampos);
        }
        Destroy(gm, 5f);
    }
    GameObject GetRandomDeathModel() => playerDeathModelsWithAnimations[Random.Range(0, playerDeathModelsWithAnimations.Count)];
}
