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

    void Start()
    {
        playerManager = GetComponent<PlayerManager>();
        mouseLock = new MouseLock();
        playerInputs = new PlayerInputs();
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
        if (playerManager.newWeapon != null && playerInputs.ShootInput((int)playerManager.newWeapon.GetFireMode()))
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
            mouseScroll
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
