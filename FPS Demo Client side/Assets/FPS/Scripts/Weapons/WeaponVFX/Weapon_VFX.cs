using UnityEngine;

public class Weapon_VFX 
{
    ClientWeapon baseWeapon;

    public Weapon_VFX(ClientWeapon baseWeapon) => this.baseWeapon = baseWeapon;

    public virtual void PlayMuzleFlash()
    {
        foreach (ParticleSystem muzle in baseWeapon.muzleFlash.GetComponentsInChildren<ParticleSystem>())
        {
            if (muzle != null)
                muzle.Play();
        }
        if (baseWeapon.muzleLight != null)
        {
            if (!baseWeapon.muzleLight.enabled)
            {
                baseWeapon.muzleLight.enabled = true;
                baseWeapon.timer.SetTimer(0.2f, false);
            }
        }
    }

}
