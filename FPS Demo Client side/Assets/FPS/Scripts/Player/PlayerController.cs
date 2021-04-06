using System;
using TMPro;
using UnityEngine;

public class PlayerController : MonoBehaviour, IConsole
{
    public static Action<bool> onLocalPlayerLiveStateChanged;

    [SerializeField] Transform camTransform;

    PlayerInputs playerInputs;
    MouseLock mouseLock;
    PlayerManager playerManager;

    float mouseScroll;
    bool isDied;
    bool cursorLockDisabled = false;

    void Start()
    {
        playerManager = GetComponent<PlayerManager>();
        mouseLock = new MouseLock();
        playerInputs = new PlayerInputs();
        onLocalPlayerLiveStateChanged += SetIsDied;
        Com com = new Com();
        com.Init();
        com.AddCommand("dscr","Disables cursor locking");
        ConsoleController.instance.consoles.Add(com, this);
    }
    void Update()
    {
        if (!cursorLockDisabled)
            mouseLock.CursorState();
        // ===================== shoot =================
        if (!isDied && playerManager.newWeapon != null && playerInputs.ShootInput((int)playerManager.newWeapon.GetFireMode()))
        {
            PacketsToSend.PlayerShoot(camTransform.forward);
        }
        mouseScroll = playerInputs.MouseScrollInput();
    }
    void FixedUpdate() => SendInputsToServer();
    void SendInputsToServer()
    {
        // move =================================
        float x = playerInputs.HorizontalInputs();
        float z = playerInputs.VerticalInputs();
        playerManager.PlayMoveAnimation(x, z);

        bool jumpInput = playerInputs.JumpInput();
        bool crouchInput = playerInputs.CrouchInput();

        float[] inputs = new float[]
        {
            x,
            z,
            mouseScroll
        };
        bool[] otherInputs = new bool[]
        {
            jumpInput,
            crouchInput
        };

        PacketsToSend.SendPlayerInputs(inputs);
        PacketsToSend.SendOtherInputs(otherInputs);
    }
    void SetIsDied(bool isAlive) => isDied = isAlive;

    public void Execute()
    {
        if (cursorLockDisabled)
            mouseLock.UnLockCursor();
    }

    public void PrintToConsole(ref TextMeshProUGUI output, string prefix)
    {
        output.text += $" { prefix } Cursor locking has been disabled.";
    }
}
