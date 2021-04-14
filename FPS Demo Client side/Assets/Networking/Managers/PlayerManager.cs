using FPSClient.Timers;
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
    public SurfaceTypes currentSurface;

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
    public ClientWeapon newWeapon { get; protected set; }
    public int startingWeaponIndex = 0;

    public MeshRenderer model;
    public Animator characterAnimator;
    public PlayerAnimations playerAnimations { get; private set; }

    [Space(10)]
    [Header("Player foot steps audio settings")]
    [SerializeField] PlayerAudio playerAudio;

    [Space(10)]
    [Header("Surface checking settings")]
    public Transform surfaceCheckPosition;
    public float checkDistance = 1f;
    public float checkRatio = 0.3f;
    public bool isDebugEnabled = false;

    Timer timer;

    [HideInInspector] public AudioSource playerSource;
    SurfaceChecker surfaceChecker;

    internal void Initialize(int id, string userName)
    {
        this.id = id;
        this.username = userName;
        health = maxHealth;
        playerAnimations = new PlayerAnimations(this);

        playerAudio.Init(this);
        playerSource = GetComponent<AudioSource>();
        PacketsToSend.InitializeWeaponsAndSetStartingWeapon(GetAllWeapons(), startingWeaponIndex); // send msg: set starting weapon
        SurfaceAnimationEvents.onFoostepPlay += playerAudio.Play;
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
    #endregion

}
