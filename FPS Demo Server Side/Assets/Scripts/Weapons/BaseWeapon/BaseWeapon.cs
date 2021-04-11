using MFPS.ServerTimers;
using UnityEngine;

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
namespace MFPS.Weapons
{
    public class BaseWeapon : MonoBehaviour, IWeapon
    {
        public FireMode firemode;
        [SerializeField] WeaponState weaponState;

        public int id;
        public string weaponName;
        [Space(10)]
        [Header("Bullets Setting")]
        public GameObject projectile;
        [SerializeField] Transform projectileSpawnPosition;
        [SerializeField] float bulletForce = 100f;
        [SerializeField] int totalBullets = 100;
        [SerializeField] int magazineCapacity = 20;
        int shootedbullets;

        [Header("Weapon damage seetings")]
        [SerializeField] float weaponDamage;
        [Space(10)]
        [Header("Weapon cooldown settings")]
        public float coolDown;
        public float weaponDrawTime = 2f;
        public float reloadTime = 1f;

        public IWeapon weaponType;
        int count = 0;
        Timer drawWeaponTimer;
        Timer reloadTimer;

        public virtual void Init()
        {
            if (drawWeaponTimer == null)
                drawWeaponTimer = new Timer(weaponDrawTime, false);
            else
                drawWeaponTimer.SetTimer(weaponDrawTime, false);

            if (reloadTimer == null)
                reloadTimer = new Timer(reloadTime, false);
            else
                reloadTimer.SetTimer(reloadTime, false);
        }
        public virtual void DoDamage(IDamagable damagable)
        {
            damagable.TakeDamage(weaponDamage);
        }
        public void SpawnProjectile(Vector3 direction)
        {
            if (IsMagazineEmpty())
            {
                reloadTimer.SetTimer(reloadTime, false);
                SetWeaponState(WeaponState.Reloading);
                return;
            }
            if (firemode == FireMode.auto)
            {
                count++;
                if (count >= 3)
                {
                    CreateBullet(direction);
                    count = 0;
                }
            }
            else
            {
                CreateBullet(direction);
            }
            shootedbullets++;
        }

        public bool IsoutOfAmmo() => (GetCurrentBulletsAtMagazine() <= 0 && totalBullets - shootedbullets <= 0);
        public void SetWeaponState(WeaponState state) => weaponState = state;
        public WeaponState GetWeaponState() => weaponState;

        void Update()
        {
            if (GetWeaponState() == WeaponState.DrawWeapon)
            {
                drawWeaponTimer.StartTimer();
                if (drawWeaponTimer.IsDone())
                    SetWeaponState(WeaponState.Idle);
            }
            if (GetWeaponState() == WeaponState.Reloading)
            {
                reloadTimer.StartTimer();
                if (reloadTimer.IsDone())
                {
                    ReloadWeapon();
                }
            }
        }
        void CreateBullet(Vector3 direction)
        {
            GameObject gm = Instantiate(projectile, projectileSpawnPosition.position, projectileSpawnPosition.rotation);
            Projectile proj = gm.GetComponent<Projectile>();
            proj.GetComponent<Rigidbody>().AddForce(projectileSpawnPosition.position + direction * bulletForce, ForceMode.Impulse);
            PacketsToSend.SpawnProjectile(proj);
        }
        void ReloadWeapon()
        {
            if (totalBullets - shootedbullets >= 0)
            {
                totalBullets -= shootedbullets;
                shootedbullets = 0;
                SetWeaponState(WeaponState.Idle);
            }
        }
        int GetCurrentBulletsAtMagazine() => magazineCapacity - shootedbullets;
        bool IsMagazineEmpty() => magazineCapacity - shootedbullets <= 0;
    }
}