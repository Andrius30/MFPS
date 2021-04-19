using UnityEngine;

public class PlayerInputs
{
    // ============ MOVE ========================================================
    public float HorizontalInputs() => Input.GetAxis("Horizontal");
    public float VerticalInputs() => Input.GetAxis("Vertical");
    public bool JumpInput() => Input.GetKey(KeyCode.Space);
    public bool CrouchInput() => Input.GetKey(KeyCode.C);
    public bool WalkInput() => Input.GetKey(KeyCode.LeftShift);

    // =========== MOVE END =====================================================

    // ========== SHOOT =========================================================
    public bool ShootInput() => Input.GetKey(KeyCode.Mouse0);
    public bool ShootInput(int fireMode)
    {
        switch (fireMode)
        {
            case (int)FireMode.normal:
                return Input.GetKeyDown(KeyCode.Mouse0);
            case (int)FireMode.burst:
                return Input.GetKeyDown(KeyCode.Mouse0);
            case (int)FireMode.auto:
                return Input.GetKey(KeyCode.Mouse0);
        }
        return false;
    }

    // ========== SHOOT END =====================================================

    // =========== WEAPONs ========================================================
    public float MouseScrollInput() => Input.GetAxis("Mouse ScrollWheel");

    // =========== Weapons END ==================================================
}