using UnityEngine;

public class PlayerInputs
{
    #region Move Inputs
    public float HorizontalInputs() => Input.GetAxis("Horizontal");
    public float VerticalInputs() => Input.GetAxis("Vertical");
    public bool JumpInput() => Input.GetKey(KeyCode.Space);
    public bool CrouchInput() => Input.GetKey(KeyCode.C);
    public bool WalkInput() => Input.GetKey(KeyCode.LeftShift);
    #endregion

    #region Shoot Inputs
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
    #endregion

    #region Weapon inputs
    public float MouseScrollInput() => Input.GetAxis("Mouse ScrollWheel");
    public bool ReloadWeapon() => Input.GetKey(KeyCode.R);
    #endregion   

}