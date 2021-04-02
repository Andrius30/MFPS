using System;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public static Action<bool> onLocalPlayerLiveStateChanged;

    [SerializeField] Transform camTransform;
   
    PlayerInputs playerInputs;
    MouseLock mouseLock;
    PlayerManager playerManager;
    
    float mouseScroll;
    bool isDied;

    void Start()
    {
        playerManager = GetComponent<PlayerManager>();
        mouseLock = new MouseLock();
        playerInputs = new PlayerInputs();
        onLocalPlayerLiveStateChanged += SetIsDied;
    }
    void Update()
    {
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
}
