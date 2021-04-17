using MFPS.ServerTimers;
using MFPS.Weapons;
using MFPS.Weapons.Controllers;
using System.Collections;
using UnityEngine;

namespace MFPS.ServerCharacters
{
    public class Player : MonoBehaviour, IDamagable
    {
        public AttackerTypes attackerType = AttackerTypes.Player;

        [HideInInspector] public int id;
        [HideInInspector] public string userName;

        [Header("Shoot position")]
        public Transform shootOrigin;
        [Space(10)]
        [Header("Health settings")]
        public float maxHealth = 100;
        [HideInInspector] public float health;

        [Space(10)]
        [Header("Move settings")]
        [HideInInspector] public float moveSpeed;
        public float runSpeed = 5f;
        public float crouchSpeed = 2f;
        public float walkSpeed = 2.5f;
        public float crouchCenter = -.4f;
        [Space(10)]
        [Header("Jump settings")]
        public float jumpSpeed = 10f;

        [Space(10)]
        [Header("Gravity modifier")]
        public float gravity = -9.81f;
        [Space(10)]
        [Header("Player Weapons")]
        public Transform weaponsParent;

        public float velocityY;
        [HideInInspector] public float[] inputs;
        [HideInInspector] public bool[] otherInputs;
        public CharacterController characterController;

        [HideInInspector] public PlayerMove playerMove;
        [HideInInspector] public WeaponsController weaponsController;

        Timer timer;
        Timer drawWeaponTimer;
        Timer reloadTimer;

        void Start()
        {
            weaponsController = new WeaponsController(this);

            timer = new Timer(0, false);
            drawWeaponTimer = new Timer(0, true);
            reloadTimer = new Timer(0, true);
            playerMove = new PlayerMove(this);
            gravity *= Time.fixedDeltaTime * Time.fixedDeltaTime;
            moveSpeed *= Time.fixedDeltaTime;
            runSpeed *= Time.fixedDeltaTime;
            crouchSpeed *= Time.fixedDeltaTime;
            walkSpeed *= Time.fixedDeltaTime;
            jumpSpeed *= Time.fixedDeltaTime;
            health = maxHealth;
        }

        public void Initialize(int id, string userName)
        {
            this.id = id;
            this.userName = userName;
            characterController = GetComponent<CharacterController>();

            inputs = new float[2];
            otherInputs = new bool[3];
        }
        void Update()
        {
            TrackWeaponStates();
        }

        private void TrackWeaponStates()
        {
            if (GetCurrentWeapon() == null) return;
            timer.StartTimer();
            if (!timer.IsDone() && // if timer is not done and not realoding and not out of ammo and not drawing weapon set to idle
                WepState() != WeaponState.Reloading &&
                WepState() != WeaponState.DrawWeapon)
                weaponsController.GetCurrentWeapon().SetWeaponState(WeaponState.Idle);

            if (WepState() == WeaponState.DrawWeapon)
            {
                drawWeaponTimer.StartTimer();
                if (drawWeaponTimer.IsDone())
                    GetCurrentWeapon().SetWeaponState(WeaponState.Idle);
            }
            if (WepState() == WeaponState.Reloading)
            {
                reloadTimer.StartTimer();
                if (reloadTimer.IsDone())
                {
                    GetCurrentWeapon().ReloadWeapon(this);
                    return;
                }
            }
        }

        void FixedUpdate()
        {
            if (health <= 0) return;
            playerMove.Move(inputs);

            if (GetCurrentWeapon() == null) return;

            if (WepState() != WeaponState.OutOfAmmo)
                PacketsToSend.WeaponState(this);
        }
        public void Shoot(Vector3 _viewDirection)
        {
            if (health <= 0) return;
            if (WepState() == WeaponState.Reloading ||
                WepState() == WeaponState.DrawWeapon)
                return;

            if (timer.IsDone())
            {
                if (WepState() == WeaponState.OutOfAmmo)
                {
                    PacketsToSend.WeaponState(this);
                    return;
                }

                GetCurrentWeapon().SetWeaponState(WeaponState.Shooting);

                if (GetCurrentWeapon().IsMagazineEmpty())
                {
                    if (GetCurrentWeapon().IsoutOfAmmo())
                    {
                        GetCurrentWeapon().SetWeaponState(WeaponState.OutOfAmmo);
                        Debug.Log($"Out Of Ammo!! :red;".Interpolate());
                        return;
                    }
                    reloadTimer.SetTimer(GetCurrentWeapon().reloadTime, false);
                    GetCurrentWeapon().SetWeaponState(WeaponState.Reloading);
                    return;
                }
                if (Physics.Raycast(shootOrigin.position, _viewDirection, out RaycastHit _hit, GetCurrentWeapon().weaponRange))
                {
                    IDamagable damagable = _hit.transform.GetComponent<IDamagable>();
                    if (damagable != null)
                    {
                        weaponsController.GetCurrentWeaponType()?.DoDamage(damagable, transform.root, attackerType);
                    }
                }
                GetCurrentWeapon().SpawnProjectile();
                timer.SetTimer(weaponsController.GetCoolDown(), false);
            }
        }

        #region inputs Section
        /// <summary>Updates the player input with newly received input.</summary>
        /// <param name="_inputs">The new key inputs.</param>
        /// <param name="_rotation">The new rotation.</param>
        public void SetInput(float[] inputs, Quaternion rotation)
        {
            this.inputs = inputs;
            transform.rotation = rotation;
        }
        public void SetOtherInputs(bool[] inputs)
        {
            this.otherInputs = inputs;
        }
        public void UpdateAimingPivotRotation(float angle, Quaternion localRot)
        {
            weaponsParent.localRotation = localRot;
            PacketsToSend.PlayerAimingRotation(this, angle, weaponsParent.localRotation);
        }
        #endregion

        #region Damage section
        public void TakeDamage(float dmg, Transform attacker, AttackerTypes type)
        {
            if (health <= 0) return;
            health -= dmg;
            Debug.Log($"Player geting {dmg} dmg from aattacker {attacker.name } attacker type {type}:yellow;".Interpolate());
            if (health <= 0)
                Die();

            PacketsToSend.PlayerHealth(this);
            PacketsToSend.SendAttackerAndDamage(attacker, dmg, id, type);
        }
        public void Die()
        {
            health = 0;
            characterController.enabled = false;
            transform.position = new Vector3(0, 25f, 0);
            PacketsToSend.PlayerPosition(this);
            StartCoroutine(Respawn());
        }
        IEnumerator Respawn()
        {
            yield return new WaitForSeconds(5f);

            health = maxHealth;
            characterController.enabled = true;
            PacketsToSend.PlayerRespawned(this);
        }
        #endregion
        WeaponState WepState() => GetCurrentWeapon().GetWeaponState();
        BaseWeapon GetCurrentWeapon() => weaponsController.GetCurrentWeapon();

    }
}