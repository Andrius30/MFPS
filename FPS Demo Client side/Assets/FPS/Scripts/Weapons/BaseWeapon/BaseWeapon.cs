using UnityEngine;

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
    
    int fireMode;

    AudioSource source;

    internal void Initialize(int id, string weaponName, int fireMode)
    {
        weaponID = id;
        this.weaponName = weaponName;
        this.fireMode = fireMode;

        source = GetComponent<AudioSource>();
    }
    public int GetFireMode() => fireMode;
    public void PlayShootingSound() => source.PlayOneShot(shootingSound);
}
