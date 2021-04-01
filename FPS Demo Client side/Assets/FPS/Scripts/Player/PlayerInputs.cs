using UnityEngine;

public class PlayerInputs
{
    // ============ MOVE ========================================================
    public float HorizontalInputs() => Input.GetAxis("Horizontal");
    public float VerticalInputs() => Input.GetAxis("Vertical");
    public bool JumpInput() => Input.GetKey(KeyCode.Space);
    public bool CrouchInput() => Input.GetKey(KeyCode.C);

    // =========== MOVE END =====================================================

    // ========== SHOOT =========================================================

    public bool ShootInput() => Input.GetKeyDown(KeyCode.Mouse0);
    public bool ThrowProjectile() => Input.GetKeyDown(KeyCode.E);

    // ========== SHOOT END =====================================================

}
