using MFPS.ServerCharacters;
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
    [RequireComponent(typeof(Acuracy))]
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
        public Transform projectileSpawnPoint;
        [SerializeField] float bulletForce = 100f;
        [SerializeField] int totalBullets = 100;
        [SerializeField] int magazineCapacity = 20;
        public int GetMaxBullets => totalBullets;
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
        public Acuracy acuracy { get; private set; }
        Player player;
        Timer timer;
        Timer drawWeaponTimer;
        Timer reloadTimer;

        protected virtual void Start()
        {
            timer = new Timer(0, false);
            drawWeaponTimer = new Timer(0, true);
            reloadTimer = new Timer(0, true);
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
            acuracy = GetComponent<Acuracy>();
            weaponModel.localPosition = modelPosition;
            weaponModel.localRotation = modelRotation;
            this.projectileSpawnPoint.localPosition = shootPos;
            this.projectileSpawnPoint.localRotation = shootRot;
        }
        public virtual void DoDamage(IDamagable damagable, Transform attacker, AttackerTypes type)
        {
            damagable.TakeDamage(weaponDamage, attacker, type);
        }
        public virtual void Attack()
        {
            if (weaponState == WeaponState.Reloading || weaponState == WeaponState.DrawWeapon)
                return;

            if (timer != null && timer.IsDone())
            {
                if (weaponState == WeaponState.OutOfAmmo)
                {
                    PacketsToSend.WeaponState(player);
                    return;
                }

                acuracy.UpdateWeaponInacuracy(player, true); // inaccuracy
                SetWeaponState(WeaponState.Shooting);

                if (IsMagazineEmpty())
                {
                    if (IsoutOfAmmo())
                    {
                        SetWeaponState(WeaponState.OutOfAmmo);
                        return;
                    }
                    reloadTimer.SetTimer(reloadTime, false);
                    SetWeaponState(WeaponState.Reloading);
                    return;
                }
                ShootRaycast();
                shootedbullets++;
                timer.SetTimer(coolDown, false);
            }
        }


        public virtual void SpawnProjectile(Vector3 direction)
        {
            if (firemode == FireMode.auto)
            {
                count++;
                if (count >= 3)
                {
                    CreateBullet(projectileSpawnPoint, direction);
                    count = 0;
                }
            }
            else
            {
                CreateBullet(projectileSpawnPoint, direction);
            }
        }
        public virtual bool IsoutOfAmmo() => totalBullets - shootedbullets < 0;
        public virtual void SetWeaponState(WeaponState state) => weaponState = state;
        public virtual void ReloadWeapon(Player player)
        {
            if (totalBullets - shootedbullets >= 0)
            {
                totalBullets -= shootedbullets;
                shootedbullets = 0;
                SetWeaponState(WeaponState.Idle);
                PacketsToSend.UpdateBullets(player, this);
            }
        }
        public virtual bool IsMagazineEmpty() => magazineCapacity - shootedbullets <= 0;
        public virtual int GetCurrentBulletsAtMagazine() => magazineCapacity - shootedbullets;

        public WeaponState GetWeaponState() => weaponState;
        public void SetPlayer(Player player) => this.player = player;
        public void TrackWeaponStates()
        {
            if (timer == null) return;
            timer.StartTimer();
            if (!timer.IsDone() && // if timer is not done and not realoding and not out of ammo and not drawing weapon set to idle
                weaponState != WeaponState.Reloading &&
               weaponState != WeaponState.DrawWeapon)
                SetWeaponState(WeaponState.Idle);

            if (weaponState == WeaponState.DrawWeapon)
            {
                drawWeaponTimer.StartTimer();
                if (drawWeaponTimer.IsDone())
                    SetWeaponState(WeaponState.Idle);
            }
            if (weaponState == WeaponState.Reloading)
            {
                reloadTimer.StartTimer();
                if (reloadTimer.IsDone())
                {
                    ReloadWeapon(player);
                    return;
                }
            }
        }

        protected virtual void ShootRaycast()
        {
            if (Physics.Raycast(player.shootOrigin.position, player.shootOrigin.forward, out RaycastHit hit, weaponRange))
            {
                if (hit.transform != this.transform)
                {
                    IDamagable damagable = hit.transform.GetComponent<IDamagable>();
                    Surface surface = hit.transform.GetComponent<Surface>();
                    Player killedbyHeadShot = hit.transform.root.GetComponent<Player>();
                    if (killedbyHeadShot != null && hit.transform.GetComponent<BoxCollider>()) // Very simple implementation
                    {
                        float dmg = weaponDamage + player.maxHealth;
                        Debug.Log($"HEADSHOT!!!! {dmg} :red:20;".Interpolate());
                        killedbyHeadShot.TakeDamage(dmg, transform.root, player.attackerType);
                        PacketsToSend.HeadShot(player);
                    }
                    if (damagable != null)
                    {
                        weaponType?.DoDamage(damagable, transform.root, player.attackerType);
                    }
                    if (surface != null)
                    {
                        PacketsToSend.CreateHitEffect(player, hit.point, Quaternion.LookRotation(hit.normal), surface.GetSurfaceType());
                    }
                    SpawnProjectile(hit.point);
                }
            }
        }
        protected virtual void CreateBullet(Transform spawntr, Vector3 direction)
        {
            GameObject gm = Instantiate(projectile, spawntr.position, spawntr.rotation);
            Projectile proj = gm.GetComponent<Projectile>();
            Vector3 dir = (direction - spawntr.position).normalized;
            //Debug.DrawRay(spawntr.position, spawntr.position + dir * 25f, Color.red);
            proj.GetComponent<Rigidbody>().AddForce(spawntr.position + dir * bulletForce, ForceMode.Impulse);
            PacketsToSend.SpawnProjectile(proj);
        }

    }
}