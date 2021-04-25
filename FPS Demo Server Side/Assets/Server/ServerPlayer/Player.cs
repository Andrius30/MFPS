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

        // ===== Can be refactored, but leave as it is for now =================
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
        // ======================================================================

        [Space(10)]
        [Header("Player Weapons")]
        public Transform weaponsParent;

        public float velocityY;
        [HideInInspector] public float[] inputs;
        [HideInInspector] public bool[] otherInputs;
        public CharacterController characterController;

        [HideInInspector] public PlayerMove playerMove;
        [HideInInspector] public WeaponsController weaponsController;

        void Init()
        {
            characterController = GetComponent<CharacterController>();
            weaponsController = new WeaponsController(this);
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

            Init();
            inputs = new float[2];
            otherInputs = new bool[5];
        }
        void FixedUpdate()
        {
            if (health <= 0) return;
            playerMove.Move(inputs);

            if (weaponsController.GetCurrentWeapon() == null) return;

            Attack(otherInputs[3]);
            if (otherInputs[4] && weaponsController.GetCurrentWeapon().GetWeaponState() != WeaponState.Reloading) // reload weapon input from client
            {
                if (weaponsController.GetCurrentWeapon().IsoutOfAmmo())
                {
                    Debug.Log($"SORRY!!!!!!!!!! BUT YOU ARE OUT OF AMMO!!!! :red:20;".Interpolate());
                    return;
                }
                weaponsController.GetCurrentWeapon().reloadTimer.SetTimer(weaponsController.GetCurrentWeapon().reloadTime, false);
                weaponsController.GetCurrentWeapon().SetWeaponState(WeaponState.Reloading);
            }

            if (weaponsController.GetCurrentWeapon().GetWeaponState() != WeaponState.OutOfAmmo)
                PacketsToSend.WeaponState(this);

            weaponsController.GetCurrentWeapon().TrackWeaponStates();
        }

        public void Attack(bool attack)
        {
            if (attack)
                weaponsController.GetCurrentWeapon().Attack();
            else
                weaponsController.GetCurrentWeapon().acuracy.UpdateWeaponInacuracy(this, false); // inaccuracy
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
        public void SetOtherInputs(bool[] inputs) => this.otherInputs = inputs;
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

    }
}