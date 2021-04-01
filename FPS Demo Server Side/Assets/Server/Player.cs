using Assets.Server;
using System.Collections;
using UnityEngine;

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
    [Header("Projectile settings")]
    public GameObject projectilePrefab;
    public Transform launchPosition;
    public float throwForce;

    [SerializeField] int itemsHave;
    [SerializeField] int maxItems = 3;

    public float velocityY;
    float[] inputs;
    public bool[] otherInputs;
    public CharacterController characterController;
    public ProjectileSpawner projectileSpawner;

    PlayerMove playerMove;

    void Start()
    {
        playerMove = new PlayerMove(this);
        projectileSpawner = new ProjectileSpawner(this);

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

        inputs = new float[2];
        otherInputs = new bool[2];
    }
    /// <summary>
    /// Processes player input and moves the player.
    /// </summary>

    void FixedUpdate()
    {
        if (health <= 0) return;
        Vector2 _inputDirection = Vector2.zero;
        _inputDirection.x = inputs[0];
        _inputDirection.y = inputs[1];
        playerMove.Move(_inputDirection);
    }

    /// <summary>Updates the player input with newly received input.</summary>
    /// <param name="_inputs">The new key inputs.</param>
    /// <param name="_rotation">The new rotation.</param>
    public void SetInput(float[] inputs, Quaternion rotation)
    {
        this.inputs = inputs;
        transform.rotation = rotation;
    }
    internal void SetOtherInputs(bool[] inputs)
    {
        this.otherInputs = inputs;
    }

    public void Shoot(Vector3 _viewDirection)
    {
        if (Physics.Raycast(shootOrigin.position, _viewDirection, out RaycastHit _hit, 25f))
        {
            IDamagable damagable = _hit.transform.GetComponent<IDamagable>();
            if (damagable != null)
            {
                if (_hit.collider.CompareTag("Player"))
                {
                    _hit.collider.GetComponent<Player>().TakeDamage(50f);
                }
                else if (_hit.collider.CompareTag("Enemy"))
                {
                    _hit.collider.GetComponent<Enemy>().TakeDamage(50f);
                }
            }
        }
    }

    public void TakeDamage(float dmg)
    {
        if (health <= 0) return;
        health -= dmg;
        // send msg how much health left

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

    public void SetItems()
    {
        if (itemsHave < maxItems)
            itemsHave++;
    }
    public void DecreaseItemsByOne()
    {
        if (itemsHave > 0)
            itemsHave--;
    }
    public bool CanPickMoreItems() => itemsHave < maxItems;
    public bool CanThrowProjectile() => itemsHave > 0;
}