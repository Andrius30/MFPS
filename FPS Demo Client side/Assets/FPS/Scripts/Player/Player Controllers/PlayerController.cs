using TMPro;
using UnityEngine;

public class PlayerController : MonoBehaviour, IConsole
{
    [SerializeField] Transform camTransform; // REMOVE AFTER DEBUGING!! =====================================================

    PlayerInputs playerInputs;
    MouseLock mouseLock;
    PlayerManager playerManager;

    float mouseScroll;
    bool cursorLockDisabled = false;
    int currentWeaponIndex = 0;

    void Start()
    {
        playerManager = GetComponent<PlayerManager>();
        mouseLock = new MouseLock();
        playerInputs = new PlayerInputs();
        Com com = new Com();
        com.Init();
        com.AddCommand("dscr", "Disables cursor locking");
        ConsoleController.instance.consoles.Add(com, this);
    }
    void FixedUpdate()
    {
        // ====== DEBUGING ==== TESTING =======
        if (!cursorLockDisabled)
            mouseLock.CursorState();
        // =====================================
        if (playerManager.newWeapon == null) return;
        #region Player shoot
        if (CanShoot())
        {
            playerManager.newWeapon.bulletsLeft--;
            if (playerManager.newWeapon.bulletsLeft <= 0)
            {
                playerManager.newWeapon.bulletsLeft = 0;
            }
            playerManager.playerAmunition.UpdateBulletsLeft(playerManager.newWeapon.bulletsLeft);
        }
        #endregion

        #region Changing and sending weapon index
        mouseScroll = playerInputs.MouseScrollInput();

        if (mouseScroll > 0) // up
        {
            currentWeaponIndex++;
            if (currentWeaponIndex > playerManager.GetWeaponsLength() - 1)
                currentWeaponIndex = 0;
            PacketsToSend.PlayerChangingWeapon(currentWeaponIndex);
        }
        else if (mouseScroll < 0) // down
        {
            currentWeaponIndex--;
            if (currentWeaponIndex < 0)
                currentWeaponIndex = playerManager.GetWeaponsLength() - 1;
            PacketsToSend.PlayerChangingWeapon(currentWeaponIndex);
        }
        #endregion

        SendInputsToServer();
    }
    void SendInputsToServer()
    {
        // move =================================
        float x = playerInputs.HorizontalInputs();
        float z = playerInputs.VerticalInputs();
        bool jumpInput = playerInputs.JumpInput();
        bool crouchInput = playerInputs.CrouchInput();
        bool walkInput = playerInputs.WalkInput();
        bool isShooting = playerInputs.ShootInput(playerManager.newWeapon.GetFireMode());
        playerManager.playerAnimations.PlayMoveAnimation(x, z);
        playerManager.playerAnimations.PlayCrouchAnimation(crouchInput);
        playerManager.playerAnimations.PlayWalkAnimation(walkInput);

        float[] inputs = new float[]
        {
            x,
            z,
        };
        bool[] otherInputs = new bool[]
        {
            jumpInput,
            crouchInput,
            walkInput,
            isShooting
        };

        PacketsToSend.SendPlayerInputs(inputs);
        PacketsToSend.SendOtherInputs(otherInputs);
    }
    bool CanShoot()
    {
        return
            playerManager.newWeapon != null &&
            GetCurState() == WeaponState.Shooting &&
            GetCurState() != WeaponState.Reloading;
    }
    WeaponState GetCurState() => playerManager.newWeapon == null ? WeaponState.Idle : playerManager.newWeapon.weaponState;
    #region Developer console test
    public void Execute()
    {
        if (cursorLockDisabled)
            mouseLock.UnLockCursor();
    }

    public void PrintToConsole(ref TextMeshProUGUI output, string prefix)
    {
        output.text += $" { prefix } Cursor locking has been disabled.";
    }
    #endregion
}
