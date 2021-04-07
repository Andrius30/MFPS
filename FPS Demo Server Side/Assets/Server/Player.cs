using MFPS.ServerTimers;
using MFPS.Weapons;
using MFPS.Weapons.Controllers;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MFPS.ServerCharacters
{
    public class Player : MonoBehaviour, IDamagable
    {
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

        PlayerMove playerMove;
        Timer timer;
        [HideInInspector] public WeaponsController weaponsController;

        void Start()
        {
            timer = new Timer(0, false);
            weaponsController = new WeaponsController(this);
            playerMove = new PlayerMove(this);
            gravity *= Time.fixedDeltaTime * Time.fixedDeltaTime;
            moveSpeed *= Time.fixedDeltaTime;
            runSpeed *= Time.fixedDeltaTime;
            crouchSpeed *= Time.fixedDeltaTime;
            jumpSpeed *= Time.fixedDeltaTime;
            health = maxHealth;
        }

        public void Initialize(int id, string userName)
        {
            this.id = id;
            this.userName = userName;
            characterController = GetComponent<CharacterController>();

            inputs = new float[3];
            otherInputs = new bool[2];
        }
        void Update() => timer.StartTimer();
        void FixedUpdate()
        {
            if (health <= 0) return;
            Vector2 _inputDirection = Vector2.zero;
            _inputDirection.x = inputs[0];
            _inputDirection.y = inputs[1];
            playerMove.Move(_inputDirection);

            PacketsToSend.PlayMovementAnimation(this, _inputDirection.x, _inputDirection.y);
            weaponsController.ChangeWeapon(this);
        }
        public void Shoot(Vector3 _viewDirection)
        {
            if (timer.IsDone())
            {
                PacketsToSend.PlayerShoot(this);
                weaponsController.GetCurrentWeapon().SpawnProjectile(_viewDirection);
                if (Physics.Raycast(shootOrigin.position, _viewDirection, out RaycastHit _hit, 25f))
                {
                    IDamagable damagable = _hit.transform.GetComponent<IDamagable>();
                    if (damagable != null)
                    {
                        weaponsController.GetCurrentWeaponType()?.DoDamage(damagable);
                    }
                }
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
        public void TakeDamage(float dmg)
        {
            if (health <= 0) return;
            health -= dmg;

            if (health <= 0)
                Die();

            PacketsToSend.PlayerHealth(this);
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