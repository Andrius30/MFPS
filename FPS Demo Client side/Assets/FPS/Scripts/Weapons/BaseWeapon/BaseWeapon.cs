using FPSClient.Timers;
using UnityEngine;
using UnityEngine.Animations.Rigging;

public enum FireMode
{
    normal = 1,
    burst,
    auto
}

public class BaseWeapon : MonoBehaviour
{
    public int weaponID;
    public string weaponName;

    [Header("Weapon sounds")]
    public AudioClip shootingSound;

    [Space(10)]
    [Header("Weapon muzle flash prefab")]
    [SerializeField] GameObject muzleFlash;
    [SerializeField] Light muzleLight;

    [Space(10)]
    [Header("Unity animation riging. RIG")]
    [SerializeField] Rig rig;

    int fireMode;
    Timer timer;

    AudioSource source;

    public void Initialize(int id, string weaponName, int fireMode)
    {
        weaponID = id;
        this.weaponName = weaponName;
        this.fireMode = fireMode;

        timer = new Timer(0, false);
        source = GetComponent<AudioSource>();
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
    public int GetFireMode() => fireMode;
    public virtual void PlayShootingSound() => source.PlayOneShot(shootingSound);
    public virtual void PlayMuzleFlash()
    {
        foreach (ParticleSystem muzle in muzleFlash.GetComponentsInChildren<ParticleSystem>())
        {
            if (muzle != null)
                muzle.Play();
        }
        if (muzleLight != null)
        {
            if (!muzleLight.enabled)
            {
                muzleLight.enabled = true;
                timer.SetTimer(0.2f, false);
            }
        }
    }

    void OnEnable() => rig.weight = 1;
    void OnDisable() => rig.weight = 0;
}
