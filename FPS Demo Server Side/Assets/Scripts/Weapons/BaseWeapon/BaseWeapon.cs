using MFPS.ServerCharacters;
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
        public Transform shootPos;
        [SerializeField] float bulletForce = 100f;
        [SerializeField] int totalBullets = 100;
        [SerializeField] int magazineCapacity = 20;
        public int GetMaxBullets => totalBullets;
        int shootedbullets;

        [Space(10)]
        [Header("Hit Effects Setting")]
        //[SerializeField] GameObject hitEffectprefab;
        [SerializeField] Vector3 hitOffset;

        [Header("Weapon damage seetings")]
        [SerializeField] float weaponDamage;
        [Space(10)]
        [Header("Weapon cooldown settings")]
        public float coolDown;
        public float weaponDrawTime = 2f;
        public float reloadTime = 1f;

        public IWeapon weaponType;
        int count = 0;
        public Acuracy acuracy;

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
            acuracy.Initialize();
            weaponModel.localPosition = modelPosition;
            weaponModel.localRotation = modelRotation;
            this.shootPos.localPosition = shootPos;
            this.shootPos.localRotation = shootRot;
            //Debug.Log($"Weapon ID {id} initialized with position { modelPosition} and rotation {modelRotation} :green:18;".Interpolate());
        }

        public virtual void DoDamage(IDamagable damagable, Transform attacker, AttackerTypes type)
        {
            damagable.TakeDamage(weaponDamage, attacker, type);
        }

        public void SpawnProjectile()
        {
            if (firemode == FireMode.auto)
            {
                count++;
                if (count >= 3)
                {
                    CreateBullet(shootPos);
                    count = 0;
                }
            }
            else
            {
                CreateBullet(shootPos);
            }
            shootedbullets++;
        }
        public virtual bool IsoutOfAmmo() => totalBullets - shootedbullets < 0;
        public virtual void SetWeaponState(WeaponState state) => weaponState = state;
        public WeaponState GetWeaponState() => weaponState;
        public void ReloadWeapon(Player player)
        {
            if (totalBullets - shootedbullets >= 0)
            {
                totalBullets -= shootedbullets;
                shootedbullets = 0;
                SetWeaponState(WeaponState.Idle);
                PacketsToSend.UpdateBullets(player, this);
            }
        }
        public bool IsMagazineEmpty() => magazineCapacity - shootedbullets <= 0;

        void CreateBullet(Transform spawntr)
        {
            GameObject gm = Instantiate(projectile, spawntr.position, spawntr.rotation);
            Projectile proj = gm.GetComponent<Projectile>();
            proj.GetComponent<Rigidbody>().AddForce(spawntr.forward * bulletForce, ForceMode.Impulse);
            PacketsToSend.SpawnProjectile(proj);
        }
        public int GetCurrentBulletsAtMagazine() => magazineCapacity - shootedbullets;
    }
}