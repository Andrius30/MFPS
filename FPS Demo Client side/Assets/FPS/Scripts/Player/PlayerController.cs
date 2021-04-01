using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] Transform cantransform;

    MouseLock mouseLock;
    PlayerInputs playerInputs;

    void Start()
    {
        playerInputs = new PlayerInputs();
        mouseLock = new MouseLock();
    }
    void Update()
    {
        // ===================== shoot =================
        if (!PlayerManager.isDied && playerInputs.ShootInput())
        {
            PacketsToSend.PlayerShoot(cantransform.forward);
        }
        if (!PlayerManager.isDied && playerInputs.ThrowProjectile())
        {
            PacketsToSend.PlayerThrowProjectile(cantransform.forward);
        }
    }
    void FixedUpdate()
    {
        mouseLock.CursorState();
        SendInputsToServer();
    }

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
            z
        };
        bool[] otherInputs = new bool[]
        {
            jumpInput,
            crouchInput,
        };

        PacketsToSend.SendPlayerInputs(inputs);
        PacketsToSend.SendOtherInputs(otherInputs);
    }
}
