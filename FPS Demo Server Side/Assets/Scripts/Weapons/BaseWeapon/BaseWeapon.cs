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

        [Header("Other Weapon settings and options")]
        public int id;
        public string weaponName;
        public float weaponRange = 25f;
        [SerializeField] Transform weaponModel;

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

        /// <summary>
        /// Getting all required weapon position from client when new client connect
        /// </summary>
        /// <param name="modelPosition"></param>
        /// <param name="modelRotation"></param>
        /// <param name="shootPos"></param>
        /// <param name="shootRot"></param>
        public virtual void InitializeWeapons(Vector3 modelPosition, Quaternion modelRotation, Vector3 shootPos, Quaternion shootRot)
        {
            weaponModel.localPosition = modelPosition;
            weaponModel.localRotation = modelRotation;
            projectileSpawnPosition.localPosition = shootPos;
            projectileSpawnPosition.localRotation = shootRot;
            //Debug.Log($"Weapon ID {id} initialized with position { modelPosition} and rotation {modelRotation} :green:18;".Interpolate());
        }
       
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
        public virtual void DoDamage(IDamagable damagable, Transform attacker)
        {
            damagable.TakeDamage(weaponDamage, attacker);
        }
        public void SpawnProjectile()
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
                    CreateBullet(projectileSpawnPosition);
                    count = 0;
                }
            }
            else
            {
                CreateBullet(projectileSpawnPosition);
            }
            shootedbullets++;
        }
        public virtual bool IsoutOfAmmo() => (GetCurrentBulletsAtMagazine() <= 0 && totalBullets - shootedbullets <= 0);
        public virtual void SetWeaponState(WeaponState state) => weaponState = state;
        public WeaponState GetWeaponState() => weaponState;

        void CreateBullet(Transform spawntr)
        {
            GameObject gm = Instantiate(projectile, spawntr.position, spawntr.rotation);
            Projectile proj = gm.GetComponent<Projectile>();
            proj.GetComponent<Rigidbody>().AddForce(spawntr.forward * bulletForce, ForceMode.Impulse);
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