using FPSClient.Timers;
using TMPro;
using UnityEngine;

public class PlayerController : MonoBehaviour, IConsole
{
    [SerializeField] Transform camTransform;

    PlayerInputs playerInputs;
    MouseLock mouseLock;
    PlayerManager playerManager;

    float mouseScroll;
    bool cursorLockDisabled = false;
    int currentWeaponIndex = 0;
    Timer cdTimer;

    void Start()
    {
        cdTimer = new Timer(0, true);
        playerManager = GetComponent<PlayerManager>();
        mouseLock = new MouseLock();
        playerInputs = new PlayerInputs();
        Com com = new Com();
        com.Init();
        com.AddCommand("dscr", "Disables cursor locking");
        ConsoleController.instance.consoles.Add(com, this);
    }
    void Update() => cdTimer.StartTimer();
    void FixedUpdate()
    {
        // ====== DEBUGING ==== TESTING =======
        if (!cursorLockDisabled)
            mouseLock.CursorState();
        // =====================================

        #region Player shoot
        if (CanShoot())
        {
            if (cdTimer.IsDone())
            {

                playerManager.newWeapon.bulletsLeft--;
                if (playerManager.newWeapon.bulletsLeft <= 0)
                {
                    playerManager.newWeapon.bulletsLeft = 0;
                }
                playerManager.playerAmunition.UpdateBulletsLeft(playerManager.newWeapon.bulletsLeft);
                cdTimer.SetTimer(playerManager.newWeapon.coolDown, false);
                PacketsToSend.PlayerShoot(camTransform.forward);
            }
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
            walkInput
        };

        PacketsToSend.SendPlayerInputs(inputs);
        PacketsToSend.SendOtherInputs(otherInputs);
    }
    bool CanShoot()
    {
        return
            playerManager.newWeapon != null &&
            playerInputs.ShootInput((int)playerManager.newWeapon.GetFireMode()) &&
            GetCurState() != WeaponState.Reloading;
    }
    WeaponState GetCurState() => playerManager.newWeapon.weaponState;
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