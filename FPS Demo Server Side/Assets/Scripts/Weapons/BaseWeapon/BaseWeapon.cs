using UnityEngine;

public enum FireMode
{
    normal = 1,
    burst,
    auto
}
namespace MFPS.Weapons
{
    public class BaseWeapon : MonoBehaviour, IWeapon
    {
        public FireMode firemode;

        public int id;
        public string weaponName;
        [Space(10)]
        [Header("Projectiles")]
        public GameObject projectile;
        [SerializeField] Transform projectileSpawnPosition;
        [SerializeField] float bulletForce = 100f;

        [Header("Weapon damage seetings")]
        [SerializeField] float weaponDamage;
        [Space(10)]
        [Header("Weapon cooldown settings")]
        public float coolDown;

        public IWeapon weaponType;
        int count = 0;

        public void DoDamage(IDamagable damagable)
        {
            damagable.TakeDamage(weaponDamage);
        }
        public void SpawnProjectile(Vector3 direction)
        {
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
        }

        private void CreateBullet(Vector3 direction)
        {
            GameObject gm = Instantiate(projectile, projectileSpawnPosition.position, projectileSpawnPosition.rotation);
            Projectile proj = gm.GetComponent<Projectile>();
            proj.GetComponent<Rigidbody>().AddForce(projectileSpawnPosition.position + direction * bulletForce, ForceMode.Impulse);
            PacketsToSend.SpawnProjectile(proj);
        }
    }
}